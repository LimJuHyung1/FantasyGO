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
    static List<Vector3> stonePosList = new List<Vector3>(); // 바둑알 위치 리스트

    float fadeSpeed = 2.0f; // 투명도 감소 속도

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

    bool IsMyTurnStarted()  // 내 차례 이미지 보이게 할지 않할지
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
        // 리스트에서 요소를 찾습니다.
        int index = stonePosList.IndexOf(pos);

        if (index != -1)
        {
            Console.WriteLine($"찾는 요소가 리스트에서 인덱스 {index}에 있습니다.");
            return false;
        }
        else
        {
            Console.WriteLine("찾는 요소가 리스트에 존재하지 않습니다.");
            return true;
        }        
    }

    void FadeIn()
    {
        turnImage.gameObject.SetActive(true);

        // 현재 색깔을 가져옵니다.
        Color currentColor = turnImage.color;

        // 현재 알파값에 페이드 속도를 곱한 만큼 증가시킵니다.
        float newAlpha = currentColor.a + fadeSpeed * Time.deltaTime;

        // 새로운 알파값이 1을 초과하지 않도록 보정합니다.
        newAlpha = Mathf.Min(newAlpha, 1f);

        // 변경된 알파값을 적용하여 이미지를 나타냅니다.
        turnImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
    }

    void FadeOut()
    {
        Color currentColor = turnImage.color;
        float newAlpha = currentColor.a - fadeSpeed * Time.deltaTime;

        // 투명도가 0 이하로 가지 않도록 보정
        newAlpha = Mathf.Max(newAlpha, 0f);

        // Color 값을 변경하여 투명도를 조절
        turnImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);

        if (newAlpha <= 0) turnImage.gameObject.SetActive(false);
    }
}
