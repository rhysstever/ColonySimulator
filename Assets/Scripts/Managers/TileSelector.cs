using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileSelector : MonoBehaviour
{
    public GameObject currentSelectedGameObj;

    public UnityGameObjectEvent selectEvent = new UnityGameObjectEvent();

    // Start is called before the first frame update
    void Start()
    {
        selectEvent.AddListener(SelectObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            SelectObject(null);
	}

    public void SelectObject(GameObject selectedObject)
	{
        // If the previously selected object was a tile, deselect it
        if(currentSelectedGameObj != null
            && currentSelectedGameObj.GetComponent<Tile>() != null)
            currentSelectedGameObj.GetComponent<Tile>().Select(false);

        currentSelectedGameObj = selectedObject;

        // If the selected object is a tile
        if(currentSelectedGameObj != null
            && currentSelectedGameObj.GetComponent<Tile>() != null)
		{
            // If the selected object is a tile with an improvement, then select the improvement
            if(currentSelectedGameObj.GetComponent<Tile>().improvement != null)
                currentSelectedGameObj = currentSelectedGameObj.GetComponent<Tile>().improvement;
            // If there is no improvement, select the tile
            else
                currentSelectedGameObj.GetComponent<Tile>().Select(true);
        }

        // Update UI
        GetComponent<UIManager>().UpdateSelectedObjectUI(currentSelectedGameObj);
    }
}
