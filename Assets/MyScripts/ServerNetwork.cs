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
    public Player serverReference;
    public int amountOfPlayersToStart = 2;
    public int secondsToStart = 3;


    Dictionary<Player, PlayerInstance> _players = new Dictionary<Player, PlayerInstance>();

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

        _players.Add(p, newPlayer);

        //_view.RPC("RequestSetActive", serverReference, p, false);

        if (_players.Count > amountOfPlayersToStart - 1)
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

        foreach (var player in _players)
        {

//             _view.RPC("SetActiveRPC", RpcTarget.OthersBuffered, active);
            //_view.RPC("RequestSetActive", serverReference, player.Key, true);
            //RequestSetActive(player.Key, true);
            //_players[player.Key].SetActive(true);
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

        if (_players.ContainsKey(player))
            _players[player].InstantiateBullet(player);
    }

    [PunRPC]
    void RequestAccelerate(Player player)
    {
        if (!_view.IsMine)
            return;

        if (_players.ContainsKey(player))
            _players[player].Accelerate();
    }

    [PunRPC]
    void RequestRotate(Player player, float axis)
    {
        if (!_view.IsMine)
            return;
        if (_players.ContainsKey(player))
            _players[player].RotatePlayer(axis);
    }

//     [PunRPC]
//     void RequestSetActive(Player player, bool active)
//     {
//         if (!_view.IsMine)
//             return;
// 
//         if (_players.ContainsKey(player))
//             _players[player].ActiveGameObject(active);
//     }

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




    //bullet

    public void InstantiateBullet(Transform shotSpawn)
    {
        if (!_view.IsMine)
            return;

        var bullet = PhotonNetwork.Instantiate("Bullet", shotSpawn.position, shotSpawn.rotation).GetComponent<BulletBehaviour>();
    }

    public void BulletRequestDestroy(BulletBehaviour bullet)
    {
        if (!_view.IsMine)
            return;

        PhotonNetwork.Destroy(bullet.gameObject);
        Debug.Log("bulletDestroyed");
    }

}
