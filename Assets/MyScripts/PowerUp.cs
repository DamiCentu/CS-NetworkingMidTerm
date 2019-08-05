using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PowerUp : MonoBehaviourPun
{
    public float tumble = 15;
    public float effectDuration = 5;
    public ActivableGO shieldGO;
    public ActivableGO rapidShootGO;
   

    Rigidbody _rb;
    Boundary _boundary;
    ServerNetwork _server;
    PhotonView _view;

    PowerUpType _type = PowerUpType.RapidShot;
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
            player.PowerUpPicked(effectDuration, _type);
            PhotonNetwork.Instantiate("PickUpPowerUpEffect", transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void SetType(PowerUpType type)
    {
        if (!_view)
            _view = GetComponent<PhotonView>();

        if (!_view.IsMine)
            return;

        _type = type;

        if (_type == PowerUpType.Shield)
        {
            shieldGO.RequestActivateObject(true);
            rapidShootGO.RequestActivateObject(false);
        }
    }

    public enum PowerUpType
    {
        RapidShot,
        Shield
    }
}
