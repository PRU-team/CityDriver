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

    [Header("Health Settings")]
    public int maxHearts = 3;
    private int currentHearts;

    [Header("Runtime")]
    public bool isPlaying = true;
    public bool isPaused = false; // Thêm pause state
    private float currentScrollSpeed;
    private float score = 0f;

    private int coins = 0;

    [Header("Player Protection")]
    public bool isShieldActive = false;
    private float shieldTimer = 0f;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Tự tìm các controller nếu chưa gán
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
    public void InitializeGame()
    {
        currentHearts = maxHearts;
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateHearts(currentHearts);
    }


    void Start()
    {
        currentScrollSpeed = baseScrollSpeed;
        currentHearts = maxHearts;
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateHearts(currentHearts);

        if (tilemapController != null)
            tilemapController.scrollSpeed = currentScrollSpeed;
        else
            Debug.LogWarning("GameManager.Start: tilemapController is not assigned. Assign it in the Inspector or ensure a TilemapScrollDown exists in scene.", this);
    }

    void Update()
    {
        if (!isPlaying) return;

        // Giữ tốc độ cuộn đồng bộ
        currentScrollSpeed = baseScrollSpeed;
        if (tilemapController != null)
            tilemapController.scrollSpeed = currentScrollSpeed;
        else
            Debug.LogWarning("GameManager.Update: tilemapController is null. Game will not scroll. Assign the TilemapScrollDown reference in the Inspector.", this);

        UpdateScore();
        UpdateShieldTimer(); 
    }

    public void PlayerHit()
    {
        if (!isPlaying) return;

        currentHearts--;

        Debug.Log($" Player bị va chạm! Còn lại {currentHearts}/{maxHearts} tim.");

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateHearts(currentHearts);

        if (currentHearts <= 0)
        {
            GameOver();
        }
    }

    public int GetCurrentHearts()
    {
        return currentHearts;
    }

    public void AddHeart(int amount = 1)
    {
        currentHearts = Mathf.Min(maxHearts, currentHearts + amount);
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateHearts(currentHearts);
    }
    public void AddCoin(int amount = 1)
    {
        coins += amount;
        Debug.Log($"Player nhặt được coin! Tổng: {coins}");
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateCoins(coins);
    }

    public int GetCoins()
    {
        return coins;
    }


    public void ModifyBaseScrollSpeed(float delta)
    {
        baseScrollSpeed = Mathf.Max(0f, baseScrollSpeed + delta);
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
    
    public void ActivateShield(float duration)
    {
        isShieldActive = true;
        shieldTimer = duration;

        Debug.Log($" Shield (Fuel) activated for {duration} seconds!");

        // Nếu có UI thì có thể bật icon shield ở đây:
        //if (UIManager.Instance != null)
        //    UIManager.Instance.ShowShield(true);
    }
    
    private void UpdateShieldTimer()
    {
        if (isShieldActive)
        {
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0)
            {
                isShieldActive = false;
                Debug.Log(" Shield expired!");

                //if (UIManager.Instance != null)
                //    UIManager.Instance.ShowShield(false);
            }
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
        
        Debug.Log($" GAME OVER! Final Score: {Mathf.FloorToInt(score)}");
        
        // Show Game Over UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver(score);
        }
    }
    
    /// <summary>
    /// Pause game - stop scrolling but keep objects active
    /// </summary>
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Debug.Log("GameManager: Game paused");
    }
    
    /// <summary>
    /// Resume game - restore normal time and scrolling
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Debug.Log("GameManager: Game resumed");
    }

    public float GetScore()
    {
        return score;
    }
}
