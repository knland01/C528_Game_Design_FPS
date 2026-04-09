using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject innocentPrefab;
    public GameObject sheriffPrefab;

    [Header("Spawn Area")]
    public Vector3 center;
    public Vector3 size;

    [Header("Spawn Settings")]
    //public int maxEnemies = 10;
    public float spawnInterval = 3f;

    //private int currentEnemies = 0;

    void Start()
    {
        StartCoroutine(SpawnSheriffWithDelay());
        InvokeRepeating(nameof(SpawnCharacter), 1f, spawnInterval);
    }

    // Sheriff spawn Coroutine
    IEnumerator SpawnSheriffWithDelay()
    {
        float delay = Random.Range(2f, 10f); // random time window
        yield return new WaitForSeconds(delay);

        Vector3 spawnPos = GetRandomPosition();
        Instantiate(sheriffPrefab, spawnPos, Quaternion.identity);
    }

    void SpawnCharacter()
    {
        //Debug.Log("Spawning attempt");

        Vector3 spawnPos = GetRandomPosition();

        // 50/50 split (you can tweak this later)
        bool spawnEnemy = Random.value < 0.5f;

        GameObject prefabToSpawn = spawnEnemy ? enemyPrefab : innocentPrefab;

        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        Vector3 startPos = new Vector3(x, center.y + 20f, z); // start above ground

        // Find the ground and spawn there (instead of specific height)
        RaycastHit hit;
        if (Physics.Raycast(startPos, Vector3.down, out hit, 50f))
        {
            Debug.Log("ground hit");
            return hit.point;
        }

        // fallback (in case no ground hit)
        return new Vector3(x, center.y, z);
    }
}