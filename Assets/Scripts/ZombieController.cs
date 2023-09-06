using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    public int Health;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float detectionRadius;
    [SerializeField] private GameObject targetPrefab;
    public GameObject playerController;
    public GameObject clickTarget;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] soundClips;
    [SerializeField] private float minDelayBetweenSounds = 3f;
    [SerializeField] private float maxDelayBetweenSounds = 6f;

    private NavMeshAgent agent;
    private GameObject targetClone;

    public bool attacking;
    private float nextSoundTime;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();


        attacking = false;

        playerController = GameObject.FindGameObjectWithTag("Player");
        clickTarget = GameObject.FindGameObjectWithTag("ZombieTarget");

        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;

        // Create a target clone object at the zombie's location
        targetClone = Instantiate(targetPrefab, transform.position, Quaternion.identity);
        targetClone.name = gameObject.name + " Target";
        targetClone.SetActive(true);
    }

    private void Update()
    {
        AreYouDeadYet();

        agent.SetDestination(targetClone.transform.position);

        OnPlayerClick();

        if (Time.time >= nextSoundTime)
        {
            PlayRandomSound();
            nextSoundTime = Time.time + Random.Range(minDelayBetweenSounds, maxDelayBetweenSounds);
        }

        if (!attacking)
        {
            // Check if a human object is within 20 units and no obstacles between them
            GameObject humanObject = LocateHumanObject();
            if (humanObject != null)
            {
                // Found a human object, set the position of targetClone to be the same as the human object's position
                targetClone.transform.position = humanObject.transform.position;
                Debug.Log("Found a human object: " + humanObject.name);
                attacking = true;
            }
        }
        else
        {
            
        }
    }

    public void OnPlayerClick()
    {
        if (playerController.GetComponent<PlayerController>().clicked)
        {
            targetClone.transform.position = clickTarget.transform.position;
            attacking = false;
        }
    }

    void AreYouDeadYet()
    {
        if (Health <= 0)
        {
            OnDestroy();
            Destroy(gameObject);
        }
    }

    public void ReduceHealth()
    {
        if (Health > 0)
        {
            Health--;

            Debug.Log(gameObject.name + "health = " + Health);
        }
    }

    private void OnDestroy()
    {
        // Destroy the target clone when the zombie is destroyed
        Destroy(targetClone);
    }

    private GameObject LocateHumanObject()
    {
        // Get the position of the zombie
        Vector3 zombiePosition = transform.position;

        // Get all the game objects with the "Human" tag
        GameObject[] humanObjects = GameObject.FindGameObjectsWithTag("Human");

        // Iterate through the human objects and check if they are within 20 units and not interrupted by a raycast
        foreach (GameObject humanObject in humanObjects)
        {
            Vector3 humanPosition = humanObject.transform.position;

            // Check if the human object is within 20 units of the zombie
            float distance = Vector3.Distance(zombiePosition, humanPosition);
            if (distance <= 20f)
            {
                // Check if there is a clear line of sight between the zombie and the human object
                RaycastHit hit;
                Vector3 direction = humanPosition - zombiePosition;

                if (Physics.Raycast(zombiePosition, direction, out hit, distance))
                {
                    // If there is an obstacle between the zombie and the human object, continue to the next human object
                    continue;
                }

                // If there is no obstacle, return the human object
                return humanObject;
            }
        }

        // If no human object is found, return null
        return null;
    }

    private void PlayRandomSound()
    {
        if (soundClips.Length == 0 || audioSource == null)
            return;

        if (soundClips.Length > 0)
        {
            int randomIndex = Random.Range(0, soundClips.Length);
            audioSource.clip = soundClips[randomIndex];
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No sound clips found in the soundClips array.");
        }
    }

}
