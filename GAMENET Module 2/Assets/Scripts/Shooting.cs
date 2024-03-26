using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using Photon.Pun.UtilityScripts;

public class Shooting : MonoBehaviourPunCallbacks
{
    //public NetworkManager networkManager;
    //public GameObject insideRoomPanel;

    public Camera camera;
    public GameObject hitEffectPrefab;

    [Header("HP Related Stuff")]
    public float startHealth = 100;
    private float health;
    public Image healthBar;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire()
    {
        RaycastHit hit;
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out hit, 200))
        {
            Debug.Log(hit.collider.gameObject.name);

            photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point);

            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25);
            }
        }
    }

    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        this.health -= damage;
        this.healthBar.fillAmount = health / startHealth;

        if (health <= 0)
        {
            Die();
            info.Sender.GetScore();
            info.Sender.AddScore(1);

            if (info.Sender.GetScore() >= 10)
            {                
                gameObject.GetComponent<PhotonView>().RPC("WinningText", RpcTarget.AllBuffered);
                Debug.Log(info.Sender.NickName + " wins with " + info.Sender.GetScore().ToString());
            }
        }
    }
    
    [PunRPC]
    public void WinningText(PhotonMessageInfo info)
    {
        TMP_Text winnerText = GameObject.Find("WinnerText").GetComponent<TMP_Text>();

        if (photonView.IsMine)
        {
            winnerText.text = "You lose!";
        }
        else
        {
            winnerText.text = "You Win!";
        }
    }
    
    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
        GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGameObject, 0.2f);
    }

    public void Die()
    {
        if (photonView.IsMine)
        {
            animator.SetBool("isDead", true);
            StartCoroutine(RespawnCountdown());
        }
        gameObject.GetComponent<PhotonView>().RPC("KillFeed", RpcTarget.AllBuffered);
    }

    IEnumerator RespawnCountdown()
    {
        TMP_Text respawnText = GameObject.Find("Respawn Text").GetComponent<TMP_Text>();
        float respawnTime = 5.0f;

        while (respawnTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime--;

            transform.GetComponent<PlayerMovementController>().enabled = false;
            respawnText.text = "You are killed. Respawning in " + respawnTime.ToString(".00");
        }

        animator.SetBool("isDead", false);
        respawnText.text = "";

        int randomPointX = Random.Range(-20, 20);
        int randomPointZ = Random.Range(-20, 20);

        this.transform.position = new Vector3(randomPointX, 0, randomPointZ);
        transform.GetComponent<PlayerMovementController>().enabled = true;

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RegainHealth()
    {
        health = 100;
        healthBar.fillAmount = health / startHealth;
    }

    [PunRPC]
    public void KillFeed(PhotonMessageInfo info)
    {
        TMP_Text killFeed = GameObject.Find("KillFeed").GetComponent<TMP_Text>();

        if (photonView.IsMine)
        {
            killFeed.text = info.Sender.NickName + " killed " + info.photonView.Owner.NickName;
        }
        else
        {
            killFeed.text = "You killed " + info.photonView.Owner.NickName;
        }

        StartCoroutine(ShowKillTimer());
    }

    [PunRPC]
    IEnumerator ShowKillTimer()
    {
        TMP_Text killFeed = GameObject.Find("KillFeed").GetComponent<TMP_Text>();
        float showKillTimer = 3.0f;

        while (showKillTimer > 0)
        {
            yield return new WaitForSeconds(1.0f);
            showKillTimer--;
        }

        killFeed.text = "";
    }

    IEnumerator GoBackToLobby()
    {
        TMP_Text winnerText = GameObject.Find("WinnerText").GetComponent<TMP_Text>();
        float showWinText = 3.0f;

        while (showWinText > 0)
        {
            yield return new WaitForSeconds(1.0f);
            showWinText--;
        }

        winnerText.text = "";
        //networkManager.ActivatePanel(insideRoomPanel);
    }
}
