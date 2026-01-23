using UnityEngine;

public class FallingItem : MonoBehaviour
{
    [Header("---- Height & Arc Settings ----")]
    public float startHeight = -5.0f; // Mulai dari bawah
    public float targetHeight = -2.0f; // Target di keranjang
    public float arcHeight = 5.0f;     // Melambung tinggi

    [Header("---- Visual Depth (Layer) ----")]
    [Tooltip("Urutan Layer saat baru muncul (Harus tinggi biar di depan)")]
    public int startOrder = 100;

    [Tooltip("Urutan Layer saat mendarat (Samakan dengan layer Monyet)")]
    public int targetOrder = 1;

    [Header("---- Fall Settings ----")]
    public float fallDuration = 3.0f;
    public float startScale = 3.0f;   // Besar banget di awal
    public float endScale = 0.5f;

    [Header("---- Game Data ----")]
    public int funValueAmount = 10;

    private float timer = 0;
    private Vector3 startPos;
    private Vector3 targetPos;
    private Collider2D col;
    private SpriteRenderer sr; // <-- Referensi ke Gambar
    private bool isCatchable = false;

    void Start()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>(); // Ambil komponen gambar

        if (col != null) col.enabled = false;

        // Setup Posisi & Scale
        transform.position = new Vector3(transform.position.x, startHeight, 0);
        startPos = transform.position;
        targetPos = new Vector3(startPos.x, targetHeight, 0);
        transform.localScale = Vector3.one * startScale;

        // Setup Layer Awal
        if (sr != null)
        {
            sr.sortingOrder = startOrder;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / fallDuration;

        if (progress <= 1.0f)
        {
            // --- 1. GERAKAN PARABOLA ---
            Vector3 linearPos = Vector3.Lerp(startPos, targetPos, progress);
            float heightOffset = 4 * arcHeight * progress * (1 - progress);
            transform.position = linearPos + new Vector3(0, heightOffset, 0);

            // --- 2. SCALING ---
            float currentScale = Mathf.Lerp(startScale, endScale, progress);
            transform.localScale = Vector3.one * currentScale;

            // --- 3. DYNAMIC SORTING ORDER (Baru!) ---
            if (sr != null)
            {
                // Mengubah float hasil Lerp menjadi int (bilangan bulat)
                float currentOrder = Mathf.Lerp(startOrder, targetOrder, progress);
                sr.sortingOrder = (int)currentOrder;
            }

            // --- 4. COLLIDER ---
            if (progress > 0.8f && !isCatchable)
            {
                isCatchable = true;
                if (col != null) col.enabled = true;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}