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
        // If the camera parent exists, the player can move it
        if (cameraParent != null)
		{
            // Sprinting
            int currentMoveSpeed = moveSpeed;
            if(Input.GetAxis("Sprint") > 0)
                currentMoveSpeed *= 2;

            // Vertical and horizontal movement
            float horizMove = Input.GetAxis("Horizontal") * Time.deltaTime * currentMoveSpeed;
            float vertMove = Input.GetAxis("Vertical") * Time.deltaTime * currentMoveSpeed;
            cameraParent.transform.Translate(-horizMove, 0, -vertMove);
        }
    }

    /// <summary>
    /// A helper method that translates the camera parent
    /// </summary>
    /// <param name="newPos">A new position that will be added to the camera's position</param>
    public void MoveCamera(Vector3 newPos)
	{
        cameraParent.transform.position += newPos;
    }

    /// <summary>
    /// A helper method that zeros out the camera parent's position
    /// </summary>
    public void ResetCamera()
    {
        cameraParent.transform.position = new Vector3();
    }
}
