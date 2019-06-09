using UnityEngine;
using System.Collections;

public class BulletBehaviour : MonoBehaviour
{
	public float speed;
    public float destroyTreshhold = 5;

    Rigidbody _rb;
    Boundary _boundary;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Start ()
	{
		_rb.velocity = transform.forward * speed;
        _boundary = FindObjectOfType<Boundary>();
    }

    private void Update()
    {
        if (_rb.position.x > _boundary.x + destroyTreshhold || _rb.position.x < -_boundary.x - destroyTreshhold || _rb.position.z > _boundary.z + destroyTreshhold || _rb.position.z < -_boundary.z - destroyTreshhold)
            Destroy(gameObject);
    }
}
