using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Stone : MonoBehaviourPunCallbacks
{
    GameObject gameManager;
    [SerializeField] GameObject[] aroundStones = new GameObject[4];   // 상하좌우 바둑돌 인식
    [SerializeField] string[] aroundStonesTags = new string[4];
    string myTag;
    bool amIDead = false;

    public PhotonView PV;

    void Start()
    {
        gameManager = GameObject.Find("GameManager");

        gameManager.GetComponent<GameManager>().AddStonePosition(this.transform.position, this.gameObject);

        myTag = this.gameObject.tag;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            // 주변 바둑돌 참조
            aroundStones = gameManager.GetComponent<GameManager>().ReturnAroundStones(this.transform.position);            

            // 주변 바둑돌 태그 반환받음
            aroundStonesTags = gameManager.GetComponent<GameManager>().ReturnAroundStonesTags(this.transform.position);
        }
    }    

    bool AmIDead()  // 자신이 죽은 돌인지 아닌지 확인
    {
        for(int i = 0; i < aroundStonesTags.Length; i++)
        {
            if (aroundStonesTags[i] != null)
            {
                if (aroundStonesTags[i] != myTag)
                {
                    amIDead = true;
                }
                else amIDead = false; break;
            }
            else amIDead = false; break;
        }
        return amIDead;
    }

    [PunRPC]
    void DestroyRPC()
    {
        this.gameObject.SetActive(false);
    }
}
