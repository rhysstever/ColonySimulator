using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileSelector : MonoBehaviour
{
    private GameObject currentSelectedGameObj;

    public UnityGameObjectEvent selectEvent = new UnityGameObjectEvent();

    // Start is called before the first frame update
    void Start()
    {
        selectEvent.AddListener(SelectObject);
    }

    // Update is called once per frame
    void Update()
    {
        // ESC - deselects the current object
        if(Input.GetKeyDown(KeyCode.Escape))
            DeselectObject();
	}

    /// <summary>
    /// Selects a new object 
    /// </summary>
    /// <param name="selectedObject">The new object to be selected</param>
    public void SelectObject(GameObject selectedObject)
	{
        // The player can only select an object unless it is the Game state
        if(GameManager.instance.currentGameState != GameState.game)
            return;

        // If the previously selected object was a tile, deselect it
        if(currentSelectedGameObj != null
            && currentSelectedGameObj.GetComponent<Tile>() != null)
            currentSelectedGameObj.GetComponent<Tile>().Select(false);

        currentSelectedGameObj = selectedObject;

        // If the selected object is a tile
        if(currentSelectedGameObj != null
            && currentSelectedGameObj.GetComponent<Tile>() != null)
		{
            // Selection heirarchy: Improvement > Resource > Tile
            if(currentSelectedGameObj.GetComponent<Tile>().improvement != null)
                currentSelectedGameObj = currentSelectedGameObj.GetComponent<Tile>().improvement;
            else if(currentSelectedGameObj.GetComponent<Tile>().resource != null)
                currentSelectedGameObj = currentSelectedGameObj.GetComponent<Tile>().resource;
            else
                currentSelectedGameObj.GetComponent<Tile>().Select(true);
        }

        // Update UI
        GetComponent<UIManager>().UpdateSelectedObjectUI(currentSelectedGameObj);
    }

    /// <summary>
    /// Deselects the selected object
    /// </summary>
    public void DeselectObject()
	{
        SelectObject(null);
	}

    /// <summary>
    /// Gets the object that is currently selected
    /// </summary>
    /// <returns>The currently selected object (could be null)</returns>
    public GameObject GetSelectedObject()
	{
        return currentSelectedGameObj;
	}
}
