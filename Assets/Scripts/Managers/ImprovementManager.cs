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

    // Empty parent gameObjects
    public GameObject houseParent, farmParent, mineParent;

    // Improvement prefabs
    public GameObject housePrefab, farmPrefab, minePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

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

        // Check if a resource is currently selected
        if(tile.GetComponent<Resource>() != null)
		{
            tile = tile.GetComponent<Resource>().tile;
        }

        // Ends early if there is already an improvement
        // OR the tile's resource does not match up with the improvement trying to be built
        if(tile.GetComponent<Tile>().improvement != null
            || !IsResourceCapatible(tile, improvementType))
            return null;

        // Create a new GameObject based on the type
        GameObject newImprovement = null;
        switch(improvementType)
        {
            case ImprovementType.House:
                newImprovement = Instantiate(housePrefab, houseParent.transform);
                // Update resource values
                int space = newImprovement.GetComponent<House>().space;
                GameManager.instance.UpdateHousing(space);
                break;
            case ImprovementType.Farm:
                newImprovement = Instantiate(farmPrefab, farmParent.transform);
                newImprovement.GetComponent<Producer>().produceEvent = new UnityProduceEvent();
                newImprovement.GetComponent<Producer>().produceEvent.AddListener(GameManager.instance.UpdateResourceAmount);
                // Update resource values
                int foodProduction = newImprovement.GetComponent<Producer>().productionAmount;
                GameManager.instance.UpdateProduction(improvementType, foodProduction);
                break;
            case ImprovementType.Mine:
                tile.GetComponent<Tile>().resource.SetActive(false);
                newImprovement = Instantiate(minePrefab, mineParent.transform);
                newImprovement.GetComponent<Producer>().produceEvent = new UnityProduceEvent();
                newImprovement.GetComponent<Producer>().produceEvent.AddListener(GameManager.instance.UpdateResourceAmount);
                // Update resource values
                int stoneProduction = newImprovement.GetComponent<Producer>().productionAmount;
                GameManager.instance.UpdateProduction(improvementType, stoneProduction);
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
    /// Checks to make sure the tile and improvement are applicable
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
}
