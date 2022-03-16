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
    public Dictionary<ResourceType, ResourceDesc> resources;

	// Start is called before the first frame update
	void Start()
    {
        FillResourceDescriptions();
        ChangeGameState(currentGameState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// A helper method to fill the resource dictionary for each resource
    /// </summary>
    private void FillResourceDescriptions()
	{
        resources = new Dictionary<ResourceType, ResourceDesc>();
        resources.Add(
            ResourceType.Population, 
            new ResourceDesc(ResourceType.Population, UIManager.instance.populationText));
        resources.Add(
            ResourceType.Food,
            new ResourceDesc(ResourceType.Food, UIManager.instance.foodText));
        resources.Add(
            ResourceType.Wood,
            new ResourceDesc(ResourceType.Wood, UIManager.instance.woodText));
        resources.Add(
            ResourceType.Stone,
            new ResourceDesc(ResourceType.Stone, UIManager.instance.stoneText));
        resources.Add(
            ResourceType.Metal,
            new ResourceDesc(ResourceType.Metal, null));

        UIManager.instance.UpdateResourcesUI();
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
                break;
            case GameState.game:
                // Reset the selected object
                TileSelector.instance.DeselectObject();
                UIManager.instance.UpdateResourcesUI();
                break;
            case GameState.pause:
                break;
        }
	}

    /// <summary>
    /// Adds resource(s) to the player 
    /// </summary>
    /// <param name="resourceType">The resource type</param>
    /// <param name="amount">The amount of the resource being produced</param>
    public void UpdateResourceAmount(ResourceType resourceType, int amount)
	{
        UpdateResourceData(resourceType, amount, 0);
    }

    /// <summary>
    /// Adds resource production to the player
    /// </summary>
    /// <param name="resourceType">The production type</param>
    /// <param name="amount">The amount of production being added</param>
    public void UpdateProduction(ResourceType resourceType, int amount)
    {
        UpdateResourceData(resourceType, 0, amount);
    }

    /// <summary>
    /// An all-in-one method that updates the player's resource and/or resource production values
    /// </summary>
    /// <param name="resourceType">The type of resource</param>
    /// <param name="amountOfResource">The amount of the resource being added</param>
    /// <param name="amountOfProduction">The amount of production being added</param>
    private void UpdateResourceData(ResourceType resourceType, int amountOfResource, int amountOfProduction)
	{
        resources[resourceType].AddAmount(amountOfResource);
        resources[resourceType].AddSecondaryAmount(amountOfProduction);

        // Update UI
        UIManager.instance.UpdateResourcesUI();
    }
}
