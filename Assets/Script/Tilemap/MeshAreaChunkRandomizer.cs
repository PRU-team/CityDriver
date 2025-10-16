using System.Collections.Generic;
using UnityEngine;

// Attach this to a GameObject that has a MeshFilter/MeshRenderer (e.g. a Quad).
// It will place instances of chunk prefabs (Tilemap GameObjects) as children
// inside the area defined by the mesh renderer bounds. This is intentionally
// simple: it samples positions inside the axis-aligned bounds of the mesh.
[ExecuteAlways]
public class MeshAreaChunkRandomizer : MonoBehaviour
{
    [Tooltip("MeshRenderer used to define the area. If not set, uses this GameObject's MeshRenderer.")]
    public MeshRenderer areaRenderer;

    [Tooltip("Chunk prefabs to place (should be GameObjects containing Tilemap/TilemapRenderer etc).")]
    public List<GameObject> chunkPrefabs = new List<GameObject>();

    [Tooltip("Number of chunks to spawn (will attempt to place this many).")]
    public int spawnCount = 4;

    [Tooltip("Minimum cell-aligned spacing (in world units) between placed chunk origins. Setting to 0 disables collision checks.")]
    public float minSpacing = 0.5f;

    [Tooltip("If true, instantiated chunks will be parented to this GameObject.")]
    public bool parentToArea = true;

    [Tooltip("Random rotation (Z axis) applied to each spawned chunk prefab.")]
    public bool randomRotation = false;

    [Tooltip("Seed for deterministic placement. Set to 0 for time-based randomness.")]
    public int randomSeed = 0;

    [Tooltip("Whether to clear previously spawned children created by this script when placing new chunks.")]
    public bool clearExistingGeneratedChildren = true;

    // Keep track of generated instances so ClearGenerated only removes them
    [HideInInspector]
    public List<GameObject> generatedInstances = new List<GameObject>();

    // Public API: clear generated children created previously
    public void ClearGenerated()
    {
        for (int i = generatedInstances.Count - 1; i >= 0; i--)
        {
            var go = generatedInstances[i];
            if (go != null)
            {
                #if UNITY_EDITOR
                if (!Application.isPlaying) UnityEditor.Undo.DestroyObjectImmediate(go);
                else
                #endif
                    DestroyImmediate(go);
            }
        }
        generatedInstances.Clear();
    }

    // Public API: place random chunks inside the area
    public void PlaceChunks()
    {
        if (areaRenderer == null) areaRenderer = GetComponent<MeshRenderer>();
        if (areaRenderer == null)
        {
            Debug.LogWarning("MeshAreaChunkRandomizer: no MeshRenderer found to define area.", this);
            return;
        }

        if (chunkPrefabs == null || chunkPrefabs.Count == 0)
        {
            Debug.LogWarning("MeshAreaChunkRandomizer: no chunkPrefabs assigned.", this);
            return;
        }

        if (clearExistingGeneratedChildren)
            ClearGenerated();

        var bounds = areaRenderer.bounds; // world-space AABB

        System.Random rng = (randomSeed != 0) ? new System.Random(randomSeed) : new System.Random(System.Environment.TickCount ^ this.GetInstanceID());

        int attempts = 0;
        int placed = 0;
        int maxAttempts = spawnCount * 10 + 50;

        List<Vector3> placedPositions = new List<Vector3>();

        while (placed < spawnCount && attempts < maxAttempts)
        {
            attempts++;

            float rx = (float)(rng.NextDouble() * (bounds.max.x - bounds.min.x) + bounds.min.x);
            float ry = (float)(rng.NextDouble() * (bounds.max.y - bounds.min.y) + bounds.min.y);
            Vector3 worldPos = new Vector3(rx, ry, bounds.center.z);

            // spacing check
            bool ok = true;
            if (minSpacing > 0f)
            {
                foreach (var p in placedPositions)
                {
                    if (Vector3.Distance(p, worldPos) < minSpacing) { ok = false; break; }
                }
            }

            if (!ok) continue;

            // choose prefab
            var prefab = chunkPrefabs[rng.Next(0, chunkPrefabs.Count)];
            if (prefab == null) continue;

            // instantiate
            GameObject inst;
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                inst = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab);
            }
            else
            #endif
            {
                inst = Instantiate(prefab);
            }

            if (inst == null) continue;

            // set position and parent
            inst.transform.position = worldPos;
            if (parentToArea)
            {
                if (Application.isPlaying) inst.transform.SetParent(this.transform, true);
                else
                {
                    #if UNITY_EDITOR
                    UnityEditor.Undo.SetTransformParent(inst.transform, this.transform, "Parent generated chunk");
                    #else
                    inst.transform.SetParent(this.transform, true);
                    #endif
                }
            }

            if (randomRotation)
            {
                float angle = (float)(rng.NextDouble() * 360.0);
                inst.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }

            generatedInstances.Add(inst);
            placedPositions.Add(worldPos);
            placed++;
        }

        Debug.Log($"MeshAreaChunkRandomizer: placed {placed} chunks (attempts={attempts}).", this);
    }

    // Convenience context menu for editor
    [ContextMenu("Place Chunks (Editor)")]
    private void ContextPlaceChunks() { PlaceChunks(); }

    [ContextMenu("Clear Generated Chunks (Editor)")]
    private void ContextClear() { ClearGenerated(); }
}
