using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImprovementDesc
{
    // Fields
    private ImprovementType improvementType;
    private GameObject prefab;
    private GameObject parentObj;
    private bool isProducer;
    private bool isOnResource;
    private float prodAmount;
    private Dictionary<ResourceType, int> resourceCosts;

    // Properties
    public ImprovementType Type { get { return improvementType; } }
    public GameObject Prefab { get { return prefab; } }
    public GameObject ParentObj { get { return parentObj; } }
    public bool IsProducer { get { return isProducer;} }
    public bool HasResource { get { return isOnResource; } }
    public float ProdAmount { get { return prodAmount; } }
    public Dictionary<ResourceType, int> ResourceCosts { get { return resourceCosts; } }

    // Constructor
    public ImprovementDesc(ImprovementType improvementType, GameObject prefab, GameObject parentObj, bool isProducer, bool isOnResource)
    {
        this.improvementType = improvementType;
        this.prefab = prefab;
        this.parentObj = parentObj;
        this.isProducer = isProducer;
        this.isOnResource = isOnResource;
        prodAmount = isProducer ? prefab.GetComponent<Producer>().productionAmount : 0;
        resourceCosts = new Dictionary<ResourceType, int>();
    }

    // Methods
    public void AddResourceCost(ResourceType resource, int amount)
	{
        resourceCosts.Add(resource, amount);
	}
}
