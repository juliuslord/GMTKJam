using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float movementSpeed = 5f; // Speed of camera movement
    public float boundary = 25f; // Distance from screen edge to start moving camera

    public float zoomSpeed;

    public GameObject playerController;


    public float minCameraSize; // Minimum camera size
    public float maxCameraSize; // Maximum camera size
    public float maxDistanceFromPlayer;

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse position
        Vector3 mousePosition = Input.mousePosition;

        // Get screen dimensions
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Calculate movement direction
        float moveX = 0f;
        float moveZ = 0f;

        if (mousePosition.x < boundary)
            moveX = -1f;
        else if (mousePosition.x > screenWidth - boundary)
            moveX = 1f;

        if (mousePosition.y < boundary)
            moveZ = -1f;
        else if (mousePosition.y > screenHeight - boundary)
            moveZ = 1f;

        // Apply movement
        Vector3 movement = new Vector3(moveX, 0f, moveZ) * movementSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + movement;
        float distanceFromPlayer = Vector3.Distance(newPosition, playerController.transform.position);
        if (distanceFromPlayer <= maxDistanceFromPlayer)
        {
            transform.Translate(movement, Space.World);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            Camera.main.orthographicSize += zoomSpeed * Time.deltaTime;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minCameraSize, maxCameraSize);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            Camera.main.orthographicSize -= zoomSpeed * Time.deltaTime;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minCameraSize, maxCameraSize);
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (Camera.main != null)
            {
                Vector3 playerPosition = playerController.transform.position;
                Vector3 cameraPosition = Camera.main.transform.position;
                cameraPosition.x = playerPosition.x;
                cameraPosition.y = playerPosition.y + 6f;
                cameraPosition.z = playerPosition.z - 3f;
                Camera.main.transform.position = cameraPosition;
            }
        }
    }
}
