using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDesc
{
    #region Fields
    private ResourceType type;
    private GameObject text;
    private int currentAmount;
    private int secondaryAmount;
    #endregion

    #region Properties
    public ResourceType Type { get { return type; } }
    public GameObject Text { get { return text; } }
    public int CurrentAmount { get { return currentAmount; } }
    public int SecondaryAmount { get { return secondaryAmount; } }
    #endregion

    #region Constructor
    public ResourceDesc(ResourceType type, GameObject text)
    {
        this.type = type;
        this.text = text;
        currentAmount = 0;
        secondaryAmount = 0;
    }
    public ResourceDesc(ResourceType type, GameObject text, int currentAmount, int secondaryAmount)
    {
        this.type = type;
        this.text = text;
        this.currentAmount = currentAmount;
        this.secondaryAmount = secondaryAmount;
    }
    #endregion

    #region Methods
    public void AddAmount(int amount)
	{
        currentAmount += amount;
	}

    public void AddSecondaryAmount(int amount)
    {
        secondaryAmount += amount;
    }

    public void ResetAmount()
	{
        currentAmount = 0;
        secondaryAmount = 0;
    }
    #endregion
}
