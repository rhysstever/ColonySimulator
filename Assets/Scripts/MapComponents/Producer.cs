using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityProduceEvent : UnityEvent<ImprovementType, int>
{
    // An empty class that is used for events that hold parameterized methods
}

public class Producer : MonoBehaviour
{
    public int productionAmount;
    public float rate;
    public UnityProduceEvent produceEvent;

    private float timer;
    private ImprovementType improvementType;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
        improvementType = GetComponent<Improvement>().type;
    }

    // Update is called once per frame
    void Update()
    {

    }

	void FixedUpdate()
    {
        timer += Time.deltaTime;
        if(timer >= rate)
        {
            produceEvent.Invoke(improvementType, productionAmount);
            timer = 0.0f;
        }
    }
}
