using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class GameStateScript : MonoBehaviour, IPunObservable
{
    int tasksCompleted = 0;
    public string gameState = "StartMenu";
    public GameObject startScreen, canvas, lobbyScreen, lobby, mainMap;
    public static GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        lobbyScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(transform.localScale);
        }
        else
        {
            //transform.localScale = (Vector3)stream.ReceiveNext();
        }
    }
    public void toLobbyScreen()
    {

        startScreen.SetActive(false);
        lobbyScreen.SetActive(true);
    }
    public void joinLobby()
    {
        if (gameState != "gameStarted")
        {
            canvas.SetActive(false);
            mainMap.SetActive(false);
            lobby.SetActive(true);
        }
    }

    public void startGame()
    {
        canvas.SetActive(false);
        mainMap.SetActive(true);
        lobby.SetActive(false);
        player.transform.position = Vector3.zero;
    }

}
