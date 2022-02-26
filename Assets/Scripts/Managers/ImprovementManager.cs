using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImprovementType
{
    House,
    Farm,
    Mine
}

public class ImprovementManager : MonoBehaviour
{
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
    /// Builds a house improvement
    /// </summary>
    public void BuildHouse()
	{
        BuildImprovement(ImprovementType.House);
    }

    /// <summary>
    /// Builds a farm improvement
    /// </summary>
    public void BuildFarm()
    {
        BuildImprovement(ImprovementType.Farm);
    }

    /// <summary>
    /// Builds a mine improvement
    /// </summary>
    public void BuildMine()
    {
        BuildImprovement(ImprovementType.Mine);
    }

    /// <summary>
    /// Builds an improvement of the given type
    /// </summary>
    /// <param name="improvementType">The type of improvement being created</param>
    /// <returns>The newly created improvement gameObject</returns>
    private GameObject BuildImprovement(ImprovementType improvementType)
    {
        // Get the currently selected object
        GameObject tile = GetComponent<TileSelector>().GetSelectedObject();

        // End early if nothing is currently selected
        if(tile == null)
            return null;

        // Check if a resource is currently selected
        if(tile.GetComponent<Resource>() != null)
		{
            tile = tile.GetComponent<Resource>().tile;
            tile.GetComponent<Tile>().resource.SetActive(false);
		}

        // An improvement can only built on an empty tile
        if(tile.GetComponent<Tile>().improvement != null)
        {
            Debug.Log("There is already an improvement built on this tile.");
            return null;
        }

        // Create a new GameObject based on the type
        GameObject newImprovement = null;
        switch(improvementType)
        {
            case ImprovementType.House:
                newImprovement = Instantiate(housePrefab, houseParent.transform);
                // Update resource values
                int space = newImprovement.GetComponent<House>().space;
                GetComponent<GameManager>().AddHousing(space);
                break;
            case ImprovementType.Farm:
                newImprovement = Instantiate(farmPrefab, farmParent.transform);
                newImprovement.GetComponent<Producer>().produceEvent = new UnityProduceEvent();
                newImprovement.GetComponent<Producer>().produceEvent.AddListener(GetComponent<GameManager>().AddResource);
                // Update resource values
                int foodProduction = newImprovement.GetComponent<Producer>().productionAmount;
                GetComponent<GameManager>().AddProduction(improvementType, foodProduction);
                break;
            case ImprovementType.Mine:
                newImprovement = Instantiate(minePrefab, mineParent.transform);
                newImprovement.GetComponent<Producer>().produceEvent = new UnityProduceEvent();
                newImprovement.GetComponent<Producer>().produceEvent.AddListener(GetComponent<GameManager>().AddResource);
                // Update resource values
                int stoneProduction = newImprovement.GetComponent<Producer>().productionAmount;
                GetComponent<GameManager>().AddProduction(improvementType, stoneProduction);
                break;
        }
        // Set initial data
        newImprovement.name = improvementType.ToString();
        newImprovement.transform.position = tile.transform.position;
        newImprovement.GetComponent<Improvement>().tile = tile;
        newImprovement.GetComponent<Selectable>().unityEvent = GetComponent<TileSelector>().selectEvent;
        tile.GetComponent<Tile>().improvement = newImprovement;

        // Update the current selected object
        GetComponent<TileSelector>().SelectObject(newImprovement);

        return newImprovement;
    }

    /// <summary>
    /// Destroys the current selected object if it is an improvement
    /// </summary>
    public void DestoryImprovement()
	{
        // Get the currently selected object
        GameObject improvement = GetComponent<TileSelector>().GetSelectedObject();

        // End early if nothing is selected or whatever is selected is not an improvement
        if(improvement == null || improvement.GetComponent<Improvement>() == null)
            return;

        // Get the tile of the improvement, remove the improvement from it, and set it as the new selected object
        GameObject tile = improvement.GetComponent<Improvement>().tile;
        tile.GetComponent<Tile>().improvement = null;
        GetComponent<TileSelector>().SelectObject(tile);

        Destroy(improvement);
	}
}
