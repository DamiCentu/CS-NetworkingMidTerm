using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AsteroidBehaviour : MonoBehaviourPun
{
    public float minSpeed = 5;
    public float maxSpeed = 15;
    public float destroyTreshhold = 5;
    public float tumble = 5;

    Rigidbody _rb;
    Boundary _boundary;
    ServerNetwork _server;
    PhotonView _view;

    void Start()
    {
        if(!_rb)
            _rb = GetComponent<Rigidbody>();
        _view = GetComponent<PhotonView>();
        _server = ServerNetwork.Instance;
        _boundary = FindObjectOfType<Boundary>();

        _rb.angularVelocity = Random.insideUnitSphere * tumble;
    }

    public void SetVelocity(Vector3 directionBySide)
    {
        if (!_rb)
            _rb = GetComponent<Rigidbody>();
        _rb.velocity = directionBySide * Random.Range(minSpeed, maxSpeed);
    }

    void Update()
    {
        if (_rb && _rb.position.x > _boundary.x + destroyTreshhold || _rb.position.x < -_boundary.x - destroyTreshhold || _rb.position.z > _boundary.z + destroyTreshhold || _rb.position.z < -_boundary.z - destroyTreshhold)
        {
            if (_view.IsMine)
                PhotonNetwork.Destroy(gameObject);
        }

        if (_view.IsMine)
        {
            if(!_server.PlayerCanMove)
                PhotonNetwork.Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_view.IsMine)
            return;

        var onHitable = other.GetComponent<IOnHit>();

        if(onHitable != null)
        {
            onHitable.OnHit();
        }

        if (other.gameObject.layer == 8 || other.gameObject.layer == 11)
            return;

        PhotonNetwork.Instantiate("ExplosionEnemy", transform.position, transform.rotation);
                
        PhotonNetwork.Destroy(gameObject);
    }
}
