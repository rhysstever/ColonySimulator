using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
    }

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
    private GameObject selectedGameObjectPanel, buildMenuPanel;
    [SerializeField]    // Empty gameObject parents
    private GameObject buildImprovementsButtonsParent;
    [SerializeField]    // Buttons
    private GameObject pauseButton, buildMenuButton;
    [SerializeField]    // Build Buttons
    private GameObject cancelBuildButton, buildHouseButton, buildFarmButton, buildMineButton, destroyButton;
    [SerializeField]    // Resource Text
    public GameObject populationText, foodText, woodText, stoneText;
    [SerializeField]    // Text
    private GameObject selectedObjectNameText, selectedObjectDescText;

    // Pause
    [SerializeField]    // Buttons
    private GameObject continueButton, loadButton, saveButton, backToMenuButton;

    private int selectedMapIndex;

    // Start is called before the first frame update
    void Start()
    {
        selectedMapIndex = -1;

        SetupUI();
        UpdateSelectedObjectUI();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        randomMapLoadButton.GetComponent<Button>().onClick.AddListener(() => WorldGenerator.instance.CreateNewRandomMap());
        mapDeleteButton.GetComponent<Button>().onClick.AddListener(() => WorldGenerator.instance.DeleteMap(selectedMapIndex));

        // Game - button events
        pauseButton.GetComponent<Button>().onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.pause));
        buildMenuButton.GetComponent<Button>().onClick.AddListener(() => UpdateBuildMenu(true));
        cancelBuildButton.GetComponent<Button>().onClick.AddListener(() => UpdateBuildMenu(false));
        buildHouseButton.GetComponent<Button>().onClick.AddListener(() => BuildImprovementButtonClicked(ImprovementType.House));
        buildFarmButton.GetComponent<Button>().onClick.AddListener(() => BuildImprovementButtonClicked(ImprovementType.Farm));
        buildMineButton.GetComponent<Button>().onClick.AddListener(() => BuildImprovementButtonClicked(ImprovementType.Mine));
        destroyButton.GetComponent<Button>().onClick.AddListener(() => ImprovementManager.instance.DestoryImprovement());

        // Pause - button events
        continueButton.GetComponent<Button>().onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.game));
        loadButton.GetComponent<Button>().onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.mapLoad));
        saveButton.GetComponent<Button>().onClick.AddListener(() => WorldGenerator.instance.SaveWorld());
        backToMenuButton.GetComponent<Button>().onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.mainMenu));
    }

    /// <summary>
    /// Updates overall UI based on the current gameState
    /// </summary>
    /// <param name="gameState">The current gameState</param>
    public void UpdateGameStateUI(GameState gameState)
	{
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
                buildMenuPanel.SetActive(false);
                break;
            case GameState.pause:
                pauseParent.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// Updates UI when a new gameObject is selected
    /// </summary>
    public void UpdateSelectedObjectUI()
	{
        GameObject selectedGameObject = TileSelector.instance.GetSelectedObject();

        // Deactivate all selected object UI
        foreach(Transform childTrans in selectedGameObjectPanel.transform)
            childTrans.gameObject.SetActive(false);

        // Update text
        if(selectedGameObject != null)
        {
            selectedGameObjectPanel.SetActive(true);
            selectedObjectNameText.SetActive(true);
            selectedObjectNameText.GetComponent<Text>().text = selectedGameObject.name;
            selectedObjectDescText.SetActive(true);
            selectedObjectDescText.GetComponent<Text>().text = selectedGameObject.GetComponent<Selectable>().GetDescription();
        }
		else
		{
            selectedGameObjectPanel.SetActive(false);
            return;
        }

        // Activate the right elements based on the selected object
        if(selectedGameObject.GetComponent<Improvement>() != null)
            destroyButton.SetActive(true);
        else 
            buildMenuButton.SetActive(true);
    }

    /// <summary>
    /// Updates the build menu and activates the correct children elements based on what is selected
    /// </summary>
    /// <param name="isActive">Whether the build menu should be active or not</param>
    private void UpdateBuildMenu(bool isActive)
	{
        buildMenuPanel.SetActive(isActive);
		if(isActive)
		{
            GameObject selectedGameObject = TileSelector.instance.GetSelectedObject();
            buildImprovementsButtonsParent.SetActive(true);
		}
	}

    /// <summary>
    /// An all-catching method for when any build improvement button is clicked
    /// </summary>
    /// <param name="improvementType">The improvement type trying to be built</param>
    private void BuildImprovementButtonClicked(ImprovementType improvementType)
	{
        UpdateBuildMenu(false);
        ImprovementManager.instance.BuildImprovement(improvementType);
	}

    /// <summary>
    /// Updates the UI text displaying all of the player's resources
    /// </summary>
	public void UpdateResourcesUI()
	{
        foreach(ResourceType resource in GameManager.instance.resources.Keys)
            UpdateResouceText(resource);
    }

    /// <summary>
    /// A helper method to update a single resource's text
    /// </summary>
    /// <param name="resource">The resource being updated</param>
    private void UpdateResouceText(ResourceType resource)
	{
        string updatedText = resource + ": ";
		if(resource == ResourceType.Population)
            updatedText += GameManager.instance.resources[resource].CurrentAmount
                + "/" + GameManager.instance.resources[resource].SecondaryAmount;
        else
            updatedText += GameManager.instance.resources[resource].CurrentAmount
                + " [" + GameManager.instance.resources[resource].SecondaryAmount + "]";

		if(GameManager.instance.resources[resource].Text != null)
            GameManager.instance.resources[resource].Text.GetComponent<Text>().text = updatedText;
    }

    /// <summary>
    /// Creates a button for each map that can be loaded
    /// </summary>
    public void CreateMapLoadButtons()
    {
        ClearMapLoadButtons();

        // Define hard-coded values
        Vector3 startingPos = new Vector3(-135.0f, 20.0f, 0.0f);
        float deltaX = 90.0f;
        float deltaY = -40.0f;

        // Loop through the number of saved maps
        foreach(int mapNum in WorldGenerator.instance.savedMapsNums)
        {
            GameObject mapLoadButton = Instantiate(mapLoadButtonPrefab, mapLoadButtonsParent.transform);
            // Change the name and text of the button
            mapLoadButton.name = "loadMap" + mapNum + "Button";
            mapLoadButton.transform.GetChild(0).GetComponent<Text>().text = "Map " + mapNum;

            // Calculate and update the button's position
            int count = mapLoadButtonsParent.transform.childCount;
            Vector3 position = startingPos;
            position.x += ((count - 1) % 4) * deltaX;
            position.y += ((count - 1) / 4) * deltaY;
            mapLoadButton.transform.localPosition = position;
            // Add an onClick to load a specific map
            int temp = mapNum;   // this is needed or the number passed in will be (# of maps + 1)
            mapLoadButton.GetComponent<Button>().onClick.AddListener(() => SelectMap(temp));
        }
    }

    /// <summary>
    /// Clears the map load button children
    /// </summary>
    private void ClearMapLoadButtons()
    {
        List<GameObject> mapButtonsToBeDestroyed = new List<GameObject>();

        // Delete any current children of the parent
        foreach (Transform childTrans in mapLoadButtonsParent.transform)
            mapButtonsToBeDestroyed.Add(childTrans.gameObject);

        // Destroy() is too slow (the new buttons are being created immediately after
        for(int i = mapButtonsToBeDestroyed.Count - 1; i >= 0; i--)
            DestroyImmediate(mapButtonsToBeDestroyed[i]);    
    }

    /// <summary>
    /// Sets the selected map index and changes the onclick to load world
    /// </summary>
    /// <param name="mapNum">The number of the newly selected map</param>
    private void SelectMap(int mapNum)
	{
        if (mapNum == -1)
            return;
        selectedMapIndex = mapNum;
        mapLoadSelectedButton.GetComponent<Button>().onClick.AddListener(() => WorldGenerator.instance.LoadWorld(selectedMapIndex));
    }
}
