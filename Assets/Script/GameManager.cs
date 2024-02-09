using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    int order = 0;
    bool isMyTurn = true;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.Euler(90f, 0f, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        // ��ġ �Է� ����
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // ù ��° ��ġ �Է¸� ó�� (���� ��ġ ����)

            // ��ġ�� ���۵Ǿ��� ��
            if (touch.phase == TouchPhase.Began)
            {
                // ��ġ ��ġ Ȯ��
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                float CeilX = Mathf.Ceil(touchPosition.x);
                float FloorX = Mathf.Floor(touchPosition.x);
                float CeilY = Mathf.Ceil(touchPosition.y);
                float FloorY = Mathf.Floor(touchPosition.y);

                touchPosition.x = CeilX % 2 == 0 ? FloorX : CeilX;
                touchPosition.y = CeilY % 2 == 0 ? FloorY : CeilY;
                touchPosition.z = -1;
                //if(isMyTurn)
                //    PhotonNetwork.Instantiate("Black", touchPosition, Quaternion.Euler(90f, 0f, 0f));
                //else
                //    PhotonNetwork.Instantiate("White", touchPosition, Quaternion.Euler(90f, 0f, 0f));

                PhotonNetwork.Instantiate("Black", touchPosition, Quaternion.Euler(90f, 0f, 0f));

                isMyTurn = !isMyTurn;
            }
        }
    }
}
