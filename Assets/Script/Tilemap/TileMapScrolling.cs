using UnityEngine;

public class TilemapScrollDown : MonoBehaviour
{
    public float scrollSpeed = 2f; // tốc độ cuộn (units/giây)
    private Vector3 startPos;      // vị trí ban đầu

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Di chuyển tilemap xuống
        transform.position += Vector3.down * scrollSpeed * Time.deltaTime;

        // Nếu muốn lặp lại (hiệu ứng nền vô tận)
        // có thể thêm đoạn wrap-around như bên dưới
        if (transform.position.y < -10f)
        {
            transform.position = startPos;
        }
    }
}
