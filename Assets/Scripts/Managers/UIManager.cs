using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;
    // Empty GameObject parents
    [SerializeField]
    private GameObject mainMenuParent, mapLoadParent, gameParent, pauseParent;

    // Main Menu
    [SerializeField]
    private GameObject playButton;
    [SerializeField]
    private GameObject quitButton;

    // Map Load
    [SerializeField]    // Panels
    private GameObject mapLoadPanel;
    [SerializeField]    // Empty gameObject parents
    private GameObject mapLoadButtonsParent;
    [SerializeField]    // Buttons
    private GameObject mapLoadButtonPrefab, mapLoadSelectedButton, randomMapLoadButton, mapDeleteButton;

    // Game
    [SerializeField]    // Panels
    private GameObject selectedGameObjectPanel;
    [SerializeField]    // Empty gameObject parents
    private GameObject tileButtonsParent, improvementInfoParent, resourceInfoParent;
    [SerializeField]    // Buttons
    private GameObject pauseButton, buildHouseButton, buildFarmButton, buildMineButton, destroyButton;
    [SerializeField]    // Text
    private GameObject populationText, foodText, stoneText, selectedObjectNameText;

    // Pause
    [SerializeField]    // Buttons
    private GameObject continueButton, loadButton, saveButton, backToMenuButton;

    int selectedMapIndex;

    // Start is called before the first frame update
    void Start()
    {
        selectedMapIndex = -1;

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
        // Main Menu - button events
        playButton.GetComponent<Button>().onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.mapLoad));
        quitButton.GetComponent<Button>().onClick.AddListener(() => Application.Quit());

        // Map Load - button events
        SelectMap(1);
        randomMapLoadButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<WorldGenerator>().CreateNewRandomMap());
        mapDeleteButton.GetComponent<Button>().onClick.AddListener(() => DeleteMap());

        // Game - button events
        pauseButton.GetComponent<Button>().onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.pause));
        buildHouseButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<ImprovementManager>().BuildHouse());
        buildFarmButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<ImprovementManager>().BuildFarm());
        buildMineButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<ImprovementManager>().BuildMine());
        destroyButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<ImprovementManager>().DestoryImprovement());

        // Pause - button events
        continueButton.GetComponent<Button>().onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.game));
        loadButton.GetComponent<Button>().onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.mapLoad));
        saveButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<WorldGenerator>().SaveWorld());
        backToMenuButton.GetComponent<Button>().onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.mainMenu));
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
        // Check for improvement, then resource, then tile
        if(selectedGameObject.GetComponent<Improvement>() != null)
            improvementInfoParent.SetActive(true);
        else if(selectedGameObject.GetComponent<Resource>() != null)
		{
            // Set the parent to be active but deactive its children
            resourceInfoParent.SetActive(true);
            foreach(Transform childTrans in resourceInfoParent.transform)
                childTrans.gameObject.SetActive(false);

            switch(selectedGameObject.GetComponent<Resource>().tile.GetComponent<Tile>().tileType)
            {
                case TileType.Mountains:
                    buildMineButton.SetActive(true);
                    break;
            }
		}
        else if(selectedGameObject.GetComponent<Tile>() != null
            && selectedGameObject.GetComponent<Tile>().tileType != TileType.Ocean)
            tileButtonsParent.SetActive(true);
    }

    /// <summary>
    /// Updates the UI text displaying all of the player's resources
    /// </summary>
	public void UpdateResourcesUI()
	{
        populationText.GetComponent<Text>().text = 
            "Population: " + GameManager.instance.resources[ResourceType.People].Item1 
            + "/" + GameManager.instance.resources[ResourceType.People].Item2;
        foodText.GetComponent<Text>().text = 
            "Food: " + GameManager.instance.resources[ResourceType.Food].Item1 
            + " [" + GameManager.instance.resources[ResourceType.Food].Item2 + "]";
        stoneText.GetComponent<Text>().text = 
            "Stone: " + GameManager.instance.resources[ResourceType.Stone].Item1 
            + " [" + GameManager.instance.resources[ResourceType.Stone].Item2 + "]";
    }

    /// <summary>
    /// Creates a button for each map that can be loaded
    /// </summary>
    private void CreateMapLoadButtons()
    {
        // Delete any current children of the parent
        foreach(Transform childTrans in mapLoadButtonsParent.transform)
            Destroy(childTrans.gameObject);

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
            mapLoadButton.GetComponent<Button>().onClick.AddListener(() => SelectMap(temp));
        }
    }

    /// <summary>
    /// Sets the selected map index and changes the onclick to load world
    /// </summary>
    /// <param name="index"></param>
    private void SelectMap(int index)
	{
        selectedMapIndex = index;
        mapLoadSelectedButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<WorldGenerator>().LoadWorld(selectedMapIndex));
    }

    /// <summary>
    /// Call delete map methods and update the buttons
    /// </summary>
    private void DeleteMap()
	{
        GetComponent<WorldGenerator>().DeleteMap(selectedMapIndex);
        CreateMapLoadButtons();
    }
}
