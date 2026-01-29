using UnityEngine;

public class FallingItem : MonoBehaviour
{
    [Header("---- Height & Arc Settings ----")]
    public float startHeight = 6.0f;
    public float targetHeight = -2.0f; // Nanti ditimpa oleh Spawner
    public float arcHeight = 2.0f;

    [Header("---- Visual Depth (Layer) ----")]
    public int startOrder = 100;
    public int targetOrder = 5;

    [Header("---- Fall Settings ----")]
    public float fallDuration = 2.0f;
    public float startScale = 2.0f;
    public float endScale = 0.6f;

    [Header("---- Game Data ----")]
    public int funValueAmount = 10; // Kalau positif = Buah, Negatif = Bom

    [Header("---- Stack Tuning (PENTING) 🔧 ----")]

    [Tooltip("Geser gambar ini ke Bawah/Atas. (Negatif = Turun, Positif = Naik). Gunakan ini biar pantat buah nempel.")]
    public float visualOffsetY = 0.0f;

    [Tooltip("Kurangi tinggi tumpukan untuk item BERIKUTNYA. (Makin Besar = Item atas makin turun).")]
    public float heightReduction = 0.0f;

    private float timer = 0;
    private Vector3 startPos;
    private Vector3 targetPos;
    private Collider2D col;
    private SpriteRenderer sr;
    private bool isCatchable = false;
    private bool hasMissed = false; // Status: Apakah sudah lewat target?

    void Start()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        if (col != null) col.enabled = false;

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

        // Kita biarkan progress lewat sampai 1.5 biar jatuh terus ke bawah
        if (progress <= 1.5f)
        {
            // 1. GERAKAN
            Vector3 linearPos = Vector3.LerpUnclamped(startPos, targetPos, progress);
            float heightOffset = 4 * arcHeight * progress * (1 - progress);
            transform.position = linearPos + new Vector3(0, heightOffset, 0);

            // 2. SCALING & SORTING
            float clampedProgress = Mathf.Min(progress, 1.0f);
            float currentScale = Mathf.Lerp(startScale, endScale, clampedProgress);
            transform.localScale = Vector3.one * currentScale;

            if (sr != null)
            {
                float currentOrder = Mathf.Lerp(startOrder, targetOrder, clampedProgress);
                sr.sortingOrder = (int)currentOrder;
            }

            // 3. LOGIKA JATUH / MISS (PENTING BUAT RESET COMBO!)
            if (progress > 1.0f && !hasMissed)
            {
                hasMissed = true; // Tandai sudah lewat
                isCatchable = false;
                if (col != null) col.enabled = false;

                // --- FITUR BARU: RESET COMBO JIKA BUAH JATUH ---
                // Cek: Ini Buah (poin positif) atau Bom?
                // Kita cuma reset kalau yang jatuh adalah BUAH yang harusnya ditangkap.
                if (funValueAmount > 0)
                {
                    if (GameManager.instance != null)
                    {
                        GameManager.instance.ResetCombo();
                        Debug.Log("Combo Putus! Buah Jatuh ke Tanah.");
                    }
                }
                // -----------------------------------------------
            }
            else if (progress > 0.8f && progress <= 1.0f && !isCatchable)
            {
                // Jendela waktu menangkap (80% - 100%)
                isCatchable = true;
                if (col != null) col.enabled = true;
            }
        }
        else
        {
            // Sudah jauh di bawah, hancurkan
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 targetLandingPos = new Vector3(transform.position.x, targetHeight, 0);
        Gizmos.DrawWireSphere(targetLandingPos, 0.3f);
        Gizmos.DrawLine(transform.position, targetLandingPos);
    }
}