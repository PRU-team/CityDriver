using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Bake one or more Tilemap layers under this GameObject into a single Mesh for performance.
// NOTE: This is a simple baker which assumes tiles use sprites from the same texture atlas.
// It is intended for static backgrounds where tiles don't change at runtime.
[DisallowMultipleComponent]
public class ChunkMeshGenerator : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Only Tilemaps whose Tile/Renderer sprite uses this texture will be included.")]
    public Texture2D atlasTexture;

    [Tooltip("If true, generate a PolygonCollider2D matching opaque pixels (slow). Otherwise no collider created.")]
    public bool generateCollider = false;

    [Tooltip("Cell size used to place quads. Usually matches Tilemap.cellSize.x/y.")]
    public Vector2 cellSize = new Vector2(1f, 1f);

    [Header("Output")]
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    [Tooltip("Optional: assign a pre-made material that uses your atlas. If set, it will be used instead of creating a new material.")]
    public Material atlasMaterial;

    // Public API to create or update mesh
    public void GenerateMesh()
    {
        var tilemaps = GetComponentsInChildren<Tilemap>(true);
        if (tilemaps == null || tilemaps.Length == 0)
        {
            Debug.LogWarning("ChunkMeshGenerator: no Tilemap children found.", this);
            return;
        }

        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        int vertOffset = 0;
        foreach (var tm in tilemaps)
        {
            var bounds = tm.cellBounds;
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    var pos = new Vector3Int(x, y, 0);
                    var tile = tm.GetTile(pos);
                    if (tile == null) continue;

                    // try get sprite from TileBase
                    Sprite s = null;
                    var t = tile as Tile;
                    if (t != null) s = t.sprite;

                    if (s == null) continue;
                    if (atlasTexture != null && s.texture != atlasTexture) continue; // skip different atlas

                    Rect uvRect = s.textureRect;
                    Vector2 atlasSize = new Vector2(s.texture.width, s.texture.height);
                    Rect uv = new Rect(uvRect.x / atlasSize.x, uvRect.y / atlasSize.y, uvRect.width / atlasSize.x, uvRect.height / atlasSize.y);

                    // quad corners in local space, anchored at tile origin (cell aligned)
                    float px = x * cellSize.x;
                    float py = y * cellSize.y;
                    verts.Add(new Vector3(px, py, 0));
                    verts.Add(new Vector3(px + cellSize.x, py, 0));
                    verts.Add(new Vector3(px + cellSize.x, py + cellSize.y, 0));
                    verts.Add(new Vector3(px, py + cellSize.y, 0));

                    uvs.Add(new Vector2(uv.xMin, uv.yMin));
                    uvs.Add(new Vector2(uv.xMax, uv.yMin));
                    uvs.Add(new Vector2(uv.xMax, uv.yMax));
                    uvs.Add(new Vector2(uv.xMin, uv.yMax));

                    tris.Add(vertOffset + 0);
                    tris.Add(vertOffset + 2);
                    tris.Add(vertOffset + 1);
                    tris.Add(vertOffset + 0);
                    tris.Add(vertOffset + 3);
                    tris.Add(vertOffset + 2);

                    vertOffset += 4;
                }
            }
        }

        if (meshFilter == null) meshFilter = gameObject.GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();
        if (meshRenderer == null) meshRenderer = gameObject.GetComponent<MeshRenderer>() ?? gameObject.AddComponent<MeshRenderer>();

        var mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;

        // assign material: prefer user-supplied atlasMaterial, otherwise create one from atlasTexture
        if (atlasMaterial != null)
        {
            meshRenderer.sharedMaterial = atlasMaterial;
        }
        else if (atlasTexture != null)
        {
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.mainTexture = atlasTexture;
            meshRenderer.sharedMaterial = mat;
        }

        Debug.Log($"ChunkMeshGenerator: generated mesh with {verts.Count/4} tiles.", this);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ChunkMeshGenerator))]
public class ChunkMeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var t = target as ChunkMeshGenerator;
        if (GUILayout.Button("Generate Mesh from Tilemaps"))
        {
            Undo.RegisterCompleteObjectUndo(t.gameObject, "Generate Chunk Mesh");
            t.GenerateMesh();
            EditorUtility.SetDirty(t);
        }
        if (GUILayout.Button("Hide Tilemap Renderers"))
        {
            HideTilemapRenderers(t, true);
        }
        if (GUILayout.Button("Show Tilemap Renderers"))
        {
            HideTilemapRenderers(t, false);
        }
    }

    private void HideTilemapRenderers(ChunkMeshGenerator t, bool hide)
    {
        var tms = t.GetComponentsInChildren<TilemapRenderer>(true);
        foreach (var r in tms)
        {
            Undo.RecordObject(r, hide ? "Hide TilemapRenderer" : "Show TilemapRenderer");
            r.enabled = !hide ? true : false;
            EditorUtility.SetDirty(r);
        }
    }
}
#endif
