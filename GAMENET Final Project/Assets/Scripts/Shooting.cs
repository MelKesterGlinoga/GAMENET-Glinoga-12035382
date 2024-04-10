using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Shooting : MonoBehaviour
{
    [SerializeField]
    Camera fpsCamera;

    [SerializeField]
    private int currentAmmo;
    public int maxAmmo = 1;

    public TextMeshProUGUI cooldownText;

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = maxAmmo;
        cooldownText.text = "Freeze gun ready!";
        cooldownText.color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && currentAmmo != 0)
        {
            currentAmmo--; 
            Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.Log(hit.collider.gameObject.name);
                if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    hit.collider.gameObject.GetComponent<PhotonView>().RPC("StunPlayer", RpcTarget.AllBuffered, 1);
                }
            }
            GunCooldown();
        }
    }

    public void GunCooldown()
    {
        if (currentAmmo <= 0)
        {
            StartCoroutine(WaitForCooldown());
        }
    }

    IEnumerator WaitForCooldown()
    {
        float cdTime = 10.0f;

        while (cdTime > 0)
        {
            cooldownText.text = "Cooling down!";
            cooldownText.color = Color.red;

            yield return new WaitForSeconds(1.0f);
            cdTime--;
        }
        currentAmmo = maxAmmo;
        cooldownText.text = "Freeze gun ready!";
        cooldownText.color = Color.green;
    }
}
