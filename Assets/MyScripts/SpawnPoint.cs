using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPoint : MonoBehaviourPun
{
    bool _taken;
    PhotonView _view;

    public bool Taken { get => _taken; }

    private void Awake()
    {
        _view = GetComponent<PhotonView>();
    }

    public void SetTaken()
    {
        _view.RPC("SetTakenRPC", RpcTarget.OthersBuffered);
    }

    [PunRPC]
    void SetTakenRPC()
    {
        _taken = true;
    }
}
