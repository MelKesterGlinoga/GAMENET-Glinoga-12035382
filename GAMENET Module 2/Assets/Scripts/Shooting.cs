using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera camera;
    public GameObject hitEffectPrefab;

    [Header("HP Related Stuff")]
    public float startHealth = 100;
    private float health;
    public Image healthBar;

    [Header("Point System")]
    public float startingPoints = 20;

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

        if(Physics.Raycast(ray, out hit, 200))
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

        if(health <= 0)
        {
            Die();
            gameObject.GetComponent<PhotonView>().RPC("LosePoints", RpcTarget.AllBuffered, 1);
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

            if(this.startingPoints <= 0)
            {
                GameOver();
            }
            else
            {
                StartCoroutine(RespawnCountdown());
            }
        }

        gameObject.GetComponent<PhotonView>().RPC("KillFeed", RpcTarget.AllBuffered);
    }

    IEnumerator RespawnCountdown()
    {
        GameObject respawnText = GameObject.Find("Respawn Text");

        float respawnTime = 5.0f;

        while (respawnTime > 0)
        {
            transform.GetComponent<PlayerMovementController>().enabled = false;
            respawnText.GetComponent<Text>().text = "You are killed. Respawning in " + respawnTime.ToString(".00");

            yield return new WaitForSeconds(1.0f);
            respawnTime--;
        }

        animator.SetBool("isDead", false);
        respawnText.GetComponent<Text>().text = "";

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
        GameObject killFeed = GameObject.Find("Kill Feed");

        if(photonView.IsMine)
        {
            killFeed.GetComponent<Text>().text = info.Sender.NickName + " killed " + info.photonView.Owner.NickName;
        }
        else
        {
            killFeed.GetComponent<Text>().text = "You killed " + info.photonView.Owner.NickName;
        }

        StartCoroutine(ShowKillTimer());
    }

    [PunRPC]
    IEnumerator ShowKillTimer()
    {
        GameObject killFeed = GameObject.Find("Kill Feed");
        float showKillTimer = 3.0f;

        while (showKillTimer > 0)
        {
            yield return new WaitForSeconds(1.0f);
            showKillTimer--;
        }

        killFeed.GetComponent<Text>().text = "";
    }

    [PunRPC]
    public void LosePoints(int point, PhotonMessageInfo info)
    {
        this.startingPoints -= point;
        Debug.Log(info.Sender.NickName + " has " + this.startingPoints + " points");
    }

    public void GameOver()
    {
        GameObject gameOverText = GameObject.Find("Game Over Text");
        gameOverText.GetComponent<Text>().text = "Game over! You lost!";

        animator.SetBool("isDead", true);
        transform.GetComponent<PlayerMovementController>().enabled = false;
    }
}