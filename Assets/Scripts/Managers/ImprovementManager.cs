using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImprovementType
{
    House,
    Farm,
    Mine,
    LumberMill
}

public enum ResourceType
{
    Population,
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
    public GameObject farmPrefab, housePrefab, minePrefab, lumberMillPrefab;

    public Dictionary<ImprovementType, ImprovementDesc> improvementDescriptions; 

    // Start is called before the first frame update
    void Start()
    {
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
        improvementDescriptions = new Dictionary<ImprovementType, ImprovementDesc>();

        // Add improvement descriptions for each improvement
        improvementDescriptions.Add(
            ImprovementType.House,
            new ImprovementDesc(
                ImprovementType.House,
                ResourceType.Population,
                housePrefab,
                FindImprovementParentGameObj(ImprovementType.House)));
        improvementDescriptions.Add(
            ImprovementType.Farm, 
            new ImprovementDesc(
                ImprovementType.Farm, 
                ResourceType.Food,
                farmPrefab, 
                FindImprovementParentGameObj(ImprovementType.Farm), 
                true, false));
        improvementDescriptions.Add(
            ImprovementType.Mine, 
            new ImprovementDesc(
                ImprovementType.Mine,
                ResourceType.Stone,
                minePrefab,
                FindImprovementParentGameObj(ImprovementType.Mine), 
                true, true));
        improvementDescriptions.Add(
            ImprovementType.LumberMill,
            new ImprovementDesc(
                ImprovementType.LumberMill,
                ResourceType.Wood,
                lumberMillPrefab,
                FindImprovementParentGameObj(ImprovementType.LumberMill),
                true, true));

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
    /// <param name="isBeingLoaded">Whether the improvement is being loaded, not built</param>
    /// <returns>The newly created improvement gameObject</returns>
    public void BuildImprovement(ImprovementType improvementType, bool isBeingLoaded)
    {
        // Get the currently selected object
        GameObject tile = TileSelector.instance.GetSelectedObject();
        BuildImprovement(improvementType, tile, isBeingLoaded);        
    }

    /// <summary>
    /// Builds an improvement of the given type on the given tile
    /// </summary>
    /// <param name="improvementType">The type of improvement being created</param>
    /// <param name="tile">The tile the improvement is built on</param>
    /// <param name="isBeingLoaded">Whether the improvement is being loaded, not built</param>
    public void BuildImprovement(ImprovementType improvementType, GameObject tile, bool isBeingLoaded)
    {
        // End early if nothing is currently selected
        if(tile == null)
            return;

        // DeactiveSelect the resource if one exists on the tile
        if(tile.GetComponent<Resource>() != null)
            tile = tile.GetComponent<Resource>().tile;

        // Check several requirements for if the improvement can be built on the tile
        if(!CanBuild(tile, improvementType, isBeingLoaded))
            return;

        Build(tile, improvementType, isBeingLoaded);
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
                GameManager.instance.UpdateProduction(
                    ImprovementType.House, 
                    -improvement.GetComponent<House>().space);
                break;
            case ImprovementType.Farm:
            case ImprovementType.Mine:
            case ImprovementType.LumberMill:
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
    /// Checks if an improvement type can be built on a tile
    /// </summary>
    /// <param name="tile">The tile the improvement will be built on</param>
    /// <param name="improvementType">The type of improvement that is attempted to be built</param>
    /// <param name="isBeingLoaded">Whether the improvement is being loaded, not built</param>
    /// <returns>Whether that type of improvement can be built on that tile</returns>
    private bool CanBuild(GameObject tile, ImprovementType improvementType, bool isBeingLoaded)
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

        if(isBeingLoaded)
            return true;

        // Check if the player has enough resources to build the improvement
        if(!HasResoucesToBuild(improvementType))
		{
            Debug.Log("You do not have enough resources");
            return false;
		}

        return true;
    }

    /// <summary>
    /// Builds an improvement
    /// </summary>
    /// <param name="tile">The tile where the improvement is being built on</param>
    /// <param name="improvementType">The improvement being built</param>
    /// <param name="isBeingLoaded">Whether the improvement is being loaded, not built</param>
    /// <returns>The built improvement</returns>
    private GameObject Build(GameObject tile, ImprovementType improvementType, bool isBeingLoaded)
	{
        if(!isBeingLoaded)
            RemoveResourcesForBuild(improvementType);

        // Create a new GameObject based on the type
        GameObject newImprovement = Instantiate(
            improvementDescriptions[improvementType].Prefab,
            improvementDescriptions[improvementType].ParentObj.transform);
        

        if(improvementType == ImprovementType.House)
		{
            int space = newImprovement.GetComponent<House>().space;
            GameManager.instance.UpdateProduction(ImprovementType.House, space);
        } else if(improvementDescriptions[improvementType].IsProducer)
            GameManager.instance.UpdateProduction(improvementType, (int)improvementDescriptions[improvementType].ProdAmount);

        if(improvementDescriptions[improvementType].IsOnResource)
            tile.GetComponent<Tile>().resource.SetActive(false);

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
            case ImprovementType.LumberMill:
                if(tile.GetComponent<Tile>().resource != null
                    && tile.GetComponent<Tile>().resource.GetComponent<Resource>().type == ResourceType.Wood)
                    return true;
                break;
        }

        return false;
	}

    /// <summary>
    /// Checks if the player enough of each of the resources required to build the improvement
    /// </summary>
    /// <param name="improvementType">The type of improvement trying to be built</param>
    /// <returns>Whether the player has enough resources</returns>
    private bool HasResoucesToBuild(ImprovementType improvementType)
	{
        Dictionary<ResourceType, int> resourceCosts = improvementDescriptions[improvementType].ResourceBuildCosts;

        foreach(ResourceType resource in resourceCosts.Keys)
            if(GameManager.instance.resources[resource].CurrentAmount < resourceCosts[resource])
                return false;

        return true;
	}

    /// <summary>
    /// A helper method to remove the resources needed to build an improvement
    /// </summary>
    /// <param name="improvementType">The improvement being built</param>
    private void RemoveResourcesForBuild(ImprovementType improvementType)
	{
        Dictionary<ResourceType, int> resourceCosts = improvementDescriptions[improvementType].ResourceBuildCosts;

        foreach(ResourceType resource in resourceCosts.Keys)
            GameManager.instance.resources[resource].AddAmount(-resourceCosts[resource]);
    }

    /// <summary>
    /// A helper method to find the right empty parent gameObj for the right improvement
    /// </summary>
    /// <param name="improvementType">The type of improvement</param>
    /// <returns>The empty parent gameObj for that improvement</returns>
    private GameObject FindImprovementParentGameObj(ImprovementType improvementType)
	{
        foreach(Transform subParentTrans in WorldGenerator.instance.improvementParent.transform)
            if(subParentTrans.gameObject.name.ToLower() == improvementType.ToString().ToLower() + "s")
                return subParentTrans.gameObject;

        return null;
	}
}
