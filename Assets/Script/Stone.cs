using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    GameObject gameManager;
    GameObject[] teamStone;

    void Start()
    {
        gameManager = GameObject.Find("GameManager");

        gameManager.GetComponent<GameManager>().AddStonePosition(this.transform.position, this.tag.ToString());
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Vector3 thisPos = this.transform.position;
            GameManager gameManagerScript = gameManager.GetComponent<GameManager>();

            // 주위의 바둑돌이 전부 다른 색깔의 돌인 경우
            if(gameManagerScript.AmIDead(thisPos, false, gameManagerScript.ReturnDictValue(thisPos)))
            {
                this.gameObject.SetActive(gameObject);
            }
        }

    }
}
