using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public GameObject civilian;
    public GameObject zombieControlBox;
    public GameObject AttackHumanBox;


    private void Start()
    {
        zombieControlBox.SetActive(false);

        // Check if the civilian is null
        if (civilian == null)
        {
            Debug.LogError("TutorialController: civilian is not assigned!");
            return;
        }

    }

    private void Update()
    {
        if (civilian == null)
        {
            zombieControlBox.SetActive(true);

            AttackHumanBox.SetActive(false);
        }
    }
}
