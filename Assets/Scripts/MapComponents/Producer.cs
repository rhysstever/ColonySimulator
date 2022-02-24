using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Producer : MonoBehaviour
{
    public int productionAmount;
    public float rate;

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
            FindObjectOfType<GameManager>().GetComponent<GameManager>().AddResource(improvementType, productionAmount);
            timer = 0.0f;
		}
    }
}
