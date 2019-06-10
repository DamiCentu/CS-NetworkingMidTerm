using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ServerNetwork : MonoBehaviourPun
{

    public static ServerNetwork Instance { get; private set; }
    PhotonView _view;
    public Dictionary<Player, PlayerInstance> players = new Dictionary<Player, PlayerInstance>();
    public Player serverReference;
    public int amountOfPlayersToStart = 2;
    public int secondsToStart = 3;

    void Awake()
    {
        _view = GetComponent<PhotonView>();

        if (!Instance)
        {
            if (_view.IsMine)
                _view.RPC("SetReferenceToSelf", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
        }
        else
            PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void AddPlayer(Player p)
    {
        if (!_view.IsMine)
            return;

        Debug.Log("Player Added");

        var spawnPoints = FindObjectsOfType<SpawnPoint>().Where(x => !x.Taken).ToArray();

        var spawnPoint = spawnPoints.Length > 0 ? spawnPoints[ Random.Range(0, spawnPoints.Length)] : null;

        if(spawnPoint)
            spawnPoint.SetTaken();

        var newPlayer = PhotonNetwork.Instantiate("PlayerInstance",
                        spawnPoint.transform.position,
                        Quaternion.identity).GetComponent<PlayerInstance>();

        newPlayer.SetActive(false);

        players.Add(p, newPlayer);

        if(players.Count > amountOfPlayersToStart - 1)
        {
            StartCoroutine(StartGame(FindObjectsOfType<TextBehaviour>().Where(x => x.id == "startText").First()));
        }
    }

    IEnumerator StartGame(TextBehaviour textToUpdate)
    {
        textToUpdate.SetActive(true);
        for (int i = secondsToStart; i > 0; i--)
        {
            textToUpdate.UpdateText("Starting game in " + i);
            yield return new WaitForSeconds(1f);
        }

        textToUpdate.SetActive(false);

        foreach (var player in players)
        {
            players[player.Key].SetActive(true);
        }
    }


    //player

    [PunRPC]
    public void SetReferenceToSelf(Player p)
    {
        Instance = this;
        serverReference = p;
        if (!PhotonNetwork.IsMasterClient)
            _view.RPC("AddPlayer", serverReference, PhotonNetwork.LocalPlayer);
    }
   
    [PunRPC]
    void RequestShoot(Player player)
    {
        if (!_view.IsMine)
            return;

        if (players.ContainsKey(player))
            players[player].InstantiateBullet();
    }

    [PunRPC]
    void RequestAccelerate(Player player)
    {
        if (!_view.IsMine)
            return;

        if (players.ContainsKey(player))
            players[player].Accelerate();
    }

    [PunRPC]
    void RequestRotate(Player player, float axis)
    {
        if (!_view.IsMine)
            return;
        if (players.ContainsKey(player))
            players[player].RotatePlayer(axis);
    }

    [PunRPC]
    void RequestAdjustVelocity(Player player, Vector3 velocity)
    {
        if (!_view.IsMine)
            return;

        if (players.ContainsKey(player))
            players[player].AdjustVelocity(velocity);
    }

    public void PlayerRequestAdjustVelocity(Player player, Vector3 velocity)
    {
        _view.RPC("RequestAdjustVelocity", serverReference, player, velocity);
    }

    public void PlayerRequestShoot(Player player)
    {
        _view.RPC("RequestShoot", serverReference, player);
    }

    public void PlayerRequestAccelerate(Player player)
    {
        _view.RPC("RequestAccelerate", serverReference, player);
    }

    public void PlayeRequestRotate(Player player, float axis)
    {
        _view.RPC("RequestRotate", serverReference, player, axis);
    }
}
