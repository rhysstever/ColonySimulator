using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelector : MonoBehaviour
{
    public GameObject currentSelectedGameObj;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if(Input.GetMouseButtonDown(0))
		{
            // Get the selected object
            currentSelectedGameObj = SelectObject();

            // If the selected object is a tile, show that it is selected
            if(currentSelectedGameObj != null
            && currentSelectedGameObj.GetComponent<Tile>() != null)
                currentSelectedGameObj.GetComponent<Tile>().Select(true);
        }
    }

    /// <summary>
    /// Uses raycasting to select a clicked on object
    /// </summary>
    /// <returns>The hit gameObject</returns>
	public GameObject SelectObject()
	{
        // If the previously selected object was a tile, deselect it
        if(currentSelectedGameObj != null
            && currentSelectedGameObj.GetComponent<Tile>() != null)
            currentSelectedGameObj.GetComponent<Tile>().Select(false);

        // Send out a ray at the mouse's position
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            GameObject hitObject = hit.transform.gameObject;

            // Make sure the selected object is not a part of a larger one
            while(hitObject.transform.parent != null 
                && hitObject.transform.parent.tag != "ParentObj")
            {
                hitObject = hitObject.transform.parent.gameObject;
            }

            // If the hit object is a tile and has an improvement, the improvement is selected
            if(hitObject.GetComponent<Tile>() != null)
                hitObject = hitObject.GetComponent<Tile>().improvement;

            return hitObject;
        }

        // Return null if nothing was hit
        return null;
    }
}
