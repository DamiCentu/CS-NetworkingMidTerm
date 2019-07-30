using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TextBehaviour : MonoBehaviourPun
{
    Text _text;
    PhotonView _view;
    bool _taken;
    public bool Taken { get => _taken; }

    public string id = "BaseId";

    void Awake()
    {
        _view = GetComponent<PhotonView>();
        _text = GetComponent<Text>();
        _text.text = "";
    }

    public void SetTaken(bool taken)
    {
        _view.RPC("SetTakenRPC", RpcTarget.AllBuffered, taken);
    }

    [PunRPC]
    void SetTakenRPC(bool taken)
    {
        _taken = taken;
    }

    public void UpdateText(string text)
    {
        _view.RPC("UpdateTextRPC", RpcTarget.AllBuffered, text);
    }

    [PunRPC]
    void UpdateTextRPC(string text)
    {
        _text.text = text;
    }

    public void SetActive(bool active)
    {
        _view.RPC("SetActiveRPC", RpcTarget.AllBuffered, active);
    }

    [PunRPC]
    void SetActiveRPC(bool active)
    {
        gameObject.SetActive(active);
    }

}
