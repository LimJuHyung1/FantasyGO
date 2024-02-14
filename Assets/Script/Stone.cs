using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Stone : MonoBehaviourPunCallbacks
{
    GameObject gameManager;
    [SerializeField] GameObject[] aroundStones = new GameObject[4];   // �����¿� �ٵϵ� �ν�
    Vector3[] aroundPos = new Vector3[4];
    [SerializeField] string[] aroundStonesTags = new string[4];


    string myTag;
    [SerializeField] bool amIDead = false;    


    public PhotonView PV;

    void Awake() => gameManager = GameObject.Find("GameManager");

    void Start()
    {               
        PV.RPC("AddStonePositionRPC", RpcTarget.All);

        myTag = this.gameObject.tag;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            PV.RPC("UpdateAroundRPC", RpcTarget.All);

            if(myTag == "Black")
                PV.RPC("AmIDead", RpcTarget.AllBuffered, aroundStonesTags, "White");
            else if (myTag == "White")
                PV.RPC("AmIDead", RpcTarget.AllBuffered, aroundStonesTags, "Black");

            if (amIDead == true)
                PV.RPC("DestroyRPC", RpcTarget.AllBuffered);

            // �ֺ� �ٵϵ� �±� ��ȯ����
            // aroundStonesTags = gameManager.GetComponent<GameManager>().ReturnAroundStonesTags(transform.position);
        }
    }


    [PunRPC]
    void AmIDead(string[] tags, string otherTag)  // �ڽ��� ���� ������ �ƴ��� Ȯ��
    {
        for(int i = 0; i < tags.Length; i++)
        {
            if (aroundStonesTags[i] == myTag || aroundStonesTags[i] == "")
            {
                amIDead = false;
                break;
            }
            else if (aroundStonesTags[i] == otherTag)
                amIDead = true;
        }
    }

    void SetAroundPosition(Vector3 pos)     // �����¿� ���Ͱ� �ʱ�ȭ
    {
        aroundPos[0] = pos + new Vector3(0, 2, 0);
        aroundPos[1] = pos + new Vector3(0, -2, 0);
        aroundPos[2] = pos + new Vector3(2, 0, 0);
        aroundPos[3] = pos + new Vector3(-2, 0, 0);
    }

    [PunRPC]
    void AddStonePositionRPC()
    {
        gameManager.GetComponent<GameManager>().AddStonePosition(transform.position, this.gameObject);
    }

    [PunRPC]
    void UpdateAroundRPC()      // ���� �ٵϵ� ���� ����
    {
        SetAroundPosition(transform.position);
        // �ֺ� �ٵϵ� ����
        // aroundStones = gameManager.GetComponent<GameManager>().ReturnAroundStones(transform.position);            
        for (int i = 0; i < aroundStones.Length; i++)
        {
            aroundStones[i] = gameManager.GetComponent<GameManager>().positionObject[aroundPos[i]];

            if (aroundStones[i] != null)
                aroundStonesTags[i] = aroundStones[i].tag;
        }
    }

    [PunRPC]
    void DestroyRPC()
    {
        gameManager.GetComponent<GameManager>().SubStonePosition(transform.position);
        Destroy(gameObject);
        // this.gameObject.SetActive(false);
    }
}
