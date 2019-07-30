using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ParticleBehaviour : MonoBehaviourPun
{
    ServerNetwork _server;
    void Start()
    {
        StartCoroutine(DestroyRoutine());
    }

    IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(GetComponent<ParticleSystem>().main.duration);
        PhotonNetwork.Destroy(gameObject);
    }
}
