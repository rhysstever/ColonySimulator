using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2 coordinates;
    public TileType tileType;
    public GameObject improvement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void ChangeMaterial(Material newMat)
	{
        transform.GetChild(0).GetComponent<MeshRenderer>().material = newMat;
    }

	public void Select(bool isSelected)
	{
		transform.GetChild(1).gameObject.SetActive(isSelected);
	}
}
