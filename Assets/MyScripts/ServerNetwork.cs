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
    bool _playerCanMove = false;
    bool _startTimerShowed = false;
    AsteroidSpawner _asteroidSpawner;
    SpiderSpawner _spiderSpawner;
    PowerUpSpawner _powerUpSpawner;

    TextBehaviour _textStart;
    ActivableGO _panel;

    void Awake()
    {
        _view = GetComponent<PhotonView>();

        if (!_textStart)
            _textStart = FindObjectsOfType<TextBehaviour>().Where(x => x.id == "startText").First();

        if (!_panel)
            _panel = FindObjectsOfType<ActivableGO>().Where(x => x.id == "Panel").First();

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
        newPlayer.SetPlayerName("Player " + p.ActorNumber.ToString());

        var text = FindObjectsOfType<TextBehaviour>().Where(x => x.id == "playerLobbyText" && !x.Taken).First();
        text.SetTaken(true);
        text.UpdateText("Player: " + p.ActorNumber.ToString());

        if (!_startTimerShowed && _players.Count > amountOfPlayersToStart - 1)
        {
            _startTimerShowed = true;
            
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame(bool fromRestart = false)
    {
        if(fromRestart)
            _textStart.UpdateText("Restarting game");

        yield return new WaitForSeconds(2f);
        _panel.RequestActivateObject(false);
        _textStart.SetActive(true);
        for (int i = secondsToStart; i > 0; i--)
        {
            _textStart.UpdateText("Starting game in " + i);
            yield return new WaitForSeconds(1f);
        }

        _textStart.SetActive(false);

        PlayerCanMove = true;

        _asteroidSpawner = gameObject.AddComponent(typeof(AsteroidSpawner)) as AsteroidSpawner;
        _asteroidSpawner.StartSpawning();

        _spiderSpawner = gameObject.AddComponent(typeof(SpiderSpawner)) as SpiderSpawner;
        _spiderSpawner.StartSpawning();

        _powerUpSpawner = gameObject.AddComponent(typeof(PowerUpSpawner)) as PowerUpSpawner;
        _powerUpSpawner.StartSpawning();
    }

    public void SetLooser(PlayerInstance instance)
    {
        if (!_view.IsMine)
            return;

        if (!_loosers.Contains(instance))
        {
            _loosers.Add(instance);
        }

        if(_loosers.Count >= _players.Count - 1 ) //1 left
        {
            StartCoroutine(EndRoundRoutine());
        }
    }

    IEnumerator EndRoundRoutine()
    {
        PlayerCanMove = false;
        _textStart.SetActive(true);

        Player winner = null;
        foreach (var player in _players)
        {
            if (player.Value.gameObject.activeSelf)
                winner = player.Key;
        }
        _textStart.UpdateText("Player " + winner.ActorNumber + " wins" );
        yield return new WaitForSeconds(secondsAtEnd);

        foreach (var player in _players)
        {
            player.Value.ResetPlayerInstance();
        }

        _loosers.Clear();

        StartCoroutine(StartGame(true));
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
    void RequestSecondaryShoot(Player player)
    {
        if (_players.ContainsKey(player))
            _players[player].InstantiateSecondaryBullet(player);
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

    public void PlayerRequestSecondaryShoot(Player player)
    {
        _view.RPC("RequestSecondaryShoot", serverReference, player);
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
