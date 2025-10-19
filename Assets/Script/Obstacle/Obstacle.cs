using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float destroyY = -6f;

    [Header("Effects")]
    public GameObject explosionEffect;
    public AudioClip hitSound;

    private AudioSource audioSource;
    private bool hasHit = false;

    void Start()
    {
        if (hitSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip = hitSound;
        }
    }

    void Update()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
        if (transform.position.y < destroyY)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;
        if (!GameManager.Instance.isPlaying) return;

        if (other.CompareTag("Player"))
        {
            hasHit = true;
            if (GameManager.Instance.isShieldActive)
            {
                GameManager.Instance.isShieldActive = false; // tắt shield
                Debug.Log("💥 Obstacle hit absorbed by shield!");
                Destroy(gameObject); // phá obstacle
                return;
            }


            if (explosionEffect != null)
                Instantiate(explosionEffect, transform.position, Quaternion.identity);

            if (audioSource != null)
                audioSource.Play();

            //  Gọi GameManager trừ tim
            if (GameManager.Instance != null)
                GameManager.Instance.PlayerHit();

            Destroy(gameObject, 0.05f);
        }
    }

    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }
}
