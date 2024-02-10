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
    static List<Vector3> stonePosList = new List<Vector3>(); // �ٵϾ� ��ġ ����Ʈ

    float fadeSpeed = 2.0f; // ���� ���� �ӵ�

    public Image turnImage;
    public PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
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

    public void AddStonePosition(Vector3 pos)
    {
        stonePosList.Add(pos);
    }

    public static bool IsNotDuplicated(Vector3 pos)
    {
        // ����Ʈ���� ��Ҹ� ã���ϴ�.
        int index = stonePosList.IndexOf(pos);

        if (index != -1)
        {
            Console.WriteLine($"ã�� ��Ұ� ����Ʈ���� �ε��� {index}�� �ֽ��ϴ�.");
            return false;
        }
        else
        {
            Console.WriteLine("ã�� ��Ұ� ����Ʈ�� �������� �ʽ��ϴ�.");
            return true;
        }        
    }

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
