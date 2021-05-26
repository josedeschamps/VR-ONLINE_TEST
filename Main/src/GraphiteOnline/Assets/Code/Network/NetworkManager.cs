using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class NetworkManager : MonoBehaviourPunCallbacks
{

    [Header("HUD Needed Prefab")]
    public GameObject vr_Menu;
    public GameObject vr_Room;

    [Header("Loading bar Prefabs")]
    public GameObject vr_loadingBar;
    public Image vr_silder;
    public Animator sliderBackground;
    public TextMeshProUGUI vr_loadingText;
    public Animator textBackground;
    private bool startLoading = false;
    private float loadingPercent = 0.4f;
    private float loadingTime = 3.0f;


    private string[] serverInformation = new string[]
    {
        "Searching.",
        "Searching..",
        "Searching...",
        "Searching....",
        "Connecting to server",
    };

    [System.Serializable]
    public class DefaultRoom
    {

        [Header("Fill room setting for the network")]
        public string name;
        public int sceneIndex;
        public int maxplayer;
        public bool isVisible = true;
        public bool isOpen = true;

    }

    public List<DefaultRoom> defaultRooms;


    public void ConnectToServer()
    {
        vr_loadingBar.SetActive(true);
        StartCoroutine(FindingServer());
        Debug.Log("Searching for server...");
    }

    IEnumerator FindingServer()
    {
        WaitForSeconds fadeIn = new WaitForSeconds(1f);
        yield return fadeIn;
        textBackground.SetTrigger("FadeIn");
        WaitForSeconds wait = new WaitForSeconds(2f);
        for (int i = 0; i < serverInformation.Length; i++)
        {
            vr_loadingText.text = serverInformation[i];

            yield return wait;

            if (i == 4)
            {

                startLoading = true;
                vr_loadingText.text = null;
                sliderBackground.SetTrigger("FadeIn");

            }


        }

    }
    private void Loading_HUD()
    {
        if (startLoading)
        {
            vr_silder.fillAmount += loadingPercent / loadingTime * Time.deltaTime;
            vr_loadingText.text = vr_silder.fillAmount * 100 + "%";

            if (vr_silder.fillAmount == 1.0f)
            {
                PhotonNetwork.ConnectUsingSettings();
                startLoading = false;
            }
        }
    }
    private void Update()
    {
        Loading_HUD();
    }



    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to the server");
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Joined the lobby");
        vr_loadingBar.SetActive(false);
        vr_Room.SetActive(true);


    }

    public void InitializeRoom(int roomIndex)
    {


        DefaultRoom roomSettings = defaultRooms[roomIndex];

        //Load Scene
        PhotonNetwork.LoadLevel(roomSettings.sceneIndex);

        //Create The Room
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)roomSettings.maxplayer;
        roomOptions.IsVisible = roomSettings.isVisible;
        roomOptions.IsOpen = roomSettings.isOpen;

        //Join Room
        Debug.Log("Joinning selecting room.");
        PhotonNetwork.JoinOrCreateRoom(roomSettings.name, roomOptions, TypedLobby.Default);

    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a Room");
        base.OnJoinedRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer + "has joined the room.");
        base.OnPlayerEnteredRoom(newPlayer);
    }


}
