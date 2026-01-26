using UnityEngine;
using UnityEngine.UI; // WAJIB ADA: Biar bisa akses Slider & Image

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Settings - Fun Meter")]
    public Slider funBarSlider;
    public float maxFun = 100f;
    public float currentFun = 0f;

    [Header("Settings - Heart System ❤️")]
    public int maxLives = 3;          // Jumlah nyawa maksimal
    private int currentLives;         // Nyawa saat ini

    // ARRAY UNTUK GAMBAR HATI
    // Nanti tarik gambar Heart_1, Heart_2, Heart_3 ke sini di Inspector
    public Image[] heartIcons;

    [Header("Settings - UI")]
    public GameObject gameOverPanel; // (Opsional) Panel kalah

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 1. Setup Fun Bar
        if (funBarSlider != null)
        {
            funBarSlider.maxValue = maxFun;
            funBarSlider.value = currentFun;
        }

        // 2. Setup Nyawa
        currentLives = maxLives;
        UpdateHeartUI(); // Tampilkan hati penuh saat mulai
    }

    // --- LOGIKA SKOR ---
    public void AddFun(float amount)
    {
        // Hanya nambah kalau nilainya positif (Buah)
        // Bom tidak lagi mengurangi fun, tapi mengurangi nyawa
        if (amount > 0)
        {
            currentFun += amount;
            currentFun = Mathf.Clamp(currentFun, 0, maxFun);

            if (funBarSlider != null) funBarSlider.value = currentFun;

            if (currentFun >= maxFun)
            {
                Debug.Log("MENANG! PENONTON SENANG!");
            }
        }
    }

    // --- LOGIKA NYAWA BARU ---
    public void ReduceLife()
    {
        // 1. Kurangi angka nyawa
        currentLives--;

        // 2. Update tampilan gambar hati
        UpdateHeartUI();

        Debug.Log("OUCH! Sisa Nyawa: " + currentLives);

        // 3. Cek Game Over
        if (currentLives <= 0)
        {
            TriggerGameOver();
        }
    }

    void UpdateHeartUI()
    {
        // Cek dulu biar gak error kalau lupa masukin gambar
        if (heartIcons == null) return;

        for (int i = 0; i < heartIcons.Length; i++)
        {
            // Jika index i kurang dari sisa nyawa, nyalakan gambarnya
            // Contoh: Sisa 2 nyawa.
            // i=0 (Hati 1) -> Hidup (0 < 2)
            // i=1 (Hati 2) -> Hidup (1 < 2)
            // i=2 (Hati 3) -> Mati (2 tidak < 2)
            if (i < currentLives)
            {
                heartIcons[i].enabled = true;
            }
            else
            {
                heartIcons[i].enabled = false;
            }
        }
    }

    void TriggerGameOver()
    {
        Debug.Log("GAME OVER BRO!");

        // Bekukan waktu biar game berhenti
        Time.timeScale = 0;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }
}