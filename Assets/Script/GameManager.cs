using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static bool isMasterTurn;
    static Dictionary<Vector3, int> stonePosList = new Dictionary<Vector3, int>(); // �ٵϾ� ��ġ ����Ʈ

    int firstX = -11;
    int firstY = 11;

    float fadeSpeed = 2.0f; // ���� ���� �ӵ�

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
                stonePosList.Add(new Vector3(firstX, firstY, -1), 0);
                Debug.Log(new Vector3(firstX, firstY, -1));
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

    public void AddStonePosition(Vector3 pos, string tag)   // ��ǥ�� �ٵϵ� �з�
    {
        if (tag == "Black")
            stonePosList[pos] = 1;
        else if (tag == "White")
            stonePosList[pos] = 2;
    }

    public static bool IsNotDuplicated(Vector3 pos)     // �ٵϵ� ��ø�Ǿ� �������� �ʵ��� �ϴ� �Լ�
    {
        int value = stonePosList[pos];
        if (value == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int ReturnDictValue(Vector3 pos)
    {
        if (stonePosList.ContainsKey(pos))
        {
            return stonePosList[pos];
        }
        else return -1;
    }

    public bool AmIDead(Vector3 pos, bool isConnected, int stone)
    {
        Vector3 upDis = new Vector3(0, 2, 0);
        Vector3 downDis = new Vector3(0, -2, 0);

        Vector3 rightDis = new Vector3(2, 0, 0);
        Vector3 leftDis = new Vector3(-2, 0, 0);

        // ȥ���� �ٵϵ��� ���
        if (isConnected == false)
        {
            Vector3 upPos = pos + upDis;
            Vector3 downPos = pos + downDis;
            Vector3 rightPos = pos + rightDis;
            Vector3 leftPos = pos + leftDis;

            // ������ ��Ȳ
            if (ReturnDictValue(upPos) != stone &&
                ReturnDictValue(downPos) != stone &&
                ReturnDictValue(rightPos) != stone &&
                ReturnDictValue(leftPos) != stone)
            {
                return true;
            }
            else return false;
        }
        // ����Ǿ��� ��� - �����ϱ�
        else
        {
            return false;
        }
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
