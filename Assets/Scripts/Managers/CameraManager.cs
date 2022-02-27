using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance = null;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
    }

    public GameObject cameraParent;
    private GameObject mainCam;

    // Movement floats
    private float moveSpeed;
    private float rotateSpeed;
    private float zoomSpeed;

    // Bounds floats
    private float camMinHoriz;
    private float camMaxHoriz;
    private float camMinVert;
    private float camMaxVert;

    private float camAngleRad;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = cameraParent.transform.GetChild(0).gameObject;

        moveSpeed = 5.0f;
        rotateSpeed = 40.0f;
        zoomSpeed = 5.0f;

        camMinHoriz = 3.0f;
        camMaxHoriz = WorldGenerator.instance.size - 5.0f;
        camMinVert = -3.0f;
        camMaxVert = 10.0f;

        camAngleRad = Mathf.Atan(6.0f / 8.0f);
    }

    // Update is called once per frame
    void Update()
    {
        camMaxHoriz = WorldGenerator.instance.size - 5.0f;

    }

	void FixedUpdate()
	{
        // 2 conditions for the camera to be able to move:
        // 1) If the camera parent exists
        // 2) The player is in the game, gameState
        if (cameraParent != null && GameManager.instance.currentGameState == GameState.game)
		{
            MoveCameraWithInput();
            // RotateCameraWithInput(); // bugged with zoom clamping
            ZoomCameraWithInput();
            CheckBounds();
        }
    }

    /// <summary>
    /// A helper method that translates the camera parent
    /// </summary>
    /// <param name="posToAdd">A new position that will be added to the camera's position</param>
    public void MoveCamera(Vector3 posToAdd)
	{
        cameraParent.transform.Translate(posToAdd);
    }

    /// <summary>
    /// Calculates horizontal movement from inputs, including sprinting
    /// </summary>
    public void MoveCameraWithInput()
	{
        // Sprinting
        float currentMoveSpeed = moveSpeed;
        if(Input.GetAxis("Sprint") > 0)
            currentMoveSpeed *= 2;

        // Vertical and horizontal movement
        float horizMove = Input.GetAxis("Horizontal") * Time.deltaTime * currentMoveSpeed;
        float vertMove = Input.GetAxis("Vertical") * Time.deltaTime * currentMoveSpeed;
        MoveCamera(new Vector3(-horizMove, 0, -vertMove));
    }

    /// <summary>
    /// A helper method that zeros out the camera parent's position
    /// </summary>
    public void ResetCamera()
    {
        cameraParent.transform.position = new Vector3();
    }

    /// <summary>
    /// Rotates the cameraParent from input
    /// </summary>
    private void RotateCameraWithInput()
	{
        float rotateAmount = Input.GetAxis("Rotate") * Time.deltaTime * rotateSpeed;
        cameraParent.transform.Rotate(0, rotateAmount, 0);
    }

    /// <summary>
    /// "Zooms" the camera in and out from input
    /// </summary>
    private void ZoomCameraWithInput()
	{
        float zoomAmount = Input.GetAxis("Zoom") * Time.deltaTime * zoomSpeed;
        Vector3 zoom = mainCam.transform.forward; 
        zoom.Normalize();
        zoom *= zoomAmount;
        mainCam.transform.Translate(zoom, Space.World);
    }

    /// <summary>
    /// Checks the bounds for each axis of the camera parent
    /// </summary>
    private void CheckBounds()
	{
        // Clamp camera parent panning
        cameraParent.transform.position = new Vector3(
            Mathf.Clamp(cameraParent.transform.position.x, camMinHoriz, camMaxHoriz),
            Mathf.Clamp(cameraParent.transform.position.y, camMinVert, camMaxVert),
            Mathf.Clamp(cameraParent.transform.position.z, camMinHoriz, camMaxHoriz));

		// Clamp camera zooming
		float currentDistance = Vector3.Distance(mainCam.transform.position, cameraParent.transform.position);
		float distToParent = Mathf.Clamp(
			currentDistance,
			5.0f,
			20.0f);
        mainCam.transform.position = new Vector3(
            mainCam.transform.position.x,
			distToParent * Mathf.Sin(camAngleRad),
			cameraParent.transform.position.z +
				distToParent * Mathf.Cos(camAngleRad));
	}
}
