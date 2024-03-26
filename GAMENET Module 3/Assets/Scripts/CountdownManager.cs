using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CountdownManager : MonoBehaviourPunCallbacks
{
    public float timeToStartRace = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        TextMeshProUGUI timerText = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
        
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            timerText = RacingGameManager.instance.timeText;
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            timerText = DeathRaceGameManager.instance.timeText2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (timeToStartRace > 0)
            {
                timeToStartRace -= Time.deltaTime;
                photonView.RPC("SetTime", RpcTarget.AllBuffered, timeToStartRace);
            }
            else if (timeToStartRace < 0)
            {
                photonView.RPC("StartRace", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    public void SetTime(float time)
    {
        TextMeshProUGUI timerText = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
        if (time > 0)
        {
            timerText.text = time.ToString("F1");
        }
        else
        {
            timerText.text = "";
        }
    }

    [PunRPC]
    public void StartRace()
    {
        GetComponent<VehicleMovement>().isControlEnabled = true;
        this.enabled = false;
    }
}
