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
    public GameState currentGameState;

    // Resources
    public (int, int) population;
    public (int, int) food;
    public (int, int) stone;

    // Start is called before the first frame update
    void Start()
    {
        population = (0, 0);
        food = (0, 0);
        stone = (0, 0);

        // Initial calls
        GetComponent<UIManager>().UpdateResourcesUI(this);
        ChangeGameState(currentGameState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Changes the current game state
    /// </summary>
    /// <param name="newGameState">The state the game will be changed to</param>
    public void ChangeGameState(GameState newGameState)
	{
        // Update UI
        GetComponent<UIManager>().UpdateGameStateUI(newGameState);

        // Perform initial, one-time logic when the gameState changes
        switch(newGameState)
        {
            case GameState.mainMenu:
                break;
            case GameState.mapLoad:
                break;
            case GameState.game:
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
    public void AddResource(ImprovementType improvementType, int amount)
	{
        UpdateResourceData(improvementType, amount, 0);
    }

    /// <summary>
    /// Adds resource production to the player
    /// </summary>
    /// <param name="improvementType">The production type</param>
    /// <param name="amount">The amount of production being added</param>
    public void AddProduction(ImprovementType improvementType, int amount)
    {
        UpdateResourceData(improvementType, 0, amount);
    }

    /// <summary>
    /// Adds housing space to the player's stats
    /// </summary>
    /// <param name="amount">The amount of new housing space that has been created</param>
    public void AddHousing(int amount)
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
        (int, int) resourceTuple = (0, 0);
        switch(improvementType)
        {
            case ImprovementType.House:
                resourceTuple = population;
                break;
            case ImprovementType.Farm:
                resourceTuple = food;
                break;
            case ImprovementType.Mine:
                resourceTuple = stone;
                break;
        }

        resourceTuple.Item1 += amountOfResource;
        resourceTuple.Item2 += amountOfProduction;

        // Update UI
        GetComponent<UIManager>().UpdateResourcesUI(this);
    }
}
