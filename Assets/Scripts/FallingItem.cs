using UnityEngine;

public class FallingItem : MonoBehaviour
{
    [Header("---- Height Settings (Atur Disini) ----")]
    [Tooltip("Ketinggian awal benda saat muncul (Y Posisi Atas)")]
    public float startHeight = 6.0f; // Misal di atas layar

    [Tooltip("Ketinggian target saat benda bisa ditangkap (Y Posisi Keranjang)")]
    public float targetHeight = -2.0f; // Sesuaikan dengan posisi keranjang monyet

    [Header("---- Fall Settings ----")]
    public float fallDuration = 3.0f; // Durasi jatuh dalam detik
    public float startScale = 2.0f;   // Ukuran Besar (Awal)
    public float endScale = 0.5f;     // Ukuran Kecil (Akhir)

    [Header("---- Game Data ----")]
    public int funValueAmount = 10;

    // Private Variables (Gak perlu diubah di Inspector)
    private float timer = 0;
    private Vector3 startPos;
    private Vector3 targetPos;
    private Collider2D col;
    private bool isCatchable = false;

    void Start()
    {
        col = GetComponent<Collider2D>();

        // 1. Pastikan Collider mati di awal
        if (col != null) col.enabled = false;

        // 2. Setup Posisi Awal (Otomatis atur ketinggian Y sesuai settingan)
        // X diambil dari posisi saat ini (bisa dari spawner atau drag manual)
        // Y dipaksa ke startHeight
        transform.position = new Vector3(transform.position.x, startHeight, 0);

        startPos = transform.position;

        // 3. Setup Posisi Target (X sama, Y sesuai targetHeight)
        targetPos = new Vector3(startPos.x, targetHeight, 0);

        // 4. Set ukuran awal
        transform.localScale = Vector3.one * startScale;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / fallDuration;

        if (progress <= 1.0f)
        {
            // Gerak Interpolasi
            transform.position = Vector3.Lerp(startPos, targetPos, progress);

            // Skala Interpolasi
            float currentScale = Mathf.Lerp(startScale, endScale, progress);
            transform.localScale = Vector3.one * currentScale;

            // Logika Aktivasi Collider (di 80% perjalanan)
            if (progress > 0.8f && !isCatchable)
            {
                isCatchable = true;
                if (col != null) col.enabled = true; // BISA DITANGKAP
            }
        }
        else
        {
            // Lewat / Jatuh
            Destroy(gameObject);
        }
    }
}