using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    mainMenu,
    mapLoad,
    game,
    pause
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
    }

    public GameState currentGameState;
    public Dictionary<ResourceType, (int, int)> resources;

	// Start is called before the first frame update
	void Start()
    {
        resources = new Dictionary<ResourceType, (int, int)>();

        // Initial calls
        FillResourceDictionary();
        UIManager.instance.UpdateResourcesUI();
        ChangeGameState(currentGameState);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// A helper method to fill the resource dictionary for each resource
    /// </summary>
    private void FillResourceDictionary()
	{
        resources.Add(ResourceType.People, (0, 0));
        resources.Add(ResourceType.Food, (0, 0));
        resources.Add(ResourceType.Wood, (0, 0));
        resources.Add(ResourceType.Stone, (0, 0));
        resources.Add(ResourceType.Metal, (0, 0));
    }

    /// <summary>
    /// Changes the current game state
    /// </summary>
    /// <param name="newGameState">The state the game will be changed to</param>
    public void ChangeGameState(GameState newGameState)
	{
        // Update UI
        UIManager.instance.UpdateGameStateUI(newGameState);
        currentGameState = newGameState;

        // Perform initial, one-time logic when the gameState changes
        switch(newGameState)
        {
            case GameState.mainMenu:
                break;
            case GameState.mapLoad:
                // TODO: Remove this when saving/deleting/loading maps has been fixed
                // bug with deleting a map that is not the last saved map
                WorldGenerator.instance.CreateNewRandomMap();
                break;
            case GameState.game:
                // Reset the selected object
                TileSelector.instance.DeselectObject();
                break;
            case GameState.pause:
                break;
        }
	}

    /// <summary>
    /// Adds resource(s) to the player 
    /// </summary>
    /// <param name="improvementType">The resource type</param>
    /// <param name="amount">The amount of the resource being produced</param>
    public void UpdateResourceAmount(ImprovementType improvementType, int amount)
	{
        UpdateResourceData(improvementType, amount, 0);
    }

    /// <summary>
    /// Adds resource production to the player
    /// </summary>
    /// <param name="improvementType">The production type</param>
    /// <param name="amount">The amount of production being added</param>
    public void UpdateProduction(ImprovementType improvementType, int amount)
    {
        UpdateResourceData(improvementType, 0, amount);
    }

    /// <summary>
    /// Adds housing space to the player's stats
    /// </summary>
    /// <param name="amount">The amount of new housing space that has been created</param>
    public void UpdateHousing(int amount)
	{
        UpdateResourceData(ImprovementType.House, 0, amount);
	}

    /// <summary>
    /// An all-in-one method that updates the player's resource and/or resource production values
    /// </summary>
    /// <param name="improvementType">The type of resource</param>
    /// <param name="amountOfResource">The amount of the resource being added</param>
    /// <param name="amountOfProduction">The amount of production being added</param>
    private void UpdateResourceData(ImprovementType improvementType, int amountOfResource, int amountOfProduction)
	{
        ResourceType resourceType = ImprovementManager.instance.ImprovementToResource(improvementType);
        (int, int) resourceStats = resources[resourceType];
        resourceStats.Item1 += amountOfResource;
        resourceStats.Item2 += amountOfProduction;
        resources[resourceType] = resourceStats;

        // Update UI
        UIManager.instance.UpdateResourcesUI();
    }
}
