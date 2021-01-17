using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using UnityEngine.UI;
public class MyPhoton : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [SerializeField]
    public List<RoomInfo> listOfRooms;

    public GameStateScript script;
    public Text joinRoomName, createRoomName;
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        Debug.Log("can now access lobbies");
    }
    // Update is called once per frame
    public override void OnJoinedRoom()
    {
        GameObject newPlayer = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        GameStateScript.player = newPlayer;
        GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().Follow = newPlayer.transform;
        script.joinLobby();
    }
    void Update()
    {

    }
    public void joinRoom()
    {
        if (joinRoomName.text != "")
        {
            if (createRoomName.text != "")
            {
                PhotonNetwork.LocalPlayer.NickName = createRoomName.text;
            }
            else
            {
                PhotonNetwork.LocalPlayer.NickName = "DefaultName";
            }
            PhotonNetwork.JoinRoom(joinRoomName.text);
        }
    }
    public void createRoom()
    {
        if (createRoomName.text != "")
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 10;
            PhotonNetwork.CreateRoom(createRoomName.text, options, null);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            listOfRooms = new List<RoomInfo>();
            Debug.Log(room.Name);
            if (!room.RemovedFromList)
            {
                listOfRooms.Add(room);
            }
            else Debug.Log(room + " was removed from the list.");
        }
        if (roomList.Count == 0) Debug.Log("no rooms");
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log(targetPlayer.NickName + " " + changedProps["isDead"]);
    }
}
