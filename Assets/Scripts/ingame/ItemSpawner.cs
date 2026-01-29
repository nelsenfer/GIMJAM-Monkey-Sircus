using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("---- PREFABS ----")]
    public GameObject[] fruitPrefabs;
    public GameObject[] bombPrefabs;

    [Header("---- AREA & POSISI ----")]
    public Collider2D groundCollider;
    public float padding = 0.5f;
    public float startHeight = 6.0f;

    [Header("---- 🍎 FRUIT SETTINGS ----")]
    public float fruitSpawnInterval = 1.0f;

    [Header("---- 💣 BOMB SETTINGS (Normal) ----")]
    public float minBombInterval = 4.0f;
    public float maxBombInterval = 8.0f;

    [Header("---- 🔥 CHAOS MODE SETTINGS 🔥 ----")]
    public float triggerTime = 60f;
    public int triggerScore = 2000;

    [Tooltip("Durasi Pure Chaos (Hujan Bom)")]
    public float chaosDuration = 5.0f; // Diubah jadi 5 Detik sesuai request
    public float chaosCooldown = 15f;
    public float chaosBombInterval = 0.5f; // Kecepatan bom pas Pure Chaos

    // Internal State
    private float fruitTimer;
    private float bombTimer;

    private float gameTimeElapsed;
    private bool isAttacking = false; // KUNCI: Kalau true, Buah & Bom Normal berhenti total
    private bool canTriggerChaos = true;

    private StackManager playerStack;

    void Start()
    {
        playerStack = FindAnyObjectByType<StackManager>();
        fruitTimer = fruitSpawnInterval;
        bombTimer = Random.Range(minBombInterval, maxBombInterval);

        if (groundCollider == null) Debug.LogError("⚠️ HEH! Masukin Collider Tali dulu!");
    }

    void Update()
    {
        gameTimeElapsed += Time.deltaTime;

        // 1. CEK TRIGGER CHAOS
        if (!isAttacking && canTriggerChaos)
        {
            CheckChaosTrigger();
        }

        // 2. LOGIKA SPAWN NORMAL (Hanya jalan kalau TIDAK sedang Chaos/Attack)
        // Ini menjawab request: "Spawn buah berhenti biar player fokus"
        if (!isAttacking)
        {
            bool spawnedSomething = false;

            // --- JANTUNG 1: BUAH ---
            fruitTimer -= Time.deltaTime;
            if (fruitTimer <= 0)
            {
                SpawnSingleItem(false);
                fruitTimer = fruitSpawnInterval;
                spawnedSomething = true;
            }

            // --- JANTUNG 2: BOM ---
            HandleBombSpawn(spawnedSomething);
        }
    }

    void CheckChaosTrigger()
    {
        int currentScore = 0;
        if (GameManager.instance != null) currentScore = GameManager.instance.score;

        if (gameTimeElapsed > triggerTime || currentScore >= triggerScore)
        {
            StartCoroutine(ChaosModeRoutine());
        }
    }

    // --- LOGIKA UTAMA WAVE ---
    IEnumerator ChaosModeRoutine()
    {
        isAttacking = true; // 🛑 STOP SPAWN BUAH & BOM NORMAL
        canTriggerChaos = false;

        Debug.Log("💀 CHAOS MODE TRIGGERED! (Player Fokus Menghindar)");

        // Peringatan Singkat
        yield return new WaitForSeconds(0.5f);

        // ACAK JENIS SERANGAN
        int chaosType = Random.Range(0, 3);

        if (chaosType == 0)
        {
            Debug.Log("🎲 Event: TRIPLE THREAT!");
            yield return StartCoroutine(Attack_TripleThreat());
        }
        else if (chaosType == 1)
        {
            Debug.Log("🎲 Event: RAIN WAVE!");
            yield return StartCoroutine(Attack_RainWave());
        }
        else
        {
            Debug.Log("🎲 Event: PURE CHAOS (5 Detik)");
            yield return StartCoroutine(Attack_PureChaos());
        }

        // --- FASE SELESAI ---
        Debug.Log("😌 CHAOS OVER. Bonus Fruit & Resume.");

        // 1. KASIH HADIAH (BONUS BUAH)
        SpawnSingleItem(false);

        // 2. PERBAIKAN DI SINI (PENTING!) 🛠️
        // Reset timer buah agar tidak "dobel spawn" di frame berikutnya.
        // Jadi player dapat 1 buah bonus, lalu nunggu 1 detik lagi buat buah berikutnya.
        fruitTimer = fruitSpawnInterval;

        // Kembalikan ke Mode Normal
        isAttacking = false;

        // Cooldown sebelum bisa Chaos lagi
        yield return new WaitForSeconds(chaosCooldown);
        canTriggerChaos = true;
    }

    // --- LOGIKA BOM NORMAL ---
    void HandleBombSpawn(bool isBusy)
    {
        bombTimer -= Time.deltaTime;

        if (bombTimer <= 0)
        {
            if (isBusy)
            {
                bombTimer = 0.3f; // Ngalah sama buah
            }
            else
            {
                SpawnSingleItem(true);
                // Random Interval Normal
                bombTimer = Random.Range(minBombInterval, maxBombInterval);
            }
        }
    }

    // ================================================================
    // ⚔️ JURUS-JURUS SPESIAL ⚔️
    // ================================================================

    // JURUS 1: TRIPLE THREAT (Tanpa Delay Akhir)
    IEnumerator Attack_TripleThreat()
    {
        float[] xPositions = GetLanePositions();

        SpawnBombAt(xPositions[0]);
        SpawnBombAt(xPositions[1]);
        SpawnBombAt(xPositions[2]);

        // Selesai Instan! Langsung lanjut ke Bonus Buah.
        yield return null;
    }

    // JURUS 2: RAIN WAVE
    IEnumerator Attack_RainWave()
    {
        float[] xPositions = GetLanePositions();
        bool leftToRight = (Random.value > 0.5f);

        if (leftToRight)
        {
            SpawnBombAt(xPositions[0]); yield return new WaitForSeconds(0.4f);
            SpawnBombAt(xPositions[1]); yield return new WaitForSeconds(0.4f);
            SpawnBombAt(xPositions[2]);
        }
        else
        {
            SpawnBombAt(xPositions[2]); yield return new WaitForSeconds(0.4f);
            SpawnBombAt(xPositions[1]); yield return new WaitForSeconds(0.4f);
            SpawnBombAt(xPositions[0]);
        }
    }

    // JURUS 3: PURE CHAOS (Looping 5 Detik)
    IEnumerator Attack_PureChaos()
    {
        float timer = chaosDuration; // 5 Detik

        while (timer > 0)
        {
            SpawnSingleItem(true); // Spawn Bom Acak
            yield return new WaitForSeconds(chaosBombInterval); // Tunggu interval cepat (0.5s)
            timer -= chaosBombInterval;
        }
    }

    // --- FUNGSI BANTUAN ---

    float[] GetLanePositions()
    {
        float min = -2f, max = 2f;
        if (groundCollider != null)
        {
            min = groundCollider.bounds.min.x + padding;
            max = groundCollider.bounds.max.x - padding;
        }
        float width = max - min;
        return new float[] { min + (width * 0.15f), min + (width * 0.5f), min + (width * 0.85f) };
    }

    void SpawnSingleItem(bool isBomb)
    {
        float spawnX = 0;
        if (groundCollider != null)
        {
            spawnX = Random.Range(groundCollider.bounds.min.x + padding, groundCollider.bounds.max.x - padding);
        }
        else spawnX = Random.Range(-2.5f, 2.5f);

        GameObject prefabToSpawn = null;

        if (isBomb)
        {
            if (bombPrefabs.Length > 0)
                prefabToSpawn = bombPrefabs[Random.Range(0, bombPrefabs.Length)];
        }
        else
        {
            if (fruitPrefabs.Length > 0)
                prefabToSpawn = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];
        }

        if (prefabToSpawn != null) SpawnObject(prefabToSpawn, spawnX);
    }

    void SpawnBombAt(float xPos)
    {
        if (bombPrefabs.Length > 0)
        {
            GameObject bomb = bombPrefabs[Random.Range(0, bombPrefabs.Length)];
            SpawnObject(bomb, xPos);
        }
    }

    void SpawnObject(GameObject prefab, float xPos)
    {
        Vector3 pos = new Vector3(xPos, startHeight, 0);
        GameObject newItem = Instantiate(prefab, pos, Quaternion.identity);

        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.lemparBuah);
        }

        FallingItem itemScript = newItem.GetComponent<FallingItem>();
        if (itemScript != null)
        {
            float targetY = (playerStack != null) ? playerStack.GetCurrentHeight() : -3f;
            itemScript.targetHeight = targetY;
            itemScript.startHeight = startHeight;
        }
    }

    void OnDrawGizmos()
    {
        if (groundCollider != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 start = new Vector3(groundCollider.bounds.min.x + padding, startHeight, 0);
            Vector3 end = new Vector3(groundCollider.bounds.max.x - padding, startHeight, 0);
            Gizmos.DrawLine(start, end);
        }
    }
}