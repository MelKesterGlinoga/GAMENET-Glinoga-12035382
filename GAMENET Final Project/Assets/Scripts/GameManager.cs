using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject floorTile;
    public GameObject deathTrigger;

    private int initialTiles = 5;

    Vector3 nextAttachPoint;
    Vector3 nextSpawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            int randomPointX = Random.Range(-10, 10);
            int randomPointZ = Random.Range(-10, 10);

            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPointX, 0, randomPointZ), Quaternion.identity);

            for (int i = 0; i < initialTiles; i++)
            {
                SpawnTile();
                SpawnDeathTrigger();
            }
        }
    }

    public void SpawnTile()
    {
        GameObject temp = Instantiate(floorTile, nextAttachPoint, Quaternion.identity);
        nextAttachPoint = temp.transform.GetChild(2).transform.position;
    }

    public void SpawnDeathTrigger()
    {
        GameObject tem = Instantiate(deathTrigger, nextSpawnPoint, Quaternion.identity);
        DestroyDeathTrig(tem);
        nextSpawnPoint = tem.transform.GetChild(0).transform.position;
    }

    public void DestroyDeathTrig(GameObject go)
    {
        Destroy(go, 300);
    }
}
