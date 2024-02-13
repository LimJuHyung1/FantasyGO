using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;
using static UnityEditor.PlayerSettings;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static bool isMasterTurn;
    [SerializeField]Dictionary<Vector3, GameObject> positionObject = new Dictionary<Vector3, GameObject>(); // �ٵϾ� ��ġ ����Ʈ

    int firstX = -11;
    int firstY = 11;

    float fadeSpeed = 2.0f; // ���� ���� �ӵ�

    GameObject[] tmp = new GameObject[4];
    // 0 - up
    // 1 - down
    // 2 - right
    // 3 - left
    Vector3[] aroundPos = new Vector3[4];

    public Image turnImage;
    public PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {        
        InitField();

        PV.RPC("SetMasterTurn", RpcTarget.AllBuffered);

        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }


    void Update()
    {
        if (IsMyTurnStarted())
        {
            FadeIn();
        }

        else if (IsMyTurnEnded())
        {
            FadeOut();
        }
    }

    [PunRPC]
    void SetMasterTurn()
    {
        isMasterTurn = true;
    }

    [PunRPC]
    public void ChangeTurnRPC()
    {
        isMasterTurn = !isMasterTurn;
    }

    void InitField()        // �ʵ� �迭 �ʱ�ȭ
    {
        for(int i = 0; i < 12; i++)
        {
            firstX = -11;
            for(int j = 0; j < 12; j++)
            {
                positionObject.Add(new Vector3(firstX, firstY, -1), null);
                //Debug.Log(new Vector3(firstX, firstY, -1));
                firstX += 2;
            }
            firstY -= 2;
        }
    }

    //-------------------------------------------//

    bool IsMyTurnStarted()  // �� ���� �̹��� ���̰� ���� ������
    {
        return PhotonNetwork.IsMasterClient && GameManager.isMasterTurn
            || !PhotonNetwork.IsMasterClient && !GameManager.isMasterTurn ? true : false;
    }
    
    bool IsMyTurnEnded()
    {
        return PhotonNetwork.IsMasterClient && !GameManager.isMasterTurn
            || !PhotonNetwork.IsMasterClient && GameManager.isMasterTurn ? true : false;
    }

    //-------------------------------------------//

    public void AddStonePosition(Vector3 pos, GameObject stone)   // ��ǥ�� �ٵϵ� �з�
    {
        /*
        if (stone.tag == "Black")
            positionObject[pos] = 1;
        else if (stone.tag == "White")
            positionObject[pos] = 2;
        */
        positionObject[pos] = stone;
    }

    public bool IsNotDuplicated(Vector3 pos)     // �ٵϵ� ��ø�Ǿ� �������� �ʵ��� �ϴ� �Լ�
    {
        GameObject stone = positionObject[pos];
        if (stone == null)
            return true;
        else
            return false;
    }

    void SetAroundPosition(Vector3 pos)
    {
        aroundPos[0] = pos + new Vector3(0, 2, 0);
        aroundPos[1] = pos + new Vector3(0, -2, 0);
        aroundPos[2] = pos + new Vector3(2, 0, 0);
        aroundPos[3] = pos + new Vector3(-2, 0, 0);
    }

    public GameObject[] ReturnAroundStones(Vector3 pos)
    {
        SetAroundPosition(pos);

        for(int i = 0; i < tmp.Length; i++)
        {
            tmp[i] = positionObject[aroundPos[i]];
        }

        return tmp;
    }

    public string[] ReturnAroundStonesTags(Vector3 pos)
    {
        string[] returnStrings = new string[4];

        SetAroundPosition(pos);

        for (int i = 0; i < tmp.Length; i++)
        {
            tmp[i] = positionObject[aroundPos[i]];

            if (tmp[i] != null)
                returnStrings[i] = tmp[i].gameObject.tag;            
        }

        return returnStrings;
    }

    //-------------------------------------------//

    void FadeIn()
    {
        turnImage.gameObject.SetActive(true);

        // ���� ������ �����ɴϴ�.
        Color currentColor = turnImage.color;

        // ���� ���İ��� ���̵� �ӵ��� ���� ��ŭ ������ŵ�ϴ�.
        float newAlpha = currentColor.a + fadeSpeed * Time.deltaTime;

        // ���ο� ���İ��� 1�� �ʰ����� �ʵ��� �����մϴ�.
        newAlpha = Mathf.Min(newAlpha, 1f);

        // ����� ���İ��� �����Ͽ� �̹����� ��Ÿ���ϴ�.
        turnImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
    }

    void FadeOut()
    {
        Color currentColor = turnImage.color;
        float newAlpha = currentColor.a - fadeSpeed * Time.deltaTime;

        // ������ 0 ���Ϸ� ���� �ʵ��� ����
        newAlpha = Mathf.Max(newAlpha, 0f);

        // Color ���� �����Ͽ� ������ ����
        turnImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);

        if (newAlpha <= 0) turnImage.gameObject.SetActive(false);
    }
}
