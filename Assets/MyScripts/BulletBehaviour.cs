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
    PhotonView _view;
    PlayerInstance _owner;

    void Start ()
    {
        _rb = GetComponent<Rigidbody>();
        _view = GetComponent<PhotonView>();
        _server = ServerNetwork.Instance;
        _rb.velocity = transform.forward * speed;
        _boundary = FindObjectOfType<Boundary>();
    }

    void Update()
    {
        if (_rb && _rb.position.x > _boundary.x + destroyTreshhold || _rb.position.x < -_boundary.x - destroyTreshhold || _rb.position.z > _boundary.z + destroyTreshhold || _rb.position.z < -_boundary.z - destroyTreshhold)
        {
            if (_view.IsMine)
                PhotonNetwork.Destroy(gameObject);
        }
    }

    public void SetOwner(PlayerInstance owner)
    {
        _owner = owner;
    }

    void OnTriggerEnter(Collider other)
    {
        if(!_view)
            _view = GetComponent<PhotonView>();

        if (!_view.IsMine)
            return;

        var player = other.gameObject.GetComponent<PlayerInstance>();
        if (player && _owner == player)
            return;

        var onHitable = other.GetComponent<IOnHit>();

        if (onHitable != null)
        {
            onHitable.OnHit();
        }

        if (other.gameObject.layer == 11)
            return;

        PhotonNetwork.Destroy(gameObject);
    }
}
