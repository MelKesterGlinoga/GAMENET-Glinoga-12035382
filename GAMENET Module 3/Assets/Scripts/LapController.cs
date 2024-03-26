using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using TMPro;

public class LapController : MonoBehaviourPunCallbacks
{
    public List<GameObject> lapTriggers = new List<GameObject>();

    public enum RaiseEventsCode
    {
        WhoFinishedEventCode = 0
    }

    private int finishOrder = 0;

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
        if (photonEvent.Code == (byte)RaiseEventsCode.WhoFinishedEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            string nickNameOfFinishedPlayer = (string)data[0];
            finishOrder = (int)data[1];
            int viewId = (int)data[2];

            Debug.Log(nickNameOfFinishedPlayer + " " + finishOrder);

            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
            {
                GameObject orderUiText = RacingGameManager.instance.finisherTextUi[finishOrder - 1];
                orderUiText.SetActive(true);

                if (viewId == photonView.ViewID) // this is u !!
                {
                    orderUiText.GetComponent<TMP_Text>().text = finishOrder + " " + nickNameOfFinishedPlayer + " (YOU)";
                    orderUiText.GetComponent<TMP_Text>().color = Color.red;
                }
                else
                {
                    orderUiText.GetComponent<TMP_Text>().text = finishOrder + " " + nickNameOfFinishedPlayer;
                }
            }
            else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
            {
                GameObject orderUiText2 = DeathRaceGameManager.instance.finisherTextUi[finishOrder - 1];
                orderUiText2.SetActive(true);

                if (viewId == photonView.ViewID) // this is u !!
                {
                    orderUiText2.GetComponent<TMP_Text>().text = finishOrder + " " + nickNameOfFinishedPlayer + " (YOU)";
                    orderUiText2.GetComponent<TMP_Text>().color = Color.red;
                }
                else
                {
                    orderUiText2.GetComponent<TMP_Text>().text = finishOrder + " " + nickNameOfFinishedPlayer;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            foreach (GameObject go in RacingGameManager.instance.lapTriggers)
            {
                lapTriggers.Add(go);
            }
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            foreach (GameObject go in DeathRaceGameManager.instance.lapTriggers)
            {
                lapTriggers.Add(go);
            }
        } 
    }

    private void OnTriggerEnter(Collider col)
    {
        if (lapTriggers.Contains(col.gameObject))
        {
            int indexOfTrigger = lapTriggers.IndexOf(col.gameObject);

            lapTriggers[indexOfTrigger].SetActive(false);
        }

        if (col.gameObject.tag == "FinishTrigger")
        {
            GameFinish();
        }
    }

    public void GameFinish()
    {
        GetComponent<PlayerSetup>().camera.transform.parent = null;
        GetComponent<VehicleMovement>().enabled = false;

        finishOrder++;

        string nickName = photonView.Owner.NickName;
        int viewId = photonView.ViewID;

        // event data
        object[] data = new object[] { nickName, finishOrder, viewId };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {   
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.WhoFinishedEventCode, data, raiseEventOptions, sendOptions);
    }
}
