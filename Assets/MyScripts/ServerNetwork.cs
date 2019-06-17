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
    public bool PlayerCanMove { get => _playerCanMove; set => _playerCanMove = value; }

    PhotonView _view;
    public Player serverReference;
    public int amountOfPlayersToStart = 2;
    public int secondsToStart = 3;
    public int secondsAtEnd= 3;

    List<PlayerInstance> _loosers = new List<PlayerInstance>();

    int playerId = 0;

    Dictionary<Player, PlayerInstance> _players = new Dictionary<Player, PlayerInstance>();
    private bool _playerCanMove = false;

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
            spawnPoint.SetTaken(true);

        var newPlayer = PhotonNetwork.Instantiate("PlayerInstance",
                        spawnPoint.transform.position,
                        Quaternion.identity).GetComponent<PlayerInstance>();

        _players.Add(p, newPlayer);

        newPlayer.SaveStartPos(spawnPoint.transform.position);       

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

        PlayerCanMove = true;
    }

    public void SetLooser(PlayerInstance instance)
    {
        if (!_loosers.Contains(instance))
        {
            _loosers.Add(instance);
        }

        if(_loosers.Count >= _players.Count - 1 ) //1 left
        {
            StartCoroutine(EndRoundRoutine(FindObjectsOfType<TextBehaviour>().Where(x => x.id == "startText").First()));
        }
    }

    IEnumerator EndRoundRoutine(TextBehaviour textToUpdate)
    {
        PlayerCanMove = false;
        textToUpdate.SetActive(true);

        Player winner = null;
        foreach (var player in _players)
        {
            if (player.Value.gameObject.activeSelf)
                winner = player.Key;
        }
        textToUpdate.UpdateText("Player " + winner.ActorNumber + "wins" );
        yield return new WaitForSeconds(secondsAtEnd);
        textToUpdate.SetActive(true);

        foreach (var player in _players)
        {
            player.Value.ResetPlayerInstance();
        }

        _loosers.Clear();

        StartCoroutine(StartGame(FindObjectsOfType<TextBehaviour>().Where(x => x.id == "startText").First()));
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

    public void BulletRequestInstantiate(Transform shotSpawn)
    {
        if (!_view.IsMine)
            return;

        PhotonNetwork.Instantiate("Bullet", shotSpawn.position, shotSpawn.rotation);
    }

    public void BulletRequestDestroy(BulletBehaviour bullet)
    {
        if (!_view.IsMine)
            return;

        PhotonNetwork.Destroy(bullet.gameObject);
    }

    //particles

    public void ParticleRequestInstantiate(string particleName,Transform pos)
    {
        if (!_view.IsMine)
            return;
        
        PhotonNetwork.Instantiate(particleName, pos.position, pos.rotation);
    }

    public void ParticleRequestDestroy(ParticleBehaviour particle)
    {
        if (!_view.IsMine)
            return;

        PhotonNetwork.Destroy(particle.gameObject);
    }

}
