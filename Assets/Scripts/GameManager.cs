using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Settings - Effects ✨")]
    public ParticleSystem confettiPrefab;
    public float confettiXOffset = 8.0f; // Jarak dari tengah ke samping (Kiri/Kanan)
    public float confettiYOffset = -4.0f; // Tinggi posisi spawn (di bawah layar)

    [Header("Settings - Fun Meter")]
    public Slider funBarSlider;
    public float maxFun = 5000f;
    public float currentFun = 0f;

    [Header("Settings - Score Popup ✨")]
    public TextMeshProUGUI scorePopupText;
    public float popupDuration = 1.0f;
    public float floatSpeed = 100f;

    [Header("Settings - Combo System 🔥")]
    public int[] comboMultipliers;
    private int comboIndex = 0;
    public TextMeshProUGUI comboText;

    [Header("Settings - Heart System ❤️")]
    public int maxLives = 3;
    private int currentLives;
    public Image[] heartIcons;

    [Header("Settings - UI")]
    public GameObject gameOverPanel;
    public GameObject splashText;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (funBarSlider != null) { funBarSlider.maxValue = maxFun; funBarSlider.value = currentFun; }

        currentLives = maxLives;
        UpdateHeartUI();

        // --- UBAHAN 1: Matikan Teks Combo saat awal ---
        if (comboText != null) comboText.gameObject.SetActive(false);
        // ----------------------------------------------

        if (splashText != null) splashText.SetActive(false);
        if (scorePopupText != null) scorePopupText.gameObject.SetActive(false);
    }

    // --- UBAHAN 2: Fungsi AddCombo dihapus/digabung ke AddFun biar rapi ---
    // (Kita tidak butuh AddCombo terpisah lagi karena otomatisasi di bawah)

    public void ResetCombo()
    {
        comboIndex = 0;
        // Kalau reset, sembunyikan lagi teksnya biar bersih
        if (comboText != null) comboText.gameObject.SetActive(false);
    }

    public void AddFun(float baseAmount, Vector3 capturePos)
    {
        if (baseAmount > 0)
        {
            // 1. AMBIL MULTIPLIER SAAT INI (Sebelum dinaikkan)
            int currentMult = 1;
            if (comboMultipliers != null && comboMultipliers.Length > 0)
                currentMult = comboMultipliers[comboIndex];

            // 2. HITUNG SKOR
            float finalScore = baseAmount * currentMult;

            // 3. TAMPILKAN UI COMBO (Sesuai yang dipakai barusan)
            if (comboText != null)
            {
                comboText.text = "x" + currentMult;
                comboText.gameObject.SetActive(true); // Nyalakan Teks!

                // Warnai merah kalau sudah mentok max
                if (comboIndex >= comboMultipliers.Length - 1)
                    comboText.color = Color.red;
                else
                    comboText.color = Color.white;
            }

            // 4. POPUP & BAR
            TriggerScorePopup(finalScore, capturePos);
            currentFun += finalScore;
            currentFun = Mathf.Clamp(currentFun, 0, maxFun);
            if (funBarSlider != null) funBarSlider.value = currentFun;

            // 5. NAIKKAN LEVEL UNTUK BOLA BERIKUTNYA (Diam-diam)
            if (comboMultipliers != null && comboIndex < comboMultipliers.Length - 1)
            {
                comboIndex++;
            }
        }
    }

    void TriggerScorePopup(float amount, Vector3 worldPos)
    {
        StopCoroutine("ShowScorePopupRoutine");
        StartCoroutine(ShowScorePopupRoutine(amount, worldPos));
    }

    IEnumerator ShowScorePopupRoutine(float amount, Vector3 worldPos)
    {
        if (scorePopupText != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            screenPos.y += 50f;
            scorePopupText.transform.position = screenPos;
            scorePopupText.text = "+ " + amount.ToString("F0");
            scorePopupText.gameObject.SetActive(true);
            scorePopupText.alpha = 1f;

            float timer = 0;
            Vector3 startPos = scorePopupText.transform.position;

            while (timer < popupDuration)
            {
                timer += Time.deltaTime;
                float progress = timer / popupDuration;
                scorePopupText.transform.position = startPos + new Vector3(0, floatSpeed * timer, 0);
                scorePopupText.alpha = Mathf.Lerp(1f, 0f, progress);
                yield return null;
            }
            scorePopupText.gameObject.SetActive(false);
        }
    }

    public void ReduceLife()
    {
        ResetCombo();
        currentLives--;
        UpdateHeartUI();
        if (currentLives <= 0) TriggerGameOver();
    }

    public void TriggerSplashEffect()
    {
        StartCoroutine(ShowSplashTextRoutine());
    }

    IEnumerator ShowSplashTextRoutine()
    {
        if (splashText != null)
        {
            // 1. Munculin Teks
            splashText.SetActive(true);

            // 2. Munculin Confetti (KIRI & KANAN)
            if (confettiPrefab != null)
            {
                // --- KIRI (Left Cannon) ---
                // Posisi: X negatif (kiri), Y di bawah
                Vector3 leftPos = new Vector3(-confettiXOffset, confettiYOffset, 0);
                // Rotasi: Miring ke kanan (-60 derajat) biar nembak ke tengah
                Quaternion leftRot = Quaternion.Euler(0, 0, -60);

                ParticleSystem leftConfetti = Instantiate(confettiPrefab, leftPos, leftRot);
                leftConfetti.Play();
                Destroy(leftConfetti.gameObject, 3.0f); // Hapus setelah 3 detik

                // --- KANAN (Right Cannon) ---
                // Posisi: X positif (kanan), Y di bawah
                Vector3 rightPos = new Vector3(confettiXOffset, confettiYOffset, 0);
                // Rotasi: Miring ke kiri (60 derajat) biar nembak ke tengah
                Quaternion rightRot = Quaternion.Euler(0, 0, 60);

                ParticleSystem rightConfetti = Instantiate(confettiPrefab, rightPos, rightRot);
                rightConfetti.Play();
                Destroy(rightConfetti.gameObject, 3.0f);
            }

            // Tunggu teks tampil sebentar
            yield return new WaitForSeconds(0.8f);
            splashText.SetActive(false);
        }
    }

    void UpdateHeartUI()
    {
        if (heartIcons == null) return;
        for (int i = 0; i < heartIcons.Length; i++) heartIcons[i].enabled = (i < currentLives);
    }
    void TriggerGameOver()
    {
        Time.timeScale = 0;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }
}