using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ServerNetwork : MonoBehaviourPun
{

    public static ServerNetwork Instance { get; private set; }
    PhotonView _view;
    public Dictionary<Player, PlayerInstance> players = new Dictionary<Player, PlayerInstance>();
    public Player serverReference;

    private void Awake()
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
    public void AddPlayer(Player p)
    {
        if (!_view.IsMine)
            return;
        var newPlayer = PhotonNetwork.Instantiate("Player",
                        new Vector3(Random.Range(0, 3),
                        Random.Range(0, 3),
                        Random.Range(0, 3)),
                        Quaternion.identity).GetComponent<PlayerInstance>();
        players.Add(p, newPlayer);
        foreach (var item in players)
        {
            Debug.Log(item);
        }
    }

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
    void RequestRotate(Player p, float axis)
    {
        if (!_view.IsMine)
            return;
        if (players.ContainsKey(p))
            players[p].RotatePlayer(axis);
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
