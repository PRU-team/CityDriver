using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Heart UI")]
    public TextMeshProUGUI heartText;   // Text hiển thị số tim
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Coin UI")]
    public TextMeshProUGUI coinText;    // Text hiển thị số coin

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateHearts(GameManager.Instance.GetCurrentHearts());
        UpdateCoins(GameManager.Instance.GetCoins());
    }

    // 🩸 Cập nhật hiển thị tim (số hoặc icon)
    public void UpdateHearts(int hearts)
    {
        if (heartText != null)
            heartText.text = $" {hearts}";
    }

    // 🪙 Cập nhật hiển thị coin
    public void UpdateCoins(int coins)
    {
        if (coinText != null)
            coinText.text = coins.ToString();
    }
}
