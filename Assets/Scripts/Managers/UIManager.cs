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
    private GameObject gameParent, pauseParent;
    // Game panels
    [SerializeField]
    private GameObject selectedGameObjectPanel;
    // Game buttons
    [SerializeField]
    private GameObject buildRoadButton, buildHouseButton, destroyButton;
    // Game text
    [SerializeField]
    private GameObject populationText, selectedObjectNameText;
    // Pause buttons
    [SerializeField]
    private GameObject quitButton;

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

    private void SetupUI()
	{
        buildRoadButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<ImprovementManager>().BuildRoad());
        buildHouseButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<ImprovementManager>().BuildHouse());
        destroyButton.GetComponent<Button>().onClick.AddListener(() => GetComponent<ImprovementManager>().DestoryImprovement());
        quitButton.GetComponent<Button>().onClick.AddListener(() => Application.Quit());
    }

    private void UpdateUI()
	{
        // 'P' key toggles the pause menu
		if(Input.GetKeyDown(KeyCode.P))
            pauseParent.SetActive(!pauseParent.activeSelf);
	}

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
        {
            buildRoadButton.SetActive(true);
            buildHouseButton.SetActive(true);
		}
        else if(selectedGameObject.GetComponent<Improvement>() != null)
            destroyButton.SetActive(true);
    }

	public void UpdateHouseInfoUI(int population, int housingSpace)
	{
        populationText.GetComponent<Text>().text = "Population: " + population + "/" + housingSpace;
    }
}
