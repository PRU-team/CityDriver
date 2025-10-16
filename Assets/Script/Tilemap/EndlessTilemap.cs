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
    [Header("Scene Mode")]
    [Tooltip("If true and there are child GameObjects under this transform, the script will reuse those children as chunks instead of instantiating prefabs.")]
    public bool preferSceneChildren = true;

    // internal flag used at runtime to know which mode we're in
    private bool useSceneChildren = false;
    [Header("Auto-detect")]
    [Tooltip("If true, when using scene children the script will try to compute tileLength automatically from child bounds.")]
    public bool autoDetectTileLength = true;
    [Tooltip("Minimum detected tile length (world units) to avoid zero sizes if bounds can't be determined.")]
    public float minDetectedTileLength = 0.1f;
    [Header("Collider")]
    [Tooltip("If true, when a scene-child chunk is moved during recycle the script will briefly toggle any TilemapCollider2D components to force the physics shapes to update.")]
    public bool refreshCollidersOnRecycle = true;

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

        // If developer wants to reuse child GameObjects placed in the Scene, prefer that mode.
        if (preferSceneChildren && transform.childCount > 0)
        {
            useSceneChildren = true;
            int count = Mathf.Min(transform.childCount, initialCount);
            // If requested, try to auto-detect tileLength from child bounds before positioning
            if (autoDetectTileLength)
            {
                float detected = ComputeTileLengthFromChildren();
                if (detected > 0f)
                {
                    tileLength = Mathf.Max(detected, minDetectedTileLength);
                    Debug.Log($"EndlessTilemap: auto-detected tileLength = {tileLength}", this);
                }
            }

            for (int i = 0; i < count; i++)
            {
                var child = transform.GetChild(i).gameObject;
                Vector3 pos = transform.position - scrollDirection * tileLength * i;
                child.transform.position = pos;
                pieces.Add(child);
            }
            Debug.Log("EndlessTilemap: using existing children as chunks (scene mode)", this);
            return;
        }

        // Otherwise fall back to spawning prefabs if available
        if (!HasValidPrefabs())
        {
            Debug.LogError("EndlessTilemap: tilePrefabs is empty or missing and no scene children found. Assign chunk prefabs or add child chunks in the Scene.", this);
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

        // If we're reusing scene children, just move the first child to the spawn position and re-add it.
        if (useSceneChildren)
        {
            if (first != null)
            {
                first.transform.position = spawnPos;
                // Optionally refresh TilemapCollider2D components in the chunk so physics shapes update
                if (refreshCollidersOnRecycle)
                {
                    var colliders = first.GetComponentsInChildren<UnityEngine.Tilemaps.TilemapCollider2D>(true);
                    foreach (var c in colliders)
                    {
                        // toggle enabled to force physics update
                        c.enabled = false;
                    }
                    foreach (var c in colliders)
                    {
                        c.enabled = true;
                    }
                }
                pieces.Add(first);
            }
            return;
        }

        // Otherwise, if we have prefabs, destroy the old and instantiate a new random prefab
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

        // Last-resort fallback: reuse first object by moving it
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

    // Try to estimate the length of a chunk (world units) along scrollDirection by
    // computing bounds that encompass all child renderers / tilemaps and projecting
    // along the scroll axis. Returns 0 if can't determine.
    private float ComputeTileLengthFromChildren()
    {
        if (transform.childCount == 0) return 0f;

        Bounds? bounds = null;
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            // prefer Tilemap bounds
            var tm = child.GetComponentInChildren<UnityEngine.Tilemaps.Tilemap>(true);
            if (tm != null)
            {
                var b = tm.localBounds;
                // localBounds is in local space; convert to world
                var worldMin = tm.transform.TransformPoint(b.min);
                var worldMax = tm.transform.TransformPoint(b.max);
                var wb = new Bounds((worldMin + worldMax) * 0.5f, worldMax - worldMin);
                if (bounds == null) bounds = wb; else bounds.Value.Encapsulate(wb);
                continue;
            }

            // fallback: renderer bounds
            var rend = child.GetComponentInChildren<Renderer>(true);
            if (rend != null)
            {
                if (bounds == null) bounds = rend.bounds; else bounds.Value.Encapsulate(rend.bounds);
            }
        }

        if (bounds == null) return 0f;

        // project the bounds size onto the scrollDirection to get length along that axis
        Vector3 axis = scrollDirection.normalized;
        float projected = Mathf.Abs(Vector3.Dot(bounds.Value.size, axis));
        return projected;
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

