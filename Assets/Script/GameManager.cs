using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static bool isMasterTurn;
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
        if (isMyTurnStarted())
        {
            FadeIn();
        }

        else if (isMyTurnEnded())
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

    bool isMyTurnStarted()
    {
        return PhotonNetwork.IsMasterClient && GameManager.isMasterTurn
            || !PhotonNetwork.IsMasterClient && !GameManager.isMasterTurn ? true : false;
    }
    
    bool isMyTurnEnded()
    {
        return PhotonNetwork.IsMasterClient && !GameManager.isMasterTurn
            || !PhotonNetwork.IsMasterClient && GameManager.isMasterTurn ? true : false;
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
