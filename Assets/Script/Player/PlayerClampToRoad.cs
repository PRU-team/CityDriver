using UnityEngine;

// Gắn script này vào Player. Set minX/maxX (và minY/maxY nếu cần) trong Inspector.
public class PlayerClampToRoad : MonoBehaviour
{
    [Header("Clamp X")]
    public bool clampX = true;
    public float minX = -3f;
    public float maxX = 3f;

    [Header("Clamp Y (optional)")]
    public bool clampY = false;
    public float minY = -10f;
    public float maxY = 10f;

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        if (clampX)
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
        if (clampY)
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }
}
