using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple endless tilemap spawner.
/// Assign an array of tile chunk prefabs (each chunk is a GameObject containing Tilemap(s) or sprites).
/// The script will instantiate `initialCount` pieces stacked along the negative scroll direction,
/// move them each frame by `scrollSpeed` (set by GameManager), and recycle the oldest piece to the end
/// choosing a random prefab from the list.
///
/// Notes:
/// - Set `tileLength` to match the size (in world units) of your tile chunk along the scroll direction.
/// - `scrollDirection` defaults to down (y-). For an endless runner where tiles move down, use Vector3.down.
/// </summary>
public class EndlessTilemap : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] tilePrefabs; // assign chunk prefabs in inspector

    [Header("Spawn")]
    public int initialCount = 5; // how many chunks to spawn initially
    public float tileLength = 10f; // size of each chunk along the scroll direction (world units)
    public Vector3 scrollDirection = Vector3.down; // direction tiles move each frame

    [Header("Runtime")]
    public float scrollSpeed = 0f; // controlled externally (e.g. GameManager)

    // internal pool
    private readonly List<GameObject> pieces = new List<GameObject>();

    void Start()
    {
        scrollDirection = scrollDirection.normalized;

        if (!HasValidPrefabs())
        {
            // If no prefabs assigned, try to use existing children as starting pieces (safe fallback).
            if (transform.childCount > 0)
            {
                for (int i = 0; i < transform.childCount && i < initialCount; i++)
                {
                    var child = transform.GetChild(i).gameObject;
                    Vector3 pos = transform.position - scrollDirection * tileLength * i;
                    child.transform.position = pos;
                    pieces.Add(child);
                }
                Debug.LogWarning("EndlessTilemap: no tilePrefabs assigned. Using existing children as chunks. Assign prefabs in the Inspector to enable random spawning.", this);
            }
            else
            {
                Debug.LogError("EndlessTilemap: tilePrefabs is empty or missing. Please assign chunk prefabs in the Inspector (Tile Prefabs) for EndlessTilemap to spawn map pieces.", this);
            }

            return;
        }

        // spawn initial chunks stacked opposite to scroll direction so they move past origin
        for (int i = 0; i < initialCount; i++)
        {
            Vector3 pos = transform.position - scrollDirection * tileLength * i;
            SpawnAt(pos);
        }
    }

    void Update()
    {
        if (pieces.Count == 0) return;

        // move pieces
        Vector3 delta = scrollDirection * scrollSpeed * Time.deltaTime;
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i] != null)
                pieces[i].transform.position += delta;
        }

        // check if the first piece moved past the despawn line relative to this.transform.position
        GameObject first = pieces[0];
        float dot = Vector3.Dot(first.transform.position - transform.position, scrollDirection);
        // when dot < -tileLength we consider it passed beyond the origin by one tile
        if (dot < -tileLength)
        {
            RecycleFirstToLast();
        }
    }

    private GameObject SpawnAt(Vector3 pos)
    {
        if (!HasValidPrefabs())
        {
            Debug.LogWarning("EndlessTilemap.SpawnAt called but tilePrefabs is empty. Skipping spawn.", this);
            return null;
        }

        GameObject prefab = tilePrefabs[Random.Range(0, tilePrefabs.Length)];
        if (prefab == null)
        {
            Debug.LogWarning("EndlessTilemap: selected prefab is null. Skipping spawn.", this);
            return null;
        }

        GameObject go = Instantiate(prefab, pos, Quaternion.identity, transform);
        pieces.Add(go);
        return go;
    }

    private void RecycleFirstToLast()
    {
        if (pieces.Count == 0) return;

        GameObject first = pieces[0];
        pieces.RemoveAt(0);

        Vector3 spawnPos = pieces.Count > 0 ? pieces[pieces.Count - 1].transform.position - scrollDirection * tileLength : transform.position - scrollDirection * tileLength;

        if (HasValidPrefabs())
        {
            GameObject prefab = tilePrefabs[Random.Range(0, tilePrefabs.Length)];
            if (prefab != null)
            {
                Destroy(first);
                GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity, transform);
                pieces.Add(go);
                return;
            }
        }

        // Fallback: reuse the first object by moving it to the spawn position and adding back to list
        if (first != null)
        {
            first.transform.position = spawnPos;
            pieces.Add(first);
        }
    }

    private bool HasValidPrefabs()
    {
        return tilePrefabs != null && tilePrefabs.Length > 0;
    }

    // Optional: allow external callers to force a reset
    public void ResetMap()
    {
        foreach (var p in pieces)
            if (p != null) Destroy(p);
        pieces.Clear();

        for (int i = 0; i < initialCount; i++)
        {
            Vector3 pos = transform.position - scrollDirection * tileLength * i;
            SpawnAt(pos);
        }
    }
}

