using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class IntroManager : MonoBehaviourPunCallbacks
{
    RoomOptions roomOption;

    public Image loadingImage;
    public Image screen;
    public GameObject networkManager;

    float fadeSpeed = 0.5f; // ���� ���� �ӵ�
    bool isConnected = false;

    void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
        // networkManager.GetComponent<NetworkManager>().Connect();
    }

    void Update()
    {
        if (PhotonNetwork.IsConnected)
        {
            // FadeOut();

            if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount == 2)
                SceneManager.LoadScene("BattleScene");
        }
    }

    public override void OnConnectedToMaster()
    {
        roomOption = new RoomOptions();
        roomOption.MaxPlayers = 2;
        roomOption.CustomRoomProperties = new Hashtable() { { "������", 0 }, { "�����÷��̾�", 1 } };
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Hashtable CP = PhotonNetwork.CurrentRoom.CustomProperties;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "asdf", "asdf" } });

            Hashtable playerCP = PhotonNetwork.LocalPlayer.CustomProperties;
        }
    }

    // �� �����ϱ�
    public void PlayButton()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinOrCreateRoom("tmp", roomOption, null);

            ActiveLoadingImage();
        }
    }

    public void ActiveLoadingImage()
    {
        loadingImage.gameObject.SetActive(true);
    }

    public void UnactiveLoadingImage()
    {
        loadingImage.gameObject.SetActive(false);
        networkManager.GetComponent<NetworkManager>().Disconnect();
    }

    /*
    void FadeOut()
    {
        Color currentColor = screen.color;
        float newAlpha = currentColor.a - fadeSpeed * Time.deltaTime;

        // ������ 0 ���Ϸ� ���� �ʵ��� ����
        newAlpha = Mathf.Max(newAlpha, 0f);

        // Color ���� �����Ͽ� ������ ����
        screen.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);

        if (newAlpha <= 0) screen.gameObject.SetActive(false);
    }
    */
}
