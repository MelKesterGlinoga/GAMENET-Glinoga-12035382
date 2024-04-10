using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using UnityStandardAssets.Characters.FirstPerson;

public class FreezePlayer : MonoBehaviourPunCallbacks
{
    private int startStats = 1;
    public float status;

    public enum RaiseEventsCode
    {
        WhoWasFrozenEventCode = 0
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventsCode.WhoWasFrozenEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            string nickNameOfFrozenPlayer = (string)data[0];

            TextMeshProUGUI frozenText = GameObject.Find("FrozenText").GetComponent<TextMeshProUGUI>();
            frozenText.enabled = true;
            frozenText.text = nickNameOfFrozenPlayer + " was frozen!";

            StartCoroutine(ClearFrozenText());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // status
        // 0 = frozen
        // 1 = !frozen
        status = startStats;
    }

    [PunRPC]
    public void StunPlayer(int damage)
    {
        status -= damage;

        if (status <= 0)
        {
            Freeze(); // Event
            photonView.RPC("WhoFrozeWho", RpcTarget.AllBuffered); // RPC
        }
    }

    
    [PunRPC]
    public void WhoFrozeWho(PhotonMessageInfo info)
    {
        TextMeshProUGUI whoFrozeText = GameObject.Find("WhoFrozeWhoText").GetComponent<TextMeshProUGUI>();
        whoFrozeText.enabled = true;

        if (photonView.IsMine)
        {
            whoFrozeText.text = info.Sender.NickName + " froze " + info.photonView.Owner.NickName;
        }
        else
        {
            whoFrozeText.text = "You froze " + info.photonView.Owner.NickName;
        }

        StartCoroutine(ClearText());
    }
    

    private void Freeze()
    {
        if (photonView.IsMine)
        {
            transform.GetComponent<RigidbodyFirstPersonController>().enabled = false;
        }
        else
        {
            transform.GetComponent<RigidbodyFirstPersonController>().enabled = true;
        }

        string nickName = photonView.Owner.NickName;

        // event data
        object[] data = new object[] { nickName };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte) RaiseEventsCode.WhoWasFrozenEventCode, data, raiseEventOptions, sendOptions);
    }

    IEnumerator ClearFrozenText()
    {
        TextMeshProUGUI frozenText = GameObject.Find("FrozenText").GetComponent<TextMeshProUGUI>();
        float timeToClear = 5.0f;

        while (timeToClear > 0)
        {
            yield return new WaitForSeconds(1.0f);
            timeToClear--;
        }

        if (photonView.IsMine)
        {
            transform.GetComponent<RigidbodyFirstPersonController>().enabled = true;
        }
        else
        {
            transform.GetComponent<RigidbodyFirstPersonController>().enabled = false;
        }

        frozenText.text = "";
        status = startStats;
    }

    IEnumerator ClearText()
    {
        TextMeshProUGUI whoFrozeText = GameObject.Find("WhoFrozeWhoText").GetComponent<TextMeshProUGUI>();
        float clearTextTime = 5.0f;

        while (clearTextTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            clearTextTime--;
        }

        whoFrozeText.text = "";
    }
}
