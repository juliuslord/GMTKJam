using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int Health;

    public GameObject objectToMove; // Object to move towards the click location
    public float movementSpeed = 5f;

    private CharacterController characterController;
    private AudioSource audioSource;
    public AudioClip[] footstepSounds;
    public float footstepInterval = 0.5f;
    private float footstepTimer = 0f;

    public bool clicked;

    private float verticalVelocity = 0f;
    private float gravity = 9.8f;

    public CameraMovement cameraController;

    public float rotationSpeed;

    public MenuController menuController;

    private void Start()
    {
        clicked = false;

        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Move the player character based on input
        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.W))
            moveZ = 1f;
        else if (Input.GetKey(KeyCode.S))
            moveZ = -1f;

        if (Input.GetKey(KeyCode.A))
            moveX = -1f;
        else if (Input.GetKey(KeyCode.D))
            moveX = 1f;

        Vector3 movement = new Vector3(moveX, 0f, moveZ);
        characterController.Move(movement * movementSpeed * Time.deltaTime);

        // Rotate the player character towards the movement direction
        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        // Apply gravity
        if (characterController.isGrounded)
        {
            verticalVelocity = 0f; // Reset vertical velocity if grounded
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        // Apply vertical velocity
        Vector3 verticalMovement = new Vector3(0f, verticalVelocity, 0f);
        characterController.Move(verticalMovement * Time.deltaTime);

        // Play random footstep sound at regular intervals
        if (characterController.isGrounded && movement != Vector3.zero)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                footstepTimer = 0f;
                PlayRandomFootstepSound();
            }
        }

        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            clicked = true;
            // Move object towards the click location
            MoveObjectToClick();
        }

        // Check for mouse button release
        if (Input.GetMouseButtonUp(0))
        {
            clicked = false;
        }

        AreYouDeadYet();
    }

    void PlayRandomFootstepSound()
    {
        if (footstepSounds.Length > 0 && audioSource != null)
        {
            int randomIndex = Random.Range(0, footstepSounds.Length);
            audioSource.PlayOneShot(footstepSounds[randomIndex]);
        }
    }

    void AreYouDeadYet()
    {
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void ReduceHealth()
    {
        if (Health > 0)
        {
            Health--;
            Debug.Log("player health = " + Health);
            menuController.DeleteHeart();
        }
    }

    public void ResetClicked()
    {
        clicked = false;
    }

    private void MoveObjectToClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;
            objectToMove.transform.position = targetPosition;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Event") && other.gameObject.name == "WASDToWalk")
        {
            menuController.AlterTutorialText("Walk with WASD \n Pause with P");
        }

        if (other.CompareTag("Event") && other.gameObject.name == "MouseToLook")
        {
            menuController.AlterTutorialText("Move your mouse to look around \n Q and E zoom the camera");
        }

        if (other.CompareTag("Event") && other.gameObject.name == "AttackHuman")
        {
            menuController.AlterTutorialText("The human above is distracted \n Touch them to make them a zombie");
        }

        if (other.CompareTag("Event") && other.gameObject.name == "ZombieControl")
        {
            menuController.AlterTutorialText("You control other zombies! \n Click on a spot and the zombies will go there \n Zombies can infect humans by touching too");
        }

        if (other.CompareTag("Event") && other.gameObject.name == "ZombieLogic")
        {
            menuController.AlterTutorialText("Zombies take 3 shots before dying \n Keep them covered");
        }

        if (other.CompareTag("Event") && other.gameObject.name == "HumanBehaviour")
        {
            menuController.AlterTutorialText("Humans often move in patterns \n Sneak up on them or use your zombies as a distraction");
        }

        if (other.CompareTag("Event") && other.gameObject.name == "Finale")
        {
            menuController.AlterTutorialText("Humans love to hide together \n With a large horde, a frontal assault might work \n Remember: they're expendable, you're not");
        }

        if (other.CompareTag("Event") && other.gameObject.name == "HumanTown")
        {
            menuController.AlterTutorialText("Humans have infested the town \n I count 8 of them");
        }

    }

    private void OnTriggerExit(Collider other)
    {
        menuController.TurnOffTutorialText();
    }
}
