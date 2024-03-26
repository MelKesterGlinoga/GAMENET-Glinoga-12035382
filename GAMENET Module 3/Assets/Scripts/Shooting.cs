using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Shooting : MonoBehaviourPunCallbacks
{
    [Header("Laser settings")]
    public int maxAmmo = 1000;
    private int currentAmmo = 0;
    public TextMeshProUGUI laserCooldownText;

    [Header("Missile settings")]
    public int maxMissile = 3;
    private int currentMissile = 0;
    public TextMeshProUGUI missileReloadText;

    [Header("Game objects initialization")]
    public GameObject hitEffectPrefab;
    public GameObject missilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = maxAmmo;
        laserCooldownText.text = "Laser ready!";
        laserCooldownText.color = Color.green;

        currentMissile = maxMissile;
        missileReloadText.text = "Missiles ready!";
        missileReloadText.color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && currentAmmo != 0)
        {
            currentAmmo--;
            GameObject muzzle = GameObject.Find("Muzzle");
            Ray ray = new Ray(muzzle.transform.position, muzzle.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 200))
            {
                //Debug.Log(hit.collider.gameObject.name);
                photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point);
                if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 1);
                }
            }
            CheckAmmo();
        }

        if (Input.GetButtonDown("Fire2") && currentMissile !=0)
        {
            currentMissile--;
            GameObject missilePod = GameObject.Find("MissilePod");
            photonView.RPC("LaunchMissile", RpcTarget.All, missilePod.transform.position, missilePod.transform.rotation);
            
            CheckMissile();
        }
    }

    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
        GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGameObject, 0.2f);
    }

    [PunRPC]
    public void LaunchMissile(Vector3 position, Quaternion rotation)
    {
        GameObject missileGameObject = Instantiate(missilePrefab, position, rotation);
        Destroy(missileGameObject, 4f);
    }

    public void CheckAmmo()
    {
        if (currentAmmo <= 0)
        {
            StartCoroutine(WaitForReloadAmmo());
        }
    }

    public void CheckMissile()
    {
        if (currentMissile <= 0)
        {
            StartCoroutine(WaitForReloadMissile());
        }
    }

    IEnumerator WaitForReloadAmmo()
    {
        float reloadTime = 5.0f;

        while (reloadTime > 0)
        {
            laserCooldownText.text = "Cooling down!";
            laserCooldownText.color = Color.red;

            yield return new WaitForSeconds(1.0f);
            reloadTime--;
        }
        currentAmmo = maxAmmo;
        laserCooldownText.text = "Laser ready!";
        laserCooldownText.color = Color.green;
    }

    IEnumerator WaitForReloadMissile()
    {
        float reloadTime = 5.0f;

        while (reloadTime > 0)
        {
            missileReloadText.text = "Reloading...";
            missileReloadText.color = Color.red;

            yield return new WaitForSeconds(1.0f);
            reloadTime--;
        }
        currentMissile = maxMissile;
        missileReloadText.text = "Missile ready!";
        missileReloadText.color = Color.green;
    }
}
