using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class SpiderBehaviour : MonoBehaviourPun
{
    public float speed = 5;
    public int hitsCanTake = 3;
    public float radiusOfWaypoints = 0.2f;

    //Vector3[] _allDirection;

    Animator _anim;
    Rigidbody _rb;
    ServerNetwork _server;
    PhotonView _view;
    bool _canMove = true;

    Waypoint _currentWaypoint;
    int _currentIndex = 0;

    void Start()
    { 
        //_allDirection = new [] { Vector3.forward, Vector3.right, -Vector3.forward, -Vector3.right};

        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _view = GetComponent<PhotonView>();
        _server = ServerNetwork.Instance;
    }

    public void SetWaypoint(Waypoint waypoint)
    {
        _currentWaypoint = waypoint;
    
        if(!_rb)
            _rb = GetComponent<Rigidbody>();

        _rb.position = waypoint.transform.position;
    }

    IEnumerator IdleRoutine()
    {
        _canMove = false;

        SetAnimationValue("Z", 0);
        SetAnimationValue("X", 0);
        yield return new WaitForSeconds(2f);

        _currentWaypoint = _currentWaypoint.next;

        _canMove = true;
    }

    void Update()
    {
        if (_view.IsMine)
        {
            if (!_server.PlayerCanMove)
                PhotonNetwork.Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (_currentWaypoint == null || _currentWaypoint.next == null || !_rb)
            return;

        if(Vector3.Distance( _rb.position, _currentWaypoint.transform.position) < radiusOfWaypoints)
        {
            if(_canMove)
                StartCoroutine(IdleRoutine());
        }

        if (!_canMove)
            return;

        var direction = _currentWaypoint.transform.position - _rb.position;
        direction.Normalize();

        if(direction == Vector3.forward)
        {
            SetAnimationValue("Z", -1);
            SetAnimationValue("X", 0);
        }
        else if (direction == -Vector3.forward)
        {
            SetAnimationValue("Z", 1);
            SetAnimationValue("X", 0);
        }
        else if (direction == Vector3.right)
        {
            SetAnimationValue("X", 1);
            SetAnimationValue("Z", 0);
        }
        else if (direction == -Vector3.right)
        {
            SetAnimationValue("X", -1);
            SetAnimationValue("Z", 0);
        }

        _rb.position = _rb.position + (direction * Time.fixedDeltaTime * speed);
    }

    private void SetAnimationValue(string parameterName, int parameterValue)
    {
        if (_anim)
            _anim.SetInteger(parameterName, parameterValue);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_view.IsMine)
            return;

        if (other.gameObject.layer == 8)
            return;

        var onHitable = other.GetComponent<IOnHit>();

        if (onHitable != null)
        {
            onHitable.OnHit();
        }

        hitsCanTake--;

        if (hitsCanTake > 0)
        {
            PhotonNetwork.Instantiate("SpiderHit", transform.position, transform.rotation);
            return;
        }

        PhotonNetwork.Instantiate("SpiderDestroy", transform.position, transform.rotation);

        PhotonNetwork.Destroy(gameObject);
    }
}
