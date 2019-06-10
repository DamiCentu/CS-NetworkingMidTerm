using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonNetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject buttons;
    bool _isHost;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void BTNConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void BTNHostServer()
    {
        _isHost = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        buttons.SetActive(false);
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        buttons.SetActive(true);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Log in to lobby");
        if (_isHost)
        {
            PhotonNetwork.CreateRoom("MainRoom", new RoomOptions() { MaxPlayers = 4 });
            return;
        }
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined to a room");

        if (_isHost)
            PhotonNetwork.Instantiate("ServerNetwork", Vector3.zero, Quaternion.identity);
        else
            PhotonNetwork.Instantiate("Controller", Vector3.zero, Quaternion.identity);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("FALLO PORQUE " + message);
        PhotonNetwork.Disconnect();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("CREO UN ROOM");
    }
    
}
