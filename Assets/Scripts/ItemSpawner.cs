using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject[] fruitPrefabs;
    public GameObject bombPrefab;

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

        // Error handling kalau lupa masukin ground
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
            // Ambil batas kiri dan kanan dari Collider Tali
            float minX = groundCollider.bounds.min.x + padding;
            float maxX = groundCollider.bounds.max.x - padding;

            // Acak posisi di antara batas itu
            spawnX = Random.Range(minX, maxX);
        }
        else
        {
            // Fallback kalau lupa setting (pakai cara lama manual)
            spawnX = Random.Range(-2.5f, 2.5f);
        }

        Vector3 spawnPos = new Vector3(spawnX, startHeight, 0);

        // 2. PILIH ITEM (Logika sama kayak sebelumnya)
        GameObject prefabToSpawn;
        if (fruitPrefabs.Length > 0)
        {
            if (Random.value < bombChance && bombPrefab != null)
            {
                prefabToSpawn = bombPrefab;
            }
            else
            {
                int randomIndex = Random.Range(0, fruitPrefabs.Length);
                prefabToSpawn = fruitPrefabs[randomIndex];
            }

            // 3. SPAWN
            GameObject newItem = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

            // 4. SET TARGET (Biar jatuh ke keranjang)
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

    // Visualisasi Area Spawn di Scene View (Biar kelihatan garisnya)
    void OnDrawGizmos()
    {
        if (groundCollider != null)
        {
            Gizmos.color = Color.green;
            float minX = groundCollider.bounds.min.x + padding;
            float maxX = groundCollider.bounds.max.x - padding;

            // Gambar garis area spawn di atas
            Vector3 lineStart = new Vector3(minX, startHeight, 0);
            Vector3 lineEnd = new Vector3(maxX, startHeight, 0);

            Gizmos.DrawLine(lineStart, lineEnd);
            Gizmos.DrawWireSphere(lineStart, 0.2f);
            Gizmos.DrawWireSphere(lineEnd, 0.2f);
        }
    }
}