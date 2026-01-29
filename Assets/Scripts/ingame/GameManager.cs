using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Settings - Effects ✨")]
    public ParticleSystem confettiPrefab;
    public float confettiXOffset = 8.0f;
    public float confettiYOffset = -4.0f;

    [Header("Settings - Fun Meter")]
    public Slider funBarSlider;
    public float maxFun = 5000f; // Batas Maksimal (Sudah diatur di sini)
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
    public GameObject winPanel;
    public TextMeshProUGUI winScoreText;

    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverScoreText;

    public GameObject splashText;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;

        StartCoroutine(StartGameRoutine());

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
        if (winPanel != null) winPanel.SetActive(false);
    }

    IEnumerator StartGameRoutine()
    {

        // 2. TUNGGU 1 FRAME (PENTING! ⏳)
        // Memberi waktu agar Script TiraiController & Animator-nya 'Bangun' (Awake/Start) dulu
        yield return null;

        // 3. Baru panggil Tirai lewat Instance
        if (TiraiController.instance != null)
        {
            Debug.Log("🏁 GameManager: Memanggil Tirai Sekarang!");
            TiraiController.instance.TampilkanTirai();
            TiraiController.instance.bg.SetActive(false);
            yield return new WaitForSeconds(1f);
            TiraiController.instance.bg.SetActive(true);
            TiraiController.instance.SembunyikanTirai();
        }
        else
        {
            Debug.LogError("❌ TiraiController INSTANCE masih Null! Pastikan ada di Scene.");
        }
    }

    // --- COMBO & SCORE SYSTEM ---

    public void ResetCombo()
    {
        comboIndex = 0;
        hasCelebratedMaxCombo = false;
        if (comboText != null) comboText.gameObject.SetActive(false);
    }

    public void AddScore(int baseAmount, Vector3 capturePos)
    {
        if (baseAmount > 0)
        {
            int currentMult = 1;
            if (comboMultipliers != null && comboMultipliers.Length > 0)
                currentMult = comboMultipliers[comboIndex];

            if (currentMult >= 10 && !hasCelebratedMaxCombo)
            {
                if (CrowdManager.instance != null) CrowdManager.instance.TriggerCelebrate();
                hasCelebratedMaxCombo = true;
                Debug.Log("🎉 FIRST TIME 10x STRIKE! HEBOH!");
                AudioManager.Instance.PlaySFX(AudioManager.Instance.crowd);
            }

            int finalScore = baseAmount * currentMult;
            score += finalScore;

            if (comboText != null)
            {
                comboText.text = "x" + currentMult;
                comboText.gameObject.SetActive(true);
                if (comboIndex >= comboMultipliers.Length - 1) comboText.color = Color.red;
                else comboText.color = Color.white;

                float targetVolume = Mathf.Clamp((float)currentMult / 10f, 0.1f, 1f);
                AudioManager.Instance.PlaySFX(AudioManager.Instance.combo, targetVolume);
            }

            currentFun += finalScore;
            // 🔒 INI PENJAGANYA: currentFun tidak akan pernah lebih dari maxFun (5000)
            currentFun = Mathf.Clamp(currentFun, 0, maxFun);

            if (funBarSlider != null) funBarSlider.value = currentFun;

            TriggerScorePopup(finalScore, capturePos);

            if (comboMultipliers != null && comboIndex < comboMultipliers.Length - 1)
            {
                comboIndex++;
            }
            if (currentFun >= maxFun)
            {
                TriggerWin();
            }
        }
    }

    public void ReduceLives()
    {
        ResetCombo();
        currentLives--;
        UpdateHeartUI();

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

    public void TriggerSplashEffect()
    {
        StartCoroutine(ShowSplashTextRoutine());
        AudioManager.Instance.PlaySFX(AudioManager.Instance.splash);
        AudioManager.Instance.PlaySFX(AudioManager.Instance.crowd);
        AudioManager.Instance.PlaySFX(AudioManager.Instance.confetti, 1.5f);
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
    void TriggerWin()
    {
        // 🔥 UPDATE FORMAT TEXT DI SINI
        if (winScoreText != null)
        {
            // Hasil: "Fun Value : 5000 / 5000"
            winScoreText.text = currentFun.ToString("F0") + " / " + maxFun.ToString("F0");
        }

        MenuScript.Instance.Pause();

        if (winPanel != null) winPanel.SetActive(true);

        if (ChangeUi.Instance != null)
        {
            ChangeUi.Instance.gameSelesai();
        }

        AudioManager.Instance.PlayMusicOnce(AudioManager.Instance.win);

    }

    void TriggerGameOver()
    {
        Debug.Log("💀 GAME OVER");

        // 🔥 UPDATE FORMAT TEXT DI SINI
        if (gameOverScoreText != null)
        {
            // Hasil contoh: "Fun Value : 1250 / 5000"
            gameOverScoreText.text = currentFun.ToString("F0") + " / " + maxFun.ToString("F0");
        }

        MenuScript.Instance.Pause();
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        AudioManager.Instance.PlayMusicOnce(AudioManager.Instance.gameOver);
    }

    public void RestartGame()
    {
        MenuScript.Instance.Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GiveUp()
    {
        MenuScript.Instance.Resume();
        SceneManager.LoadScene("MainMenu");
    }
}