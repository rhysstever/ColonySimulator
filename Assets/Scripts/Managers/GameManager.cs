using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int totalHousingSpace;
    public int totalPopulation;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<UIManager>().UpdateHouseInfoUI(totalHousingSpace, totalPopulation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHouseData()
	{
        int totalPop = 0;
        int totalSpace = 0;

        // TODO: Remove hard-coded index
        Transform housesParentTrans = GetComponent<ImprovementManager>().improvementsParent.transform.GetChild(0);
        foreach(Transform houseTrans in housesParentTrans)
		{
            totalPop += houseTrans.GetComponent<House>().occupants;
            totalSpace += houseTrans.GetComponent<House>().space;
		}

        totalPopulation = totalPop;
        totalHousingSpace = totalSpace;

        // Update UI
        GetComponent<UIManager>().UpdateHouseInfoUI(totalPopulation, totalHousingSpace);
	}
}
