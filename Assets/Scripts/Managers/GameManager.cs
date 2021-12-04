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

    // Game stats
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

    public void ChangeGameState(GameState newGameState)
	{
        GetComponent<UIManager>().UpdateGameStateUI(newGameState);

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

    public void AddHousing(int amount)
	{
        AddResource(ImprovementType.House, 0, amount);
	}

    public void AddResource(ImprovementType improvementType, int amountOfResource, int amountOfProduction)
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
