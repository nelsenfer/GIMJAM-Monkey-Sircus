using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Settings - Effects ✨")]
    public ParticleSystem confettiPrefab;
    public float confettiXOffset = 8.0f;
    public float confettiYOffset = -4.0f;

    [Header("Settings - Fun Meter")]
    public Slider funBarSlider;
    public float maxFun = 5000f;
    public float currentFun = 0f;
    public int score = 0;

    [Header("Settings - Score Popup ✨")]
    public TextMeshProUGUI scorePopupText;
    public float popupDuration = 1.0f;
    public float floatSpeed = 100f;

    [Header("Settings - Combo System 🔥")]
    public int[] comboMultipliers;
    private int comboIndex = 0;
    public TextMeshProUGUI comboText;

    // --- VARIABEL BARU UNTUK LOGIKA HEBOH 10x ---
    private bool hasCelebratedMaxCombo = false;

    [Header("Settings - Heart System ❤️")]
    public int maxLives = 3;
    private int currentLives;
    public Image[] heartIcons;

    [Header("Settings - UI")]
    public GameObject gameOverPanel;
    public GameObject splashText;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (funBarSlider != null)
        {
            funBarSlider.maxValue = maxFun;
            funBarSlider.value = currentFun;
        }

        currentLives = maxLives;
        UpdateHeartUI();

        if (comboText != null) comboText.gameObject.SetActive(false);
        if (splashText != null) splashText.SetActive(false);
        if (scorePopupText != null) scorePopupText.gameObject.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    // --- COMBO & SCORE SYSTEM ---

    public void ResetCombo()
    {
        comboIndex = 0;

        // RESET STATUS PERAYAAN
        // Biar nanti kalau player berhasil naik ke x10 lagi, mereka heboh lagi!
        hasCelebratedMaxCombo = false;

        if (comboText != null) comboText.gameObject.SetActive(false);
    }

    // Called by StackManager when catching Fruit
    public void AddScore(int baseAmount, Vector3 capturePos)
    {
        if (baseAmount > 0)
        {
            // 1. Calculate Multiplier
            int currentMult = 1;
            if (comboMultipliers != null && comboMultipliers.Length > 0)
                currentMult = comboMultipliers[comboIndex];

            // --- LOGIKA HEBOH PERTAMA KALI STRIKE 10x (ATAU LEBIH) ---
            if (currentMult >= 10 && !hasCelebratedMaxCombo)
            {
                // Panggil Pasukan Monyet!
                if (CrowdManager.instance != null)
                {
                    CrowdManager.instance.TriggerCelebrate();
                }

                // Tandai sudah dirayakan, biar buah berikutnya (yang x10 juga) gak bikin heboh terus.
                hasCelebratedMaxCombo = true;

                // Opsional: Kasih efek suara 'YEEAAAH' di sini kalau ada
                Debug.Log("🎉 FIRST TIME 10x STRIKE! HEBOH!");
            }
            // ----------------------------------------------------------

            // 2. Calculate Final Score
            int finalScore = baseAmount * currentMult;
            score += finalScore;

            // 3. Update Combo UI
            if (comboText != null)
            {
                comboText.text = "x" + currentMult;
                comboText.gameObject.SetActive(true);

                if (comboIndex >= comboMultipliers.Length - 1)
                    comboText.color = Color.red;
                else
                    comboText.color = Color.white;
            }

            // 4. Update Fun Bar & Popup
            currentFun += finalScore;
            currentFun = Mathf.Clamp(currentFun, 0, maxFun);
            if (funBarSlider != null) funBarSlider.value = currentFun;

            TriggerScorePopup(finalScore, capturePos);

            // 5. Increase Combo Level for Next Item
            if (comboMultipliers != null && comboIndex < comboMultipliers.Length - 1)
            {
                comboIndex++;
            }
        }
    }

    // --- LIFE SYSTEM ---

    public void ReduceLives()
    {
        ResetCombo(); // Ini otomatis reset hasCelebratedMaxCombo juga
        currentLives--;
        UpdateHeartUI();

        Debug.Log("ouch! Lives left: " + currentLives);

        if (currentLives <= 0)
        {
            TriggerGameOver();
        }
    }

    void UpdateHeartUI()
    {
        if (heartIcons == null) return;
        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].enabled = (i < currentLives);
        }
    }

    // --- VISUAL EFFECTS ---

    public void TriggerSplashEffect()
    {
        StartCoroutine(ShowSplashTextRoutine());
    }

    IEnumerator ShowSplashTextRoutine()
    {
        if (splashText != null)
        {
            splashText.SetActive(true);

            if (confettiPrefab != null)
            {
                Vector3 leftPos = new Vector3(-confettiXOffset, confettiYOffset, 0);
                Quaternion leftRot = Quaternion.Euler(0, 0, -60);
                ParticleSystem leftConfetti = Instantiate(confettiPrefab, leftPos, leftRot);
                leftConfetti.Play();
                Destroy(leftConfetti.gameObject, 3.0f);

                Vector3 rightPos = new Vector3(confettiXOffset, confettiYOffset, 0);
                Quaternion rightRot = Quaternion.Euler(0, 0, 60);
                ParticleSystem rightConfetti = Instantiate(confettiPrefab, rightPos, rightRot);
                rightConfetti.Play();
                Destroy(rightConfetti.gameObject, 3.0f);
            }

            yield return new WaitForSeconds(0.8f);
            splashText.SetActive(false);
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

    // --- GAME OVER LOGIC ---

    void TriggerGameOver()
    {
        Debug.Log("💀 GAME OVER");
        Time.timeScale = 0;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}