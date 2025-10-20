using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public TilemapScrollDown tilemapController;
    public CarController player;

    [Header("Game Settings")]
    public float baseScrollSpeed = 5f;
    public float speedBoostMultiplier = 1.5f;
    public float scoreMultiplier = 1f;

    [Header("Runtime")]
    public bool isPlaying = true;
    private float currentScrollSpeed;
    private float score = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        // Try to auto-assign controllers if developer forgot to set them in the Inspector
        if (tilemapController == null)
        {
            tilemapController = FindObjectOfType<TilemapScrollDown>();
            if (tilemapController != null)
                Debug.Log("GameManager: auto-assigned TilemapScrollDown reference.", this);
        }
        if (player == null)
        {
            player = FindObjectOfType<CarController>();
            if (player != null)
                Debug.Log("GameManager: auto-assigned CarController reference.", this);
        }
    }

    void Start()
    {
        currentScrollSpeed = baseScrollSpeed;
        if (tilemapController != null)
            tilemapController.scrollSpeed = currentScrollSpeed;
        else
            Debug.LogWarning("GameManager.Start: tilemapController is not assigned. Assign it in the Inspector or ensure a TilemapScrollDown exists in scene.", this);
    }

    void Update()
    {
        if (!isPlaying) return;

        // Keep current scroll speed synced with baseScrollSpeed.
        currentScrollSpeed = baseScrollSpeed;
        if (tilemapController != null)
            tilemapController.scrollSpeed = currentScrollSpeed;
        else
            Debug.LogWarning("GameManager.Update: tilemapController is null. Game will not scroll. Assign the TilemapScrollDown reference in the Inspector.", this);

        UpdateScore();
    }

    // Public method so other scripts (e.g. CarController) can adjust the game's base scroll speed.
    public void ModifyBaseScrollSpeed(float delta)
    {
        baseScrollSpeed = Mathf.Max(0f, baseScrollSpeed + delta);
        // immediately apply the change
        currentScrollSpeed = baseScrollSpeed;
        if (tilemapController != null)
            tilemapController.scrollSpeed = currentScrollSpeed;
    }

    void UpdateScore()
    {
        score += Time.deltaTime * currentScrollSpeed * scoreMultiplier;
        
        // Update UI if UIManager exists
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScore(score);
            UIManager.Instance.UpdateSpeed(currentScrollSpeed * 10f); // Convert to km/h display
        }
    }

    public void GameOver()
    {
        isPlaying = false;
        currentScrollSpeed = 0f;
        if (tilemapController != null)
            tilemapController.scrollSpeed = 0f;
        else
            Debug.LogWarning("GameManager.GameOver: tilemapController is null when trying to stop scrolling.", this);
        
        Debug.Log("Game Over! Final Score: " + Mathf.FloorToInt(score));
        
        // Show Game Over UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver(score);
        }
    }

    public float GetScore()
    {
        return score;
    }
}
