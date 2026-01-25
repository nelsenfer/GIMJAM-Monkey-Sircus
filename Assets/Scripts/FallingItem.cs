using UnityEngine;

public class FallingItem : MonoBehaviour
{
    [Header("---- Height & Arc Settings ----")]
    public float startHeight = 6.0f;
    public float targetHeight = -2.0f;
    public float arcHeight = 2.0f;

    [Header("---- Visual Depth (Layer) ----")]
    [Tooltip("Urutan Layer saat baru muncul (Harus tinggi biar di depan)")]
    public int startOrder = 100;
    [Tooltip("Urutan Layer saat mendarat (Samakan dengan layer Monyet)")]
    public int targetOrder = 5;

    [Header("---- Fall Settings ----")]
    public float fallDuration = 2.0f;
    public float startScale = 2.0f;
    public float endScale = 0.6f;

    [Header("---- Game Data ----")]
    public int funValueAmount = 10;

    private float timer = 0;
    private Vector3 startPos;
    private Vector3 targetPos;
    private Collider2D col;
    private SpriteRenderer sr;
    private bool isCatchable = false;
    private bool hasMissed = false; // <-- Status baru: Sudah lewat target

    void Start()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        if (col != null) col.enabled = false;

        // Kunci X biar lurus vertikal
        startPos = new Vector3(transform.position.x, startHeight, 0);
        targetPos = new Vector3(transform.position.x, targetHeight, 0);

        transform.position = startPos;
        transform.localScale = Vector3.one * startScale;

        if (sr != null) sr.sortingOrder = startOrder;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / fallDuration;

        // --- PERUBAHAN UTAMA DI SINI ---
        // Kita biarkan progress lewat sampai 1.5 (150%) biar jatuh terus ke bawah
        if (progress <= 1.5f)
        {
            // 1. GERAKAN PARABOLA (Rumus ini tetap bekerja walaupun progress > 1)
            Vector3 linearPos = Vector3.LerpUnclamped(startPos, targetPos, progress);
            float heightOffset = 4 * arcHeight * progress * (1 - progress);
            transform.position = linearPos + new Vector3(0, heightOffset, 0);

            // 2. SCALING & SORTING ORDER (Mentok di 1.0f biar gak aneh)
            float clampedProgress = Mathf.Min(progress, 1.0f);
            float currentScale = Mathf.Lerp(startScale, endScale, clampedProgress);
            transform.localScale = Vector3.one * currentScale;

            if (sr != null)
            {
                float currentOrder = Mathf.Lerp(startOrder, targetOrder, clampedProgress);
                sr.sortingOrder = (int)currentOrder;
            }

            // 3. LOGIKA COLLIDER (PENTING!)
            if (progress > 1.0f && !hasMissed)
            {
                // SUDAH LEWAT TARGET! Matikan collider.
                hasMissed = true;
                isCatchable = false;
                if (col != null) col.enabled = false;
            }
            else if (progress > 0.8f && progress <= 1.0f && !isCatchable)
            {
                // Jendela waktu untuk menangkap (80% - 100%)
                isCatchable = true;
                if (col != null) col.enabled = true;
            }
        }
        else
        {
            // Sudah jatuh jauh ke bawah, hancurkan.
            Destroy(gameObject);
        }
    }

    // Visualisasi Debugging (Optional)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 targetLandingPos = new Vector3(transform.position.x, targetHeight, 0);
        Gizmos.DrawWireSphere(targetLandingPos, 0.3f);
        Gizmos.DrawLine(transform.position, targetLandingPos);
    }
}