using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using TMPro;

public class TakingDamage : MonoBehaviourPunCallbacks
{
    public Image healthbar;
    public Camera camera;

    private float startHealth = 100;
    public float health;
    
    public enum RaiseEventsCode
    {
        WhoWasEliminatedCode = 1
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventsCode.WhoWasEliminatedCode)
        {
            object[] dataElim = (object[])photonEvent.CustomData;

            string nickNameOfEliminatedPlayer = (string)dataElim[0];

            TextMeshProUGUI eliminatedText = GameObject.Find("EliminatedText").GetComponent<TextMeshProUGUI>();
            eliminatedText.enabled = true;
            eliminatedText.text = nickNameOfEliminatedPlayer + " was eliminated!";
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
        healthbar.fillAmount = health / startHealth;

        TextMeshProUGUI eliminatedText = GameObject.Find("EliminatedText").GetComponent<TextMeshProUGUI>();
        eliminatedText.enabled = false;

        TextMeshProUGUI killFeed = GameObject.Find("KillFeed").GetComponent<TextMeshProUGUI>();
        killFeed.enabled = false;
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        health -= damage;
        healthbar.fillAmount = health / startHealth;
        Debug.Log(health);

        if (health <= 0)
        {
            Die();
            photonView.RPC("KillFeed", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void KillFeed(PhotonMessageInfo info)
    {
        TextMeshProUGUI killFeed = GameObject.Find("KillFeed").GetComponent<TextMeshProUGUI>();
        killFeed.enabled = true;
        if (photonView.IsMine)
        {
            killFeed.text = info.Sender.NickName + " killed " + info.photonView.Owner.NickName;
        }
        else
        {
            killFeed.text = "You killed " + info.photonView.Owner.NickName;
        }
        StartCoroutine(ClearText());
    }

    private void Die()
    {
        GetComponent<PlayerSetup>().camera.transform.parent = null;
        GetComponent<VehicleMovement>().enabled = false;
        GetComponent<Shooting>().enabled = false;

        string nickName = photonView.Owner.NickName;

        // event data
        object[] dataElim = new object[] { nickName };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.WhoWasEliminatedCode, dataElim, raiseEventOptions, sendOptions);
    }

    IEnumerator ClearText()
    {
        TextMeshProUGUI killFeed = GameObject.Find("KillFeed").GetComponent<TextMeshProUGUI>();
        float timeToClear = 5.0f;

        while (timeToClear > 0)
        {
            yield return new WaitForSeconds(1.0f);
            timeToClear--;
        }

        killFeed.text = "";
    }
}
