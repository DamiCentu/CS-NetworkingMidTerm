using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    PhotonView _view;
    ServerNetwork _server;

    void Start()
    {
        _server = ServerNetwork.Instance;
        _view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (!_view.IsMine)
            return;

        if (Input.GetButton("Fire1"))
        {
            _server.PlayerRequestShoot(PhotonNetwork.LocalPlayer);
        }
    }

    void FixedUpdate()
    {
        if (!_view.IsMine)
            return;

        var axisAmountToRotate = Input.GetAxisRaw("Horizontal");

        if(axisAmountToRotate != 0)
            _server.PlayeRequestRotate(PhotonNetwork.LocalPlayer,axisAmountToRotate);

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            _server.PlayerRequestAccelerate(PhotonNetwork.LocalPlayer);
    }
}
