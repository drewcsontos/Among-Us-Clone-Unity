using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class Movement : MonoBehaviour, IPunObservable
{
    public bool isImposter;
    Vector2 networkPosition;
    Rigidbody2D rb;
    public float speed = 1;
    PhotonView photonView;
    float scaleX;
    GameObject light2d;
    GameStateScript script;
    bool hasStartedGame = false;
    Animator animator;
    public GameObject[] players;
    public bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        isImposter = false;
        players = GameObject.FindGameObjectsWithTag("Player");
        script = FindObjectOfType<GameStateScript>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        scaleX = transform.localScale.x;
        photonView = GetComponent<PhotonView>();
        light2d = gameObject.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            Destroy(light2d);
            return;
        }
        if (canMove == false)
        {
            return;
        }
        Vector2 velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * speed;
        rb.velocity = velocity;
        if (velocity.Equals(Vector2.zero))
        {
            animator.SetBool("walking", false);
        }
        else
        {
            animator.SetBool("walking", true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && PhotonNetwork.IsMasterClient && hasStartedGame == false)
        {
            hasStartedGame = true;
            photonView.RPC("callStartGame", RpcTarget.AllBufferedViaServer);
            int enemyPlayer = Random.Range(0, PhotonNetwork.PlayerList.Length);
            photonView.RPC("setImposter", PhotonNetwork.PlayerList[enemyPlayer]);
        }
        if (Input.GetKeyDown(KeyCode.E) && isImposter && hasStartedGame == true && players.Length != 0)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            float i = float.MaxValue;
            GameObject playerToKill = null;
            foreach (GameObject player in players)
            {
                if (Vector2.Distance(gameObject.transform.position, player.transform.position) < i && !player.Equals(gameObject))
                {
                    playerToKill = player;
                    i = Vector3.Distance(gameObject.transform.position, player.transform.position);
                }
            }
            Debug.Log(i);
            Debug.Log("tried to kill");
            if (i < 6)
            {

                Debug.Log("killed");
                playerToKill.GetComponent<Animator>().SetBool("dead", true);
                var photonOfKilledPlayer = playerToKill.GetComponent<PhotonView>();
                if (photonOfKilledPlayer != null)
                    photonView.RPC("killed", photonOfKilledPlayer.Owner);
                gameObject.transform.position = playerToKill.gameObject.transform.position;
                PhotonNetwork.Instantiate("Blood", gameObject.transform.position, Quaternion.identity);
            }


        }

        if (velocity.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = scaleX * (velocity.x >= 0 ? 1 : -1);
            transform.localScale = scale;
        }
    }

    [PunRPC]
    void callStartGame()
    {
        hasStartedGame = true;
        script = FindObjectOfType<GameStateScript>();
        script.gameState = "gameStarted";
        script.startGame();
    }
    [PunRPC]
    void setImposter()
    {
        Hashtable hash = new Hashtable();
        hash.Add("isImposter", true);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        if (photonView.IsMine)
        {
            isImposter = true;
        }
        var testPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in testPlayers)
        {
            if (player.GetPhotonView().IsMine)
            {
                player.GetComponent<Movement>().isImposter = true;
            }
        }
        Debug.Log("You are the imposter.");
    }
    [PunRPC]
    void killed()
    {
        if (!photonView.IsMine)
            Debug.Log("called killed");
        var testPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in testPlayers)
        {
            if (player.GetPhotonView().IsMine)
            {
                player.GetComponent<Movement>().canMove = false;
                Hashtable hash = new Hashtable();
                hash.Add("isDead", true);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                player.GetComponent<Animator>().SetBool("dead", true);
            }
        }

    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.localScale);
        }
        else
        {
            transform.localScale = (Vector3)stream.ReceiveNext();
        }
    }
}
