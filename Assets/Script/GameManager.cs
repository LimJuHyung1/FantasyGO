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
    static Dictionary<Vector3, int> stonePosList = new Dictionary<Vector3, int>(); // 바둑알 위치 리스트

    int firstX = -11;
    int firstY = 11;

    float fadeSpeed = 2.0f; // 투명도 감소 속도

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

    void InitField()        // 필드 배열 초기화
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

    //-------------------------------------------//

    public void AddStonePosition(Vector3 pos, string tag)   // 좌표별 바둑돌 분류
    {
        if (tag == "Black")
            stonePosList[pos] = 1;
        else if (tag == "White")
            stonePosList[pos] = 2;
    }

    public static bool IsNotDuplicated(Vector3 pos)     // 바둑돌 중첩되어 생성되지 않도록 하는 함수
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

        // 혼자인 바둑돌의 경우
        if (isConnected == false)
        {
            Vector3 upPos = pos + upDis;
            Vector3 downPos = pos + downDis;
            Vector3 rightPos = pos + rightDis;
            Vector3 leftPos = pos + leftDis;

            // 잡히는 상황
            if (ReturnDictValue(upPos) != stone &&
                ReturnDictValue(downPos) != stone &&
                ReturnDictValue(rightPos) != stone &&
                ReturnDictValue(leftPos) != stone)
            {
                return true;
            }
            else return false;
        }
        // 연결되었을 경우 - 수정하기
        else
        {
            return false;
        }
    }

    //-------------------------------------------//

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
