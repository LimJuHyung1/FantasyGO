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
    public Dictionary<Vector3, GameObject> positionObject = new Dictionary<Vector3, GameObject>(); // �ٵϾ� ��ġ ����Ʈ
    public Dictionary<int, string> viewIDStoneType = new Dictionary<int, string>();
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
        Vector3 tmpVec = new Vector3(firstX, firstY, -1);

        for (int i = 0; i < 12; i++)
        {
            tmpVec.x = -11;
            for(int j = 0; j < 12; j++)
            {
                positionObject.Add(tmpVec, null);
                tmpVec.x += 2;
            }
            tmpVec.y -= 2;
        }
    }

    [PunRPC]
    public void AddPlayerRPC(int viewID, string type)
    {
        if(!viewIDStoneType.ContainsKey(viewID))
            viewIDStoneType.Add(viewID, type);
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
        positionObject[pos] = stone;
    }

    public void SubStonePosition(Vector3 pos)
    {
        positionObject[pos] = null;
    }

    public bool IsNotDuplicated(Vector3 pos)     // �ٵϵ� ��ø�Ǿ� �������� �ʵ��� �ϴ� �Լ�
    {
        GameObject stone;

        // ȭ���� ��ġ�� ��ġ�� �ٵ��� �ٱ��� ��� ��� true ��ȯ - �ٵϵ� �������� �ʵ��� ��
        stone = positionObject.ContainsKey(pos) ? positionObject[pos] : null;

        if (stone == null)
            return true;
        else
            return false;
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
