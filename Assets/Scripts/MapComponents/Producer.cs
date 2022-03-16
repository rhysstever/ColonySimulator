using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Producer : MonoBehaviour
{
    public int productionAmount;
    public float rate;

    private float timer;
    private ImprovementType improvementType;
    private ResourceType resourceType;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
        improvementType = GetComponent<Improvement>().type;
        resourceType = GetComponent<Improvement>().resource;
    }

    // Update is called once per frame
    void Update()
    {

    }

	void FixedUpdate()
    {
        if(GameManager.instance.currentGameState == GameState.game)
        {
            timer += Time.deltaTime;
            if(timer >= rate)
            {
                GameManager.instance.UpdateResourceAmount(resourceType, productionAmount);
                timer = 0.0f;
            }
        }
    }
}
