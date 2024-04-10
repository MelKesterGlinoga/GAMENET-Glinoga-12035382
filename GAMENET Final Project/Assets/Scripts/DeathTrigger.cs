using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DeathTrigger : MonoBehaviourPunCallbacks
{
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<PhotonView>().RPC("WhoDied", RpcTarget.AllBuffered);
        Destroy(other.gameObject);
    }
}
