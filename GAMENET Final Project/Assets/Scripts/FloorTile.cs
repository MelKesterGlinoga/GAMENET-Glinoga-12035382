using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : MonoBehaviour
{
    GameManager gameManager;

    public Collider spawnPos;
    public GameObject obstaclePrefab;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();

        //SpawnObstacle(spawnPos);
        float xPos = Random.Range(spawnPos.bounds.min.x, spawnPos.bounds.max.x);
        float zPos = Random.Range(spawnPos.bounds.min.z, spawnPos.bounds.max.z);
        Instantiate(obstaclePrefab, new Vector3(xPos, 0, zPos), Quaternion.identity, this.gameObject.transform);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerExit(Collider collider)
    {
        gameManager.SpawnTile();
        Destroy(gameObject, 2);
    }

    public void SpawnObstacle(Collider spawnPos)
    {
        float xPos = Random.Range(spawnPos.bounds.min.x, spawnPos.bounds.max.x);
        float zPos = Random.Range(spawnPos.bounds.min.z, spawnPos.bounds.max.z);
        GameObject obstac = Instantiate(obstaclePrefab, new Vector3(xPos, 0, zPos), Quaternion.identity);
        Destroy(obstac, 10);
    }
}
