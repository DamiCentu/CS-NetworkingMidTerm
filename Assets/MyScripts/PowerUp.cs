using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PowerUp : MonoBehaviourPun
{
    public float tumble = 15;
    public float effectDuration = 5;

    Rigidbody _rb;
    Boundary _boundary;
    ServerNetwork _server;
    PhotonView _view;
    void Start()
    {
        if (!_rb)
            _rb = GetComponent<Rigidbody>();

        _view = GetComponent<PhotonView>();
        _server = ServerNetwork.Instance;

        _rb.angularVelocity = Random.insideUnitSphere * tumble;
    }

    void Update()
    {
        if (_view.IsMine)
        {
            if (!_server.PlayerCanMove)
                PhotonNetwork.Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_view.IsMine )
            return;

        var player = other.GetComponent<PlayerInstance>();

        if (player)
        {
            player.PowerUpPicked(effectDuration);
            PhotonNetwork.Instantiate("PickUpPowerUpEffect", transform.position, Quaternion.Euler(new Vector3(-90, 0, 0))));
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
