using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImprovementType
{
    Road,
    House,
    Farm,
    Mine
}

public class ImprovementManager : MonoBehaviour
{
    // Empty parent gameObjects
    public GameObject roadParent, houseParent, farmParent, mineParent;

    // Improvement prefabs
    public GameObject roadPrefab, housePrefab, farmPrefab, minePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Builds a road improvement
    /// </summary>
    public void BuildRoad()
    {
        BuildImprovement(ImprovementType.Road);
    }

    /// <summary>
    /// Builds a house improvement
    /// </summary>
    public void BuildHouse()
	{
        BuildImprovement(ImprovementType.House);
        
        // Update world house info
        GetComponent<GameManager>().UpdateHouseData();
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
    /// Builds a generic improvement of the given type
    /// </summary>
    /// <param name="improvementType">The type of improvement being created</param>
    /// <returns>The newly created improvement gameObject</returns>
    private GameObject BuildImprovement(ImprovementType improvementType)
    {
        // Get the currently selected object
        GameObject tile = GetComponent<TileSelector>().currentSelectedGameObj;

        // End early if nothing is currently selected
        if(tile == null)
            return null;

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
            case ImprovementType.Road:
                newImprovement = Instantiate(roadPrefab, roadParent.transform);
                newImprovement.name = "Road";
                break;
            case ImprovementType.House:
                newImprovement = Instantiate(housePrefab, houseParent.transform);
                newImprovement.name = "House";
                break;
            case ImprovementType.Farm:
                newImprovement = Instantiate(farmPrefab, farmParent.transform);
                newImprovement.name = "Farm";
                break;
            case ImprovementType.Mine:
                newImprovement = Instantiate(minePrefab, mineParent.transform);
                newImprovement.name = "Mine";
                break;
        }
        // Set initial data
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
        GameObject improvement = GetComponent<TileSelector>().currentSelectedGameObj;

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
