using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Apa yang mau dilempar?")]
    [Tooltip("Masukkan Prefab Pisang dan Bom kesini")]
    public GameObject[] itemPrefabs; // Array biar bisa isi banyak jenis barang

    [Header("Pengaturan Waktu")]
    public float minSpawnTime = 1.0f; // Paling cepat muncul tiap 1 detik
    public float maxSpawnTime = 3.0f; // Paling lambat muncul tiap 3 detik

    [Header("Area Lemparan (Posisi X)")]
    public float xRange = 7.0f; // Seberapa lebar area lemparannya (kiri-kanan)

    private float timer;

    void Start()
    {
        // Set waktu lemparan pertama
        ResetTimer();
    }

    void Update()
    {
        timer -= Time.deltaTime; // Kurangi waktu mundur

        if (timer <= 0)
        {
            ThrowItem();
            ResetTimer();
        }
    }

    void ThrowItem()
    {
        // 1. Pilih barang secara acak (Pisang atau Bom)
        int randomIndex = Random.Range(0, itemPrefabs.Length);
        GameObject selectedItem = itemPrefabs[randomIndex];

        // 2. Tentukan posisi X secara acak
        // (Dari minus xRange sampai positif xRange)
        float randomX = Random.Range(-xRange, xRange);

        // Kita pakai posisi Y dari script FallingItem nanti, jadi disini set 0 dulu gpp
        Vector3 spawnPos = new Vector3(randomX, 0, 0);

        // 3. Munculkan barangnya (Instantiate)
        Instantiate(selectedItem, spawnPos, Quaternion.identity);
    }

    void ResetTimer()
    {
        // Acak waktu untuk lemparan berikutnya biar gak ketebak
        timer = Random.Range(minSpawnTime, maxSpawnTime);
    }
}