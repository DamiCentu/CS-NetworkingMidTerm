using UnityEngine;
using System.Collections;
using System;
using Photon.Pun;
using Photon.Realtime;

public class PlayerInstance : MonoBehaviourPun , IOnHit
{
    public float force = 15f;
    public float rotateSpeed = 5f;

	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
    public float secondaryFireRate;
    public PlayerTextBehaviour playerNameText;
    public int life = 3;

    int _currentLife = 0;
	 
	float nextFire;
    float secondaryNextFire;
    float fireMultiplier = 1;

    Rigidbody _rb;
    Boundary _boundary;
    AudioSource _audioSource;

    Vector3 _startPos = new Vector3();

    PhotonView _view;
    ServerNetwork _server;

    private void Start()
    {
        _server = ServerNetwork.Instance;
        _view = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody>();
        _boundary = FindObjectOfType<Boundary>();
        _audioSource = GetComponent<AudioSource>();

        _currentLife = life;
    }

    public void SaveStartPos(Vector3 startPos)
    {
        _startPos = startPos;  
    }

    public void SetPlayerName(string playerName)
    {
        playerNameText.SetText(playerName);
    }

    public void PowerUpPicked(float effectDuration)
    {
        fireMultiplier = 0.4f;
        StartCoroutine(PowerUpRoutine(effectDuration));
    }

    IEnumerator PowerUpRoutine(float effectDuration)
    {
        yield return new WaitForSeconds(effectDuration);

        fireMultiplier = 1;
    }

    public void ResetPlayerInstance()
    {
        RequestActivateObject(true);
        _currentLife = life;
        _rb.position = _startPos;
        _rb.velocity = new Vector3();
    }

    public void InstantiateBullet(Player player) //llamar cuando se hace el fire 1
    {
        if (!_server.PlayerCanMove)
            return;

        if (Time.time <= nextFire) return;

        nextFire = Time.time + fireRate * fireMultiplier;
        PhotonNetwork.Instantiate("Bullet", shotSpawn.position, shotSpawn.rotation);
        _audioSource.Play();
    }

    public void InstantiateSecondaryBullet(Player player) //llamar cuando se hace el fire 2
    {
        if (!_server.PlayerCanMove)
            return;

        if (Time.time <= secondaryNextFire) return;

        secondaryNextFire = Time.time + secondaryFireRate * fireMultiplier;
        PhotonNetwork.Instantiate("SecondaryBullet", shotSpawn.position, shotSpawn.rotation).GetComponent<SecondaryBulletBehaviour>().SetOwner(this);
        _audioSource.Play();
    }

    void FixedUpdate ()
	{
        if (!_server.PlayerCanMove)
            return;
        if (!_view.IsMine) return;

        AdjustToBoundary();
    }

    public void Accelerate()
    {
        if (!_server.PlayerCanMove)
            return;
        _rb.AddForce(transform.forward * force, ForceMode.Force);
    }

    void AdjustToBoundary()
    {
        if (_rb.position.x > _boundary.x || _rb.position.x < -_boundary.x)
        {
            AdjustVelocity(new Vector3(0, 0, _rb.velocity.z));
        }
        
        if (_rb.position.z > _boundary.z || _rb.position.z < -_boundary.z)
        {
            AdjustVelocity(new Vector3(_rb.velocity.x, 0, 0));
        }
    }

    public void RotatePlayer(float v)
    {
        if (!_server.PlayerCanMove)
            return;

        if (v != 0)
            _rb.rotation = _rb.rotation * Quaternion.Euler(new Vector3(0f, v, 0f).normalized * rotateSpeed);
    }
    
    public void AdjustVelocity(Vector3 velocity)
    {
        if (!_server.PlayerCanMove)
            return;

        _rb.velocity = velocity;
        _rb.position = new Vector3(Mathf.Clamp(_rb.position.x, -_boundary.x, _boundary.x), 0.0f, Mathf.Clamp(_rb.position.z, -_boundary.z, _boundary.z));
    }

    public void RequestActivateObject(bool active)
    {
        gameObject.SetActive(active);
        _view.RPC("DeactivateObject", RpcTarget.OthersBuffered, active);
    }

    [PunRPC]
    void DeactivateObject(bool active)
    {
        gameObject.SetActive(active);
    }

    public void OnHit()
    {
        if (!_server || !_server.PlayerCanMove)
            return;

        if (!_view.IsMine) return;

        _currentLife--;

        if (_currentLife <= 0)
        {
            PhotonNetwork.Instantiate("ExplotionPlayer", transform.position, transform.rotation);
            RequestActivateObject(false);
            _server.SetLooser(this);
        }
        else
        {
            PhotonNetwork.Instantiate("ExplotionAst", transform.position, transform.rotation);
        }
    }
}
