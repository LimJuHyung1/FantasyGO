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
        // ��ġ �Է� ����
        if (Input.touchCount > 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (GameManager.isMasterTurn)
                {
                    Touch touch = Input.GetTouch(0); // ù ��° ��ġ �Է¸� ó�� (���� ��ġ ����)

                    // ��ġ�� ���۵Ǿ��� ��
                    if (touch.phase == TouchPhase.Began)
                    {
                        Vector3 touchPos = Touch(touch);

                        // �浹ü�� �߰ߵǾ����� Ȯ��
                        if (gameManager.GetComponent<GameManager>().IsNotDuplicated(touchPos) 
                            && isTouchedInField(touchPos))
                        {
                            PhotonNetwork.Instantiate("Black", touchPos, Quaternion.Euler(90f, 0f, 0f));

                            // �� ��ȯ
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
                    Touch touch = Input.GetTouch(0); // ù ��° ��ġ �Է¸� ó�� (���� ��ġ ����)

                    // ��ġ�� ���۵Ǿ��� ��
                    if (touch.phase == TouchPhase.Began)
                    {
                        Vector3 touchPos = Touch(touch);

                        // �浹ü�� �߰ߵǾ����� Ȯ��
                        if (gameManager.GetComponent<GameManager>().IsNotDuplicated(touchPos)
                            && isTouchedInField(touchPos))
                        {
                            PhotonNetwork.Instantiate("White", touchPos, Quaternion.Euler(90f, 0f, 0f));

                            // �� ��ȯ
                            gameManager.GetComponent<PhotonView>().RPC("ChangeTurnRPC", RpcTarget.All);
                            PV.RPC("SoundRPC", RpcTarget.All);
                        }
                    }
                }
            }
        }
    }

    Vector3 Touch(Touch touch)      // ��ġ�� �� ��ǥ ��ȯ
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

    // �ʵ� �ȿ��� ��ġ�� �� ������ ���� ��ȯ
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
