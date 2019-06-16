using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class BulletBehaviour : MonoBehaviourPun
{
	public float speed;
    public float destroyTreshhold = 5;

    Rigidbody _rb;
    Boundary _boundary;
    ServerNetwork _server;

    void Start ()
    {
        _rb = GetComponent<Rigidbody>();
        _view = GetComponent<PhotonView>();
        _server = ServerNetwork.Instance;
        _rb.velocity = transform.forward * speed;
        _boundary = FindObjectOfType<Boundary>();
    }

    private void Update()
    {
        if (_rb && _rb.position.x > _boundary.x + destroyTreshhold || _rb.position.x < -_boundary.x - destroyTreshhold || _rb.position.z > _boundary.z + destroyTreshhold || _rb.position.z < -_boundary.z - destroyTreshhold)
            _server.BulletRequestDestroy(this);
    }

    private PhotonView _view;

    public bool AreYouMine()
    {
        return _view.IsMine;
    }
}
