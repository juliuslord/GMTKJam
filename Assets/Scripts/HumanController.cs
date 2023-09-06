using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanController : MonoBehaviour
{
    public GameObject humanTargetPrefab;
    private GameObject currentTarget;
    public GameObject targetObject;
    private NavMeshAgent navMeshAgent;
    private enum State { Patrolling, Attacking, Strolling }
    private State currentState;

    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    public LayerMask SeeingLayers;
    public float shootInterval; // Editable interval
    public bool shooting;
    private AudioSource audioSource;

    [SerializeField] private GameObject zombiePrefab;

    private void Start()
    {

        audioSource = GetComponent<AudioSource>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (patrolPoints.Length > 0)
        {
            currentState = State.Patrolling;
            SetNextPatrolDestination();
        }
        else
        {
            currentState = State.Strolling;
            SetNewDestination();
        }
    }

    private void Update()
    {
        Debug.Log("Current State: " + currentState.ToString());

        if (targetObject != null)
        {
            Debug.Log("Current shooting target: " + targetObject.name);
        }

        switch (currentState)
        {
            case State.Patrolling:
                if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
                {
                    SetNextPatrolDestination();
                }

                shooting = false;
                                                break;

            case State.Attacking:
                if (targetObject != null)
                {
                    // Face the target's X-Z position
                    Vector3 targetPosition = targetObject.transform.position;
                    targetPosition.y = transform.position.y; // Set target's Y position to be same as the human's Y position
                    transform.LookAt(targetPosition);
                }

                // Remain stationary while attacking
                navMeshAgent.isStopped = true;

                if (!shooting)
                {
                    // Run ShootThem function at an interval
                    StartCoroutine(ShootThem(shootInterval, targetObject));
                }

                if (targetObject == null)
                {
                    shooting = false;
                    navMeshAgent.isStopped = false;
                    currentState = State.Patrolling;
                    SetNextPatrolDestination();
                }
                                                break;

            case State.Strolling:
                if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
                {
                    SetNewDestination();
                }

                shooting = false;

                                                break;
        }
    }

    private IEnumerator ShootThem(float interval, GameObject targetObject)
    {
        while (targetObject != null)
        {
            // Check line of sight before each attack
            RaycastHit hit;
            if (Physics.Raycast(transform.position, (targetObject.transform.position - transform.position).normalized, out hit, Mathf.Infinity, SeeingLayers))
            {
                if (hit.collider.gameObject == targetObject)
                {
                    // Play attack noise
                    audioSource.Play();

                    // Retrieve the target object's script and call ReduceHealth function
                    if (targetObject.CompareTag("Zombie"))
                    {
                        ZombieController zombieController = targetObject.GetComponent<ZombieController>();
                        if (zombieController != null)
                        {
                            zombieController.ReduceHealth();
                        }
                    }
                    else if (targetObject.CompareTag("Player"))
                    {
                        PlayerController playerController = targetObject.GetComponent<PlayerController>();
                        if (playerController != null)
                        {
                            playerController.ReduceHealth();
                        }
                    }
                }
                else
                {
                    // Break line of sight with the target, stop shooting
                    targetObject = null;
                    break;
                }
            }

            shooting = true;

            // Wait for the interval
            yield return new WaitForSeconds(interval);
        }

        currentState = State.Patrolling;
        SetNextPatrolDestination();
    }


    private void SetNewDestination()
    {
        if (currentTarget != null)
        {
            Destroy(currentTarget);
        }

        Vector3 destination = GetRandomPosition();
        currentTarget = Instantiate(humanTargetPrefab, destination, Quaternion.identity);
        navMeshAgent.SetDestination(destination);
        currentState = State.Strolling;
    }

    private void SetNextPatrolDestination()
    {
        if (currentTarget != null)
        {
            Destroy(currentTarget);
        }

        Transform nextPatrolPoint = patrolPoints[currentPatrolIndex];
        currentTarget = Instantiate(humanTargetPrefab, nextPatrolPoint.position, Quaternion.identity);
        navMeshAgent.SetDestination(nextPatrolPoint.position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        currentState = State.Patrolling;
    }

    private Vector3 GetRandomPosition()
    {
        // Generate a random position within a certain range
        float x = Random.Range(-10f, 10f);
        float z = Random.Range(-10f, 10f);
        return new Vector3(x, 0f, z);
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentState != State.Attacking && (other.CompareTag("Zombie") || other.CompareTag("Player")))
        {
            Debug.Log("Player/zombie seen");

            // Check line of sight using raycast
            RaycastHit hit;
            Vector3 direction = (other.transform.position - transform.position).normalized;
            if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, SeeingLayers))
            {
                Debug.DrawRay(transform.position, direction * hit.distance, Color.red);

                targetObject = hit.collider.gameObject;

                if (targetObject.CompareTag("Zombie") || targetObject.CompareTag("Player"))
                {
                    currentState = State.Attacking;
                }

                else
                {
                    targetObject = null;
                    navMeshAgent.isStopped = false;
                    currentState = State.Patrolling;
                    SetNextPatrolDestination();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentState == State.Attacking && (other.CompareTag("Zombie") || other.CompareTag("Player")))
        {
            targetObject = null;
            navMeshAgent.isStopped = false;
            currentState = State.Patrolling;
            SetNextPatrolDestination();
        }
    }

    public void KilledByZombie()
    {
        // Spawn a zombie object at the current position
        Instantiate(zombiePrefab, transform.position, Quaternion.identity);

        // Destroy the human character
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Zombie") || collision.gameObject.CompareTag("Player"))
        {
            KilledByZombie();
        }
    }

}
