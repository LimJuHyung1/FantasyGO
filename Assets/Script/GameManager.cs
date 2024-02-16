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
    public Dictionary<Vector3, GameObject> positionObject = new Dictionary<Vector3, GameObject>(); // 바둑알 위치 리스트
    public Dictionary<int, string> viewIDStoneType = new Dictionary<int, string>();
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

    public void AddStonePosition(Vector3 pos, GameObject stone)   // 좌표별 바둑돌 분류
    {
        positionObject[pos] = stone;
    }

    public void SubStonePosition(Vector3 pos)
    {
        positionObject[pos] = null;
    }

    public bool IsNotDuplicated(Vector3 pos)     // 바둑돌 중첩되어 생성되지 않도록 하는 함수
    {
        GameObject stone;

        // 화면을 터치한 위치가 바둑판 바깥을 벗어날 경우 true 반환 - 바둑돌 생성되지 않도록 함
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
