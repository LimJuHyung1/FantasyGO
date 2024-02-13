using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    GameObject gameManager;
    AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        audio = GetComponent<AudioSource>();
    }
    void Update()
    {        
        // 터치 입력 감지
        if (Input.touchCount > 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (GameManager.isMasterTurn)
                {
                    Touch touch = Input.GetTouch(0); // 첫 번째 터치 입력만 처리 (단일 터치 예시)

                    // 터치가 시작되었을 때
                    if (touch.phase == TouchPhase.Began)
                    {
                        Vector3 touchPos = Touch(touch);

                        // 충돌체가 발견되었는지 확인
                        if (gameManager.GetComponent<GameManager>().IsNotDuplicated(touchPos) 
                            && isTouchedInField(touchPos))
                        {
                            PhotonNetwork.Instantiate("Black", touchPos, Quaternion.Euler(90f, 0f, 0f));

                            // 턴 전환
                            gameManager.GetComponent<PhotonView>().RPC("ChangeTurnRPC", RpcTarget.All);
                            PV.RPC("SoundRPC", RpcTarget.All);
                        }
                    }
                }
            }
            else
            {
                if (!GameManager.isMasterTurn)
                {
                    Touch touch = Input.GetTouch(0); // 첫 번째 터치 입력만 처리 (단일 터치 예시)

                    // 터치가 시작되었을 때
                    if (touch.phase == TouchPhase.Began)
                    {
                        Vector3 touchPos = Touch(touch);

                        // 충돌체가 발견되었는지 확인
                        if (gameManager.GetComponent<GameManager>().IsNotDuplicated(touchPos)
                            && isTouchedInField(touchPos))
                        {
                            PhotonNetwork.Instantiate("White", touchPos, Quaternion.Euler(90f, 0f, 0f));

                            // 턴 전환
                            gameManager.GetComponent<PhotonView>().RPC("ChangeTurnRPC", RpcTarget.All);
                            PV.RPC("SoundRPC", RpcTarget.All);
                        }
                    }
                }
            }
        }
    }

    Vector3 Touch(Touch touch)      // 터치한 곳 좌표 반환
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

        float CeilX = Mathf.Ceil(touchPosition.x);
        float FloorX = Mathf.Floor(touchPosition.x);
        float CeilY = Mathf.Ceil(touchPosition.y);
        float FloorY = Mathf.Floor(touchPosition.y);

        touchPosition.x = CeilX % 2 == 0 ? FloorX : CeilX;
        touchPosition.y = CeilY % 2 == 0 ? FloorY : CeilY;
        touchPosition.z = -1;

        return touchPosition;
    }   

    // 필드 안에서 터치가 된 것인지 여부 반환
    bool isTouchedInField(Vector3 touch)
    {
        return touch.x <= 9 && touch.x >= -9 && touch.y <= 9 && touch.y >= -9 ? true : false;
    }

    [PunRPC]
    void SoundRPC()
    {
        audio.Play();
    }
}
