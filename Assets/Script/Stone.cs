using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    GameObject gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager");

        gameManager.GetComponent<GameManager>().AddStonePosition(this.transform.position);
    }
}
