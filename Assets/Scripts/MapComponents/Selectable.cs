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
}
