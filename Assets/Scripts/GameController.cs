using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // MAKE IT SO IF PLAYER OBJECT IS DESTROYED THEN SET GAME OVER SCREEN
    public GameObject playerController;

    public MenuController menuController;

    public GameObject[] humanObjects;

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        humanObjects = GameObject.FindGameObjectsWithTag("Human");

        Debug.Log(humanObjects.Length);

        if (humanObjects.Length == 0)
        {
            menuController.YouWin();
        }
    }
}
