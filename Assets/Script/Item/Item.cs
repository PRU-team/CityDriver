using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float destroyY = -6f;

    [Header("Effects")]
    public GameObject explosionEffect;   // hiệu ứng nổ (nếu có)
    public AudioClip hitSound;           // âm thanh va chạm (nếu có)

    private AudioSource audioSource;

    [Header("Item Type")]
    public ItemType itemType; // chọn loại item trong Inspector
    public int value = 1;
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
            if (hitSound != null)
            {
                GameObject tempAudio = new GameObject("TempAudio");
                AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
                tempSource.clip = hitSound;
                tempSource.Play();
                Destroy(tempAudio, hitSound.length);
            }
            Debug.Log($"Item picked up: {itemType}");
            switch (itemType)
            {
                case ItemType.Heart:
                    GameManager.Instance.AddHeart(value);
                    break;
                case ItemType.Coin:
                    GameManager.Instance.AddCoin(value);
                    break;
                case ItemType.Fuel:
                    GameManager.Instance.ActivateShield(10f); // bật shield trong 10 giây
                    break;
            }

            // Xóa object (nếu muốn delay chút khi có âm thanh, có thể dùng Destroy(gameObject, 0.1f))
            Destroy(gameObject);
        }
    }

    // Cho phép thiết lập tốc độ từ script ngoài
    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }
    public enum ItemType
    {
        Heart,
        Coin,
        Fuel
    }
}
