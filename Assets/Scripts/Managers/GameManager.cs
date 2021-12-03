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
    public int totalHousingSpace;
    public int totalPopulation;

    // Start is called before the first frame update
    void Start()
    {
        // Initial calls
        GetComponent<UIManager>().UpdateHouseInfoUI(totalHousingSpace, totalPopulation);
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

    public void UpdateHouseData()
	{
        int totalPop = 0;
        int totalSpace = 0;

        // TODO: Remove hard-coded index
        Transform housesParentTrans = GetComponent<ImprovementManager>().improvementsParent.transform.GetChild(0);
        foreach(Transform houseTrans in housesParentTrans)
		{
            totalPop += houseTrans.GetComponent<House>().occupants;
            totalSpace += houseTrans.GetComponent<House>().space;
		}

        totalPopulation = totalPop;
        totalHousingSpace = totalSpace;

        // Update UI
        GetComponent<UIManager>().UpdateHouseInfoUI(totalPopulation, totalHousingSpace);
	}
}
