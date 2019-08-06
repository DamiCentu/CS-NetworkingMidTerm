using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SecondaryBulletBehaviour : MonoBehaviourPun
{
    public float speed;
    public float destroyTreshhold = 5;
    public float radiusToChase = 5;
    public float smoothToRotate = 1;
    public LayerMask maskToChase;

    Rigidbody _rb;
    Boundary _boundary;
    ServerNetwork _server;
    PhotonView _view;
    PlayerInstance _owner;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _view = GetComponent<PhotonView>();
        _server = ServerNetwork.Instance;
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

    private void FixedUpdate()
    {
        if (!_rb || !_owner)
            return;

        var overlap = Physics.OverlapSphere(_rb.position, radiusToChase, maskToChase).Select(x => x.gameObject.GetComponent<PlayerInstance>()).ToList();
        
        if (overlap.Contains(_owner))
            overlap.Remove(_owner);

        if(overlap.Count > 0)
        {
            transform.forward = Vector3.Lerp(transform.forward, (overlap[0].transform.position - transform.position).normalized , smoothToRotate);
        }

        _rb.velocity = transform.forward * speed;
    }

    public void SetOwner(PlayerInstance owner)
    {
        _owner = owner;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_view)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radiusToChase);
    }
}
