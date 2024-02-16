using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Stone : MonoBehaviourPunCallbacks
{
    GameObject gameManager;
    [SerializeField] GameObject[] aroundStones = new GameObject[4];   // �����¿� �ٵϵ� �ν�
    Vector3[] aroundPos = new Vector3[4];
    // [SerializeField] string[] aroundStonesTags = new string[4];

    enum stonePositionType { normal, edge, corner }
    stonePositionType thisPositionType;

    bool isConnected = false;
    bool canBeDead = false; // ����� ���� + ����� �� �̿ܿ� ��� ������ �ٸ� ���� ��� true�� ����
    [SerializeField] GameObject[] connectedStones = new GameObject[4];

    string myTag;
    [SerializeField] bool amIDead = false;    


    public PhotonView PV;

    void Awake() => gameManager = GameObject.Find("GameManager");

    void Start()
    {               
        PV.RPC("AddStonePositionRPC", RpcTarget.All);
        PV.RPC("SetStonePositionType", RpcTarget.All);
        myTag = this.gameObject.tag;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            PV.RPC("UpdateAroundRPC", RpcTarget.All);
            
            if(myTag == "Black")
                PV.RPC("AmIDead", RpcTarget.AllBuffered, "White");
            else if (myTag == "White")
                PV.RPC("AmIDead", RpcTarget.AllBuffered, "Black");

            if (amIDead == true)
                PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }


    [PunRPC]
    void AmIDead(string otherTag)  // �ڽ��� ���� ������ �ƴ��� Ȯ��
    {
        // ���� ������ ������� ���� ����
        if (!isConnected)
        {
            int count = 0;
            for (int i = 0; i < aroundStones.Length; i++)
            {
                if (aroundStones[i] != null && aroundStones[i].tag == otherTag) count++;
            }

            switch (thisPositionType)
            {
                case stonePositionType.normal:
                    amIDead = count == 4 ? true : false;
                    break;

                case stonePositionType.edge:
                    amIDead = count == 3 ? true : false;
                    break;

                case stonePositionType.corner:
                    amIDead = count == 2 ? true : false;
                    break;
            }
        }
        // ���� ������ ����� ����
        else
        {
            // �ڽ��� canBeDead ���·� �Ǵ��� Ȯ��


            // ���� ���� ������ canBeDead ������ ��� ���� ����


        }
    }
    [PunRPC]
    void SetStonePositionType()
    {
        if (transform.position.x == -9 || transform.position.x == 9 ||
            transform.position.y == -9 || transform.position.y == 9)
        {
            float xPos = transform.position.x;
            float yPos = transform.position.y;

            Debug.Log(xPos.ToString() + yPos.ToString());

            if(xPos % 9 == 0 && yPos % 9 == 0)
                thisPositionType = stonePositionType.corner;
            else if (xPos % 9 != 0 || yPos % 9 != 0)
                thisPositionType = stonePositionType.edge;
        }
        else thisPositionType = stonePositionType.normal;
    }

    //----------------------------------------------------------------//

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
        }

        for(int i = 0; i < aroundStones.Length; i++)
        {
            if (aroundStones[i] != null)
            {
                // �ڽ� ������ �ڽŰ� ���� ���� �ִٸ� isConnected true�� ����
                if (aroundStones[i].tag == this.gameObject.tag)
                {
                    isConnected = true;
                    break;
                }
                else
                    isConnected = false;
            }            
        }
    }

    void GetSpecialStone()  // GameManager�� viewIDStoneType ��ųʸ����� �÷��̾��� viewID�� ���� �÷��̾ ������
    {
        Dictionary<int, string> tmp = gameManager.GetComponent<GameManager>().viewIDStoneType;
        PhotonView myPlayer;

        foreach (var pair in tmp)
        {
            if (pair.Value == this.tag)
            {
                myPlayer = PhotonView.Find(pair.Key);
                myPlayer.GetComponent<Player>().specialStone++;
                Debug.Log(myPlayer.GetComponent<Player>().specialStone);
                break;
            }
        }        
    }

    [PunRPC]
    void DestroyRPC()
    {
        GetSpecialStone();
        gameManager.GetComponent<GameManager>().SubStonePosition(transform.position);
        Destroy(gameObject);
        // this.gameObject.SetActive(false);
    }
}
