using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject[] fruitPrefabs; // Array Buah Bagus
    public GameObject[] bombPrefabs;  // Array Bom (Bisa banyak variasi)

    [Header("🎯 Auto-Spawn Area (PENTING)")]
    [Tooltip("Tarik object Tali/Ground yang punya BoxCollider2D ke sini")]
    public Collider2D groundCollider;
    [Tooltip("Jarak aman dari ujung tali (biar buah gak jatuh pas di tebing)")]
    public float padding = 0.5f;

    [Header("Spawn Settings")]
    public float startHeight = 6.0f;
    public float spawnInterval = 2.0f;

    [Header("Difficulty")]
    [Range(0f, 1f)] public float bombChance = 0.1f;

    private float timer;
    private StackManager playerStack;

    void Start()
    {
        playerStack = FindAnyObjectByType<StackManager>();

        if (groundCollider == null)
        {
            Debug.LogError("⚠️ LUPA MASUKIN COLLIDER TALI di ItemSpawner!");
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SpawnItem();
            timer = spawnInterval;
        }
    }

    void SpawnItem()
    {
        // 1. HITUNG POSISI BERDASARKAN TALI
        float spawnX = 0;

        if (groundCollider != null)
        {
            float minX = groundCollider.bounds.min.x + padding;
            float maxX = groundCollider.bounds.max.x - padding;
            spawnX = Random.Range(minX, maxX);
        }
        else
        {
            spawnX = Random.Range(-2.5f, 2.5f);
        }

        Vector3 spawnPos = new Vector3(spawnX, startHeight, 0);

        // 2. PILIH ITEM (LOGIKA BARU DI SINI)
        GameObject prefabToSpawn = null; // Siapkan wadah kosong

        // Cek: Apakah Buah ada isinya?
        if (fruitPrefabs.Length > 0)
        {
            // Roll Dadu: Apakah spawn Bom? DAN Apakah kita punya daftar Bom?
            if (Random.value < bombChance && bombPrefabs.Length > 0)
            {
                // --- PILIH BOM ACAK DARI ARRAY ---
                int randomIndex = Random.Range(0, bombPrefabs.Length);
                prefabToSpawn = bombPrefabs[randomIndex];
            }
            else
            {
                // --- PILIH BUAH BAGUS ACAK ---
                int randomIndex = Random.Range(0, fruitPrefabs.Length);
                prefabToSpawn = fruitPrefabs[randomIndex];
            }

            // 3. SPAWN (Hanya jika prefabToSpawn ada isinya)
            if (prefabToSpawn != null)
            {
                GameObject newItem = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

                // 4. SET TARGET
                FallingItem itemScript = newItem.GetComponent<FallingItem>();
                if (itemScript != null)
                {
                    float targetY = -3f;
                    if (playerStack != null)
                    {
                        targetY = playerStack.GetCurrentHeight();
                    }
                    itemScript.targetHeight = targetY;
                    itemScript.startHeight = startHeight;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (groundCollider != null)
        {
            Gizmos.color = Color.green;
            float minX = groundCollider.bounds.min.x + padding;
            float maxX = groundCollider.bounds.max.x - padding;
            Vector3 lineStart = new Vector3(minX, startHeight, 0);
            Vector3 lineEnd = new Vector3(maxX, startHeight, 0);
            Gizmos.DrawLine(lineStart, lineEnd);
            Gizmos.DrawWireSphere(lineStart, 0.2f);
            Gizmos.DrawWireSphere(lineEnd, 0.2f);
        }
    }
}