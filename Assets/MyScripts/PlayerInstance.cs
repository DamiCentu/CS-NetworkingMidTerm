using UnityEngine;
using System.Collections;
using System;
using Photon.Pun;

public class PlayerInstance : MonoBehaviourPun
{
    public float force = 15f;
    public float rotateSpeed = 5f;

	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	 
	private float nextFire;

    Rigidbody _rb;
    Boundary _boundary;
    AudioSource _audioSource;

    PhotonView _view;

    private void Awake()
    {
        _view = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody>();
        _boundary = FindObjectOfType<Boundary>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void InstantiateBullet() //llamar cuando se hace el fire 1
    {
        if (Time.time <= nextFire) return;

        nextFire = Time.time + fireRate;
        Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
        _audioSource.Play();
    }

	void FixedUpdate ()
	{
        if (!_view.IsMine) return;

        AdjustToBoundary();
    }

    public void Accelerate()
    {
        _rb.AddForce(transform.forward * force, ForceMode.Force);
    }

    void AdjustToBoundary()
    {
        if (_rb.position.x > _boundary.x || _rb.position.x < -_boundary.x)
            _view.RPC("AdjustVelocity", RpcTarget.OthersBuffered, new Vector3(0, 0, _rb.velocity.z));
        
        if (_rb.position.z > _boundary.z || _rb.position.z < -_boundary.z)
            _view.RPC("AdjustVelocity", RpcTarget.OthersBuffered, new Vector3(_rb.velocity.x, 0, 0));
    }

    public void RotatePlayer(float v)
    {
        if (v != 0)
            _rb.rotation = _rb.rotation * Quaternion.Euler(new Vector3(0f, v, 0f).normalized * rotateSpeed);
    }

    [PunRPC]
    public void AdjustVelocity(Vector3 velocity)
    {
        _rb.velocity = velocity;
        _rb.position = new Vector3(Mathf.Clamp(_rb.position.x, -_boundary.x, _boundary.x), 0.0f, Mathf.Clamp(_rb.position.z, -_boundary.z, _boundary.z));
    }
}
