using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PlayerTextBehaviour : MonoBehaviourPun
{
    public TextMesh textMesh;
    PhotonView _view;
    
    void Update()
    {
        transform.position = transform.parent.position + (Vector3.forward * 2) - Vector3.up;
        transform.rotation = Quaternion.Euler(new Vector3(90f,0 , 0));
    }

    public void SetText(string playerName)
    {
        if(!_view)
            _view = GetComponent<PhotonView>();

        _view.RPC("UpdateText", RpcTarget.AllBuffered , playerName);
    }

    [PunRPC]
    void UpdateText(string text)
    {
        textMesh.text = text;
    }
}
