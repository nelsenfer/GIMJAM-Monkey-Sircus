using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] itemPrefabs;
    public float minSpawnTime = 1.0f;
    public float maxSpawnTime = 3.0f;
    public float xRange = 7.0f;

    // Referensi ke Player untuk tanya tinggi tumpukan
    public StackManager playerStackManager;

    private float timer;

    void Start()
    {
        // Cari otomatis kalau lupa drag
        if (playerStackManager == null)
            playerStackManager = FindFirstObjectByType<StackManager>();

        ResetTimer();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ThrowItem();
            ResetTimer();
        }
    }

    void ThrowItem()
    {
        int randomIndex = Random.Range(0, itemPrefabs.Length);
        GameObject selectedItemPrefab = itemPrefabs[randomIndex];

        float randomX = Random.Range(-xRange, xRange);

        // POSISI START TETAP DI ATAS LAYAR (Misal Y=6)
        // Kita ambil Start Height dari prefab aslinya aja biar aman
        FallingItem prefabScript = selectedItemPrefab.GetComponent<FallingItem>();
        float startY = (prefabScript != null) ? prefabScript.startHeight : 6f;

        Vector3 spawnPos = new Vector3(randomX, startY, 0);

        // INSTANTIATE
        GameObject newItem = Instantiate(selectedItemPrefab, spawnPos, Quaternion.identity);

        // --- UPDATE TARGET HEIGHT SECARA DINAMIS ---
        FallingItem itemScript = newItem.GetComponent<FallingItem>();
        if (itemScript != null && playerStackManager != null)
        {
            // Ambil tinggi tumpukan terkini
            float currentStackTop = playerStackManager.GetCurrentHeight();

            // Set target si buah ke tinggi tumpukan itu
            itemScript.targetHeight = currentStackTop;
        }
    }

    void ResetTimer()
    {
        timer = Random.Range(minSpawnTime, maxSpawnTime);
    }
}