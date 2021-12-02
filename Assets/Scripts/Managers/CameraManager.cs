using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject cameraParent;
    public int moveSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

	void FixedUpdate()
	{
        if (cameraParent != null)
		{
            int currentMoveSpeed = moveSpeed;
            if(Input.GetAxis("Sprint") > 0)
                currentMoveSpeed *= 2;

            float horizMove = Input.GetAxis("Horizontal") * Time.deltaTime * currentMoveSpeed;
            float vertMove = Input.GetAxis("Vertical") * Time.deltaTime * currentMoveSpeed;
            cameraParent.transform.Translate(-horizMove, 0, -vertMove);
        }
    }

    public void MoveCamera(Vector3 newPos)
	{
        cameraParent.transform.position += newPos;
    }

    public void ResetCamera()
    {
        cameraParent.transform.position = new Vector3();
    }
}
