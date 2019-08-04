using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MaterialChanger : MonoBehaviour
{
    PhotonView _view;
    MeshRenderer _rend;

    Material _mat;

    public void RequestChangeMaterial(Material mat)
    {
        if (!_view)
            _view = GetComponent<PhotonView>();

        if (!_rend)
            _rend = GetComponent<MeshRenderer>();

        _mat = mat;

        _view.RPC("ChangeMaterial", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ChangeMaterial()
    {
        _rend.materials[0] = _mat;
    }
}
