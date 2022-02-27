using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImprovementType
{
    House,
    Farm,
    Mine
}

public enum ResourceType
{
    People,
    Food,
    Wood,
    Stone,
    Metal
}

public class ImprovementManager : MonoBehaviour
{
    public static ImprovementManager instance = null;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
    }

    // Improvement prefabs
    public GameObject farmPrefab, housePrefab, minePrefab;
    // Empty parent gameObjects
    public GameObject farmParent, houseParent, mineParent;

    private Dictionary<ImprovementType, ImprovementDesc> improvementDescriptions; 

    // Start is called before the first frame update
    void Start()
    {
        improvementDescriptions = new Dictionary<ImprovementType, ImprovementDesc>();
        FillImprovementDescriptions();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Adds an improvement description for each improvement
    /// </summary>
    private void FillImprovementDescriptions()
	{
        // Add improvement descriptions for each improvement
        improvementDescriptions.Add(ImprovementType.Farm, new ImprovementDesc(ImprovementType.Farm, farmPrefab, farmParent, true, false));
        improvementDescriptions.Add(ImprovementType.House, new ImprovementDesc(ImprovementType.House, housePrefab, houseParent, false, false));
        improvementDescriptions.Add(ImprovementType.Mine, new ImprovementDesc(ImprovementType.Mine, minePrefab, mineParent, true, true));

        // Add resource costs for each improvement
        // Farm - 10 wood
        improvementDescriptions[ImprovementType.Farm].AddResourceCost(ResourceType.Wood, 10);
        // House - 20 wood, 10 food
        improvementDescriptions[ImprovementType.House].AddResourceCost(ResourceType.Wood, 20);
        improvementDescriptions[ImprovementType.House].AddResourceCost(ResourceType.Food, 10);
        // Mine - 15 wood
        improvementDescriptions[ImprovementType.Mine].AddResourceCost(ResourceType.Wood, 15);
    }

    /// <summary>
    /// Builds an improvement of the given type
    /// </summary>
    /// <param name="improvementType">The type of improvement being created</param>
    /// <returns>The newly created improvement gameObject</returns>
    public GameObject BuildImprovement(ImprovementType improvementType)
    {
        // Get the currently selected object
        GameObject tile = TileSelector.instance.GetSelectedObject();

        // End early if nothing is currently selected
        if(tile == null) return null;

        // DeactiveSelect the resource if one exists on the tile
        if(tile.GetComponent<Resource>() != null)
            tile = tile.GetComponent<Resource>().tile;

        // Check several requirements for if the improvement can be built on the tile
        if(!CanBuild(tile, improvementType))
            return null;

        // Create a new GameObject based on the type
        GameObject newImprovement = Instantiate(
            improvementDescriptions[improvementType].Prefab,
            improvementDescriptions[improvementType].ParentObj.transform);
		if(improvementDescriptions[improvementType].IsProducer)
		{
            newImprovement.GetComponent<Producer>().produceEvent = new UnityProduceEvent();
            newImprovement.GetComponent<Producer>().produceEvent.AddListener(GameManager.instance.UpdateResourceAmount);
            GameManager.instance.UpdateProduction(improvementType, (int)improvementDescriptions[improvementType].ProdAmount);
        }

        // Resource-unique logic
        switch(improvementType)
        {
            case ImprovementType.House:
                int space = newImprovement.GetComponent<House>().space;
                GameManager.instance.UpdateHousing(space);
                break;
            case ImprovementType.Mine:
                tile.GetComponent<Tile>().resource.SetActive(false);
                break;
        }

        // Set initial data
        newImprovement.name = improvementType.ToString();
        newImprovement.transform.position = tile.transform.position;
        newImprovement.GetComponent<Improvement>().tile = tile;
        newImprovement.GetComponent<Selectable>().unityEvent = TileSelector.instance.selectEvent;
        tile.GetComponent<Tile>().improvement = newImprovement;

        // Update the current selected object
        TileSelector.instance.SelectObject(newImprovement);

        return newImprovement;
    }

    /// <summary>
    /// Destroys the current selected object if it is an improvement
    /// </summary>
    public void DestoryImprovement()
	{
        // Get the currently selected object
        GameObject improvement = TileSelector.instance.GetSelectedObject();

        // End early if nothing is selected or whatever is selected is not an improvement
        if(improvement == null || improvement.GetComponent<Improvement>() == null)
            return;

        // Remove the production the improvement was providing
        switch(improvement.GetComponent<Improvement>().type)
        {
            case ImprovementType.House:
                GameManager.instance.UpdateHousing(-improvement.GetComponent<House>().space);
                break;
            case ImprovementType.Farm:
            case ImprovementType.Mine:
                GameManager.instance.UpdateProduction(
                    improvement.GetComponent<Improvement>().type,
                    -improvement.GetComponent<Producer>().productionAmount);
                break;
        }

        // Get the tile of the improvement, remove the improvement from it, and set it as the new selected object
        GameObject tile = improvement.GetComponent<Improvement>().tile;
        tile.GetComponent<Tile>().improvement = null;
        TileSelector.instance.SelectObject(tile);

        Destroy(improvement);
	}

    /// <summary>
    /// A helper function that matches the improvement type to resource type
    /// </summary>
    /// <param name="improvementType">The improvement</param>
    /// <returns>The resource the improvement provides</returns>
    public ResourceType ImprovementToResource(ImprovementType improvementType)
	{
        ResourceType resource = ResourceType.People;
        switch(improvementType)
        {
            case ImprovementType.House:
                resource = ResourceType.People;
                break;
            case ImprovementType.Farm:
                resource = ResourceType.Food;
                break;
            case ImprovementType.Mine:
                resource = ResourceType.Stone;
                break;
        }

        return resource;
	}

    /// <summary>
    /// Checks if an improvement type can be built on a tile
    /// </summary>
    /// <param name="tile">The tile the improvement will be built on</param>
    /// <param name="improvementType">The type of improvement that is attempted to be built</param>
    /// <returns>Whether that type of improvement can be built on that tile</returns>
    private bool CanBuild(GameObject tile, ImprovementType improvementType)
	{
        // Check if there is already an improvement on the tile
        if(tile.GetComponent<Tile>().improvement != null)
		{
            Debug.Log("There is already an improvement on this tile");
            return false;
		}

        // Make sure the tile and improvement check out resource-wise
        if(!IsResourceCapatible(tile, improvementType))
		{
            Debug.Log("This is the wrong improvement for this tile");
            return false;
		}

        // Check if the player has enough resources to build the improvement
        if(!HasResoucesToBuild(improvementType))
		{
            Debug.Log("You do not have enough resources");
            return false;
		}

        return true;
    }

    /// <summary>
    /// Checks to make sure the tile and improvement are capatible
    /// (ex: mines being built on mountains, etc)
    /// </summary>
    /// <param name="tile">The currently selected tile</param>
    /// <param name="improvementType">The improvement attempting to be built</param>
    /// <returns></returns>
    private bool IsResourceCapatible(GameObject tile, ImprovementType improvementType)
	{
        switch(improvementType)
        {
            case ImprovementType.House:
            case ImprovementType.Farm:
                return tile.GetComponent<Tile>().resource == null;
            case ImprovementType.Mine:
                if(tile.GetComponent<Tile>().resource != null
                    && tile.GetComponent<Tile>().resource.GetComponent<Resource>().type == ResourceType.Stone)
                    return true;
                break;
        }

        return false;
	}

    private bool HasResoucesToBuild(ImprovementType improvementType)
	{
        Dictionary<ResourceType, int> resourceCosts = improvementDescriptions[improvementType].ResourceCosts;

        foreach(ResourceType resource in resourceCosts.Keys)
            if(GameManager.instance.resources[resource].Item1 < resourceCosts[resource])
                return false;

        return true;
	}
}
