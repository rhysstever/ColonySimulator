using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject cameraParent;
    public int moveSpeed;
    public int rotateSpeed;

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
        // 2 conditions for the camera to be able to move:
        // 1) If the camera parent exists
        // 2) The player is in the game, gameState
        if (cameraParent != null && GetComponent<GameManager>().currentGameState == GameState.game)
		{
            // Sprinting
            int currentMoveSpeed = moveSpeed;
            if(Input.GetAxis("Sprint") > 0)
                currentMoveSpeed *= 2;

            // Vertical and horizontal movement
            float horizMove = Input.GetAxis("Horizontal") * Time.deltaTime * currentMoveSpeed;
            float vertMove = Input.GetAxis("Vertical") * Time.deltaTime * currentMoveSpeed;
            cameraParent.transform.Translate(-horizMove, 0, -vertMove);

            // Rotating 
            float rotateAmt = Input.GetAxis("Rotate") * Time.deltaTime * rotateSpeed;
            cameraParent.transform.Rotate(0, rotateAmt, 0);
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

    public void RotateCamera(Vector3 newRot)
	{
        cameraParent.transform.Rotate(newRot);
	}

    /// <summary>
    /// A helper method that zeros out the camera parent's position
    /// </summary>
    public void ResetCamera()
    {
        cameraParent.transform.position = new Vector3();
    }
}
