using UnityEngine;

public class RoadObjectSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] obstaclePrefabs;
    public GameObject[] itemPrefabs;

    [Header("Spawn Settings")]
    public float spawnInterval = 1.5f;
    public float spawnY = 6f; // vị trí spawn (trên cùng màn hình)

    [Header("Grid Settings")]
    public int numberOfLanes = 4;   // số làn (4 grid)
    public float laneWidth = 1f;    // mỗi làn cách nhau bao nhiêu đơn vị grid

    private float[] spawnLanes;     // mảng lưu tọa độ X trung tâm mỗi làn

    void Start()
    {

        spawnLanes = new float[numberOfLanes];
        float startX = -((numberOfLanes - 1) / 2f) * laneWidth;

        for (int i = 0; i < numberOfLanes; i++)
        {
            spawnLanes[i] = startX + i * laneWidth;
        }


        InvokeRepeating(nameof(SpawnObjects), 1f, spawnInterval);
    }

    void SpawnObjects()
    {

        float laneX = spawnLanes[Random.Range(0, spawnLanes.Length)];

        bool spawnObstacle = Random.value > 0.3f;
        GameObject prefab = spawnObstacle
            ? obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)]
            : itemPrefabs[Random.Range(0, itemPrefabs.Length)];

        Vector3 spawnPos = new Vector3(laneX, spawnY, 0);
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        float startX = -((numberOfLanes - 1) / 2f) * laneWidth;

        for (int i = 0; i < numberOfLanes; i++)
        {
            float x = startX + i * laneWidth;
            Gizmos.DrawLine(new Vector3(x, spawnY - 0.5f, 0), new Vector3(x, spawnY + 0.5f, 0));
        }
    }
}
