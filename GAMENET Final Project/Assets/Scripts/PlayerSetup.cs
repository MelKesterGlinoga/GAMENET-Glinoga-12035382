using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject fpsModel;
    public GameObject nonFpsModel;
    public GameObject playerUi;

    [SerializeField]
    private Camera camera;

    [SerializeField]
    TextMeshProUGUI playerNameText;

    private Animator animator;
    public Avatar fpsAvatar, nonFpsAvatar;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();

        fpsModel.SetActive(photonView.IsMine);
        nonFpsModel.SetActive(!photonView.IsMine);

        animator.SetBool("isLocalPlayer", photonView.IsMine);
        animator.avatar  = photonView.IsMine ? fpsAvatar : nonFpsAvatar;

        if (photonView.IsMine)
        {
            transform.GetComponent<RigidbodyFirstPersonController>().enabled = true;
            camera.GetComponent<Camera>().enabled = true;
        }
        else
        {
            transform.GetComponent<RigidbodyFirstPersonController>().enabled = false;
            camera.GetComponent<Camera>().enabled = false;
        }

        playerNameText.text = photonView.Owner.NickName;
    }

    [PunRPC]
    public void WhoDied(PhotonMessageInfo info)
    {
        TextMeshProUGUI voidText = GameObject.Find("VoidText").GetComponent<TextMeshProUGUI>();
        voidText.enabled = true;

        if (photonView.IsMine)
        {
            GetComponent<PlayerSetup>().camera.transform.parent = null;
            GetComponent<PlayerSetup>().playerUi.transform.SetParent(null);
            Destroy(GetComponent<PlayerSetup>().playerUi, 10);

            voidText.text = info.Sender.NickName + " fell to the void";
        }
        else
        {
            voidText.text = info.photonView.Owner.NickName + " fell to the void!";
        }
    }
}
