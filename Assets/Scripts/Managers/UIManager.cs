using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;
    // Empty GameObject parents
    [SerializeField]
    private GameObject mainMenuParent, mapLoadParent, gameParent, pauseParent;

    // Map Load
    [SerializeField]    // Panels
    private GameObject mapLoadPanel;
    [SerializeField]    // Buttons
    private GameObject mapLoadButtonPrefab, randomMapLoadButton;
    [SerializeField]    // Empty gameObject parents
    private GameObject mapLoadButtonsParent;

    // Game
    [SerializeField]    // Panels
    private GameObject selectedGameObjectPanel;
    [SerializeField]    // Empty gameObject parents
    private GameObject tileButtonsParent, improvementButtonsParent;
    [SerializeField]    // Buttons
    private GameObject pauseButton, buildRoadButton, buildHouseButton, buildFarmButton, buildMineButton, destroyButton;
    [SerializeField]    // Text
    private GameObject populationText, foodText, stoneText, selectedObjectNameText;

    // Pause
    [SerializeField]    // Buttons
    private GameObject continueButton, loadButton, saveButton, quitButton;

    // Start is called before the first frame update
    void Start()
    {
        SetupUI();
        UpdateSelectedObjectUI(null);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    /// <summary>
    /// Runs one-time UI logic
    /// </summary>
    private void SetupUI()
	{
        // Map Load - button events
        randomMapLoadButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<WorldGenerator>().CreateNewRandomWorld());

        // Game - button events
        pauseButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<GameManager>().ChangeGameState(GameState.pause));
        buildRoadButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<ImprovementManager>().BuildRoad());
        buildHouseButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<ImprovementManager>().BuildHouse());
        buildFarmButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<ImprovementManager>().BuildFarm());
        buildMineButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<ImprovementManager>().BuildMine());
        destroyButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<ImprovementManager>().DestoryImprovement());

        // Pause - button events
        continueButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<GameManager>().ChangeGameState(GameState.game));
        loadButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<GameManager>().ChangeGameState(GameState.mapLoad));
        saveButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<WorldGenerator>().SaveWorld());
        quitButton.GetComponent<Button>().onClick.AddListener(() => Application.Quit());
    }

    /// <summary>
    /// Runs continually updating logic
    /// </summary>
    private void UpdateUI()
	{

	}

    /// <summary>
    /// Updates overall UI based on the current gameState
    /// </summary>
    /// <param name="gameState">The current gameState</param>
    public void UpdateGameStateUI(GameState gameState)
	{
		// If the previous gameState was mapLoad, 
		// remove all the mapLoadButtons
		if(mapLoadParent.activeSelf)
            foreach(Transform childTrans in mapLoadButtonsParent.transform)
                Destroy(childTrans.gameObject);

        // Deactivate all empty gameObject parents
        foreach(Transform childTrans in canvas.transform)
            childTrans.gameObject.SetActive(false);

        // Activate the right empty parent gameObject
        switch(gameState)
        {
            case GameState.mainMenu:
                mainMenuParent.SetActive(true);
                break;
            case GameState.mapLoad:
                mapLoadParent.SetActive(true);
                CreateMapLoadButtons();
                break;
            case GameState.game:
                gameParent.SetActive(true);
                break;
            case GameState.pause:
                pauseParent.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// Creates a button for each map that can be loaded
    /// </summary>
    private void CreateMapLoadButtons()
	{
        // Define hard-coded values
        Vector3 startingPos = new Vector3(-135.0f, 20.0f, 0.0f);
        float deltaX = 90.0f;
        float deltaY = -40.0f;

        // Loop through the number of saved maps
        for(int i = 1; i <= GetComponent<WorldGenerator>().savedMapCount; i++)
		{
            GameObject mapLoadButton = Instantiate(mapLoadButtonPrefab, mapLoadButtonsParent.transform);
            // Change the name and text of the button
            mapLoadButton.name = "loadMap" + i + "Button";
            mapLoadButton.transform.GetChild(0).GetComponent<Text>().text = "Map " + i;
            // Calculate and update the button's position
            Vector3 position = startingPos;
            position.x += ((i - 1) % 4) * deltaX;
            position.y += ((i - 1) / 4) * deltaY;
            mapLoadButton.transform.localPosition = position;
            // Add an onClick to load a specific map
            int temp = i;   // this is needed or the number passed in will be (# of maps + 1)
            mapLoadButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<WorldGenerator>().LoadWorld(temp));
		}
	}

    /// <summary>
    /// Updates UI when a new gameObject is selected
    /// </summary>
    /// <param name="selectedGameObject">Teh newly selected gameObject</param>
    public void UpdateSelectedObjectUI(GameObject selectedGameObject)
	{
        // Deactivate all selected object UI
        foreach(Transform childTrans in selectedGameObjectPanel.transform)
            if(!childTrans.gameObject.Equals(selectedObjectNameText))
                childTrans.gameObject.SetActive(false);

        // Update the name text
        if(selectedGameObject == null)
		{
            selectedGameObjectPanel.SetActive(false);
            return;
		}
		else
		{
            selectedGameObjectPanel.SetActive(true);
            selectedObjectNameText.GetComponent<Text>().text = selectedGameObject.name;
		}
        
        // Determine what type of object is selected and display the correct UI
        if(selectedGameObject.GetComponent<Tile>() != null)
            tileButtonsParent.SetActive(true);
        else if(selectedGameObject.GetComponent<Improvement>() != null)
            improvementButtonsParent.SetActive(true);
    }

    /// <summary>
    /// Updates the UI text displaying all of the player's resources
    /// </summary>
    /// <param name="gm">The gameManager object that holds all resource values</param>
	public void UpdateResourcesUI(GameManager gm)
	{
        populationText.GetComponent<Text>().text = "Population: " + gm.population.Item1 + "/" + gm.population.Item2;
        foodText.GetComponent<Text>().text = "Food: " + gm.food.Item1 + " [" + gm.food.Item2 + "]";
        stoneText.GetComponent<Text>().text = "Stone: " + gm.stone.Item1 + " [" + gm.stone.Item2 + "]";
    }
}
