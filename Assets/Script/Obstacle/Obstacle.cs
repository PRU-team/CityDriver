using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float destroyY = -6f;

    [Header("Effects")]
    public GameObject explosionEffect;   // hiệu ứng nổ (nếu có)
    public AudioClip hitSound;           // âm thanh va chạm (nếu có)

    private AudioSource audioSource;

    void Start()
    {
        // Tạo AudioSource nếu cần
        if (hitSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip = hitSound;
        }
    }

    void Update()
    {
        // Di chuyển obstacle xuống
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        // Xoá khi ra khỏi màn hình
        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Hiệu ứng nổ
            if (explosionEffect != null)
                Instantiate(explosionEffect, transform.position, Quaternion.identity);

            // Âm thanh va chạm
            if (audioSource != null)
                audioSource.Play();

            // Gọi GameManager để xử lý game over
            GameManager.Instance.GameOver();

            // Xoá obstacle (sau một chút delay nếu có âm thanh)
            Destroy(gameObject);
        }
    }

    // Cho phép thiết lập tốc độ từ script ngoài
    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }
}
