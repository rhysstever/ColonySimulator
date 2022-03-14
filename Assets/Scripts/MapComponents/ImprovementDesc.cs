using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImprovementDesc
{
	#region Fields
	private ImprovementType improvementType;
    private GameObject prefab;
    private GameObject parentObj;
    private bool isProducer;
    private bool isOnResource;
    private float prodAmount;
    private Dictionary<ResourceType, int> resourceBuildCosts;
	#endregion

	#region Properties
	public ImprovementType Type { get { return improvementType; } }
    public GameObject Prefab { get { return prefab; } }
    public GameObject ParentObj { get { return parentObj; } }
    public bool IsProducer { get { return isProducer;} }
    public bool IsOnResource { get { return isOnResource; } }
    public float ProdAmount { get { return prodAmount; } }
    public Dictionary<ResourceType, int> ResourceBuildCosts { get { return resourceBuildCosts; } }
    #endregion

    #region Constructor
    public ImprovementDesc(ImprovementType improvementType, GameObject prefab, GameObject parentObj)
    {
        this.improvementType = improvementType;
        this.prefab = prefab;
        this.parentObj = parentObj;
        isProducer = false;
        isOnResource = false;
        prodAmount = 0;
        resourceBuildCosts = new Dictionary<ResourceType, int>();
    }

    public ImprovementDesc(ImprovementType improvementType, GameObject prefab, GameObject parentObj, bool isProducer, bool isOnResource)
    {
        this.improvementType = improvementType;
        this.prefab = prefab;
        this.parentObj = parentObj;
        this.isProducer = isProducer;
        this.isOnResource = isOnResource;
        prodAmount = isProducer ? prefab.GetComponent<Producer>().productionAmount : 0;
        resourceBuildCosts = new Dictionary<ResourceType, int>();
    }
	#endregion

	#region Methods
	/// <summary>
	/// Adds a resource cost to the improvement, updating the amount if the resource was already required
	/// </summary>
	/// <param name="resource">The type of resource needed to build this improvement</param>
	/// <param name="amount">The amount of the resource needed</param>
	public void AddResourceCost(ResourceType resource, int amount)
    {
        if(resourceBuildCosts.ContainsKey(resource))
            resourceBuildCosts[resource] = amount;
        else
            resourceBuildCosts.Add(resource, amount);
    }
    #endregion

}
