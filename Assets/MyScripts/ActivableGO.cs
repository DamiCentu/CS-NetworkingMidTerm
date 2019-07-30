using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActivableGO : MonoBehaviourPun
{
    PhotonView _view;
    public string id = "";

    public void RequestActivateObject(bool active)
    {
        if (!_view)
            _view = GetComponent<PhotonView>();

        _view.RPC("DeactivateObject", RpcTarget.AllBuffered, active);
    }

    [PunRPC]
    void DeactivateObject(bool active)
    {
        gameObject.SetActive(active);
    }
}
