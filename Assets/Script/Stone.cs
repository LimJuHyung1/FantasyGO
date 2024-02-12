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

            // ������ �ٵϵ��� ���� �ٸ� ������ ���� ���
            if(gameManagerScript.AmIDead(thisPos, false, gameManagerScript.ReturnDictValue(thisPos)))
            {
                this.gameObject.SetActive(gameObject);
            }
        }

    }
}
