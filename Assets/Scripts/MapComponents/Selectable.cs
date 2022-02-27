using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class UnityGameObjectEvent : UnityEvent<GameObject>
{
    // An empty class that is used for events that hold parameterized methods
}

public class Selectable : MonoBehaviour
{
    public UnityGameObjectEvent unityEvent;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnMouseUp()
	{
        if(!EventSystem.current.IsPointerOverGameObject())
		{
            unityEvent.Invoke(gameObject);
        }
	}

    /// <summary>
    /// Gets a description of the selectable
    /// </summary>
    /// <returns>A string of its description</returns>
    public string GetDescription()
	{
        if(GetComponent<Producer>() != null)
		{
            ResourceType resourceType = ImprovementManager.instance.ImprovementToResource(GetComponent<Improvement>().type);
            return "Producing " + GetComponent<Producer>().productionAmount 
                + " " + resourceType.ToString().ToLower() 
                + " / " 
                + GetComponent<Producer>().rate + "s";
		}
        else if(GetComponent<House>() != null)
            return "Housing " + GetComponent<House>().occupants + " / " + GetComponent<House>().space + " people";
        else if(GetComponent<Resource>() != null)
            return "A source of " + GetComponent<Resource>().type.ToString().ToLower();
        else
            return "A tile";
	}
}
