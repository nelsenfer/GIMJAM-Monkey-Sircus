using System.Collections;
using UnityEngine;

public class NPCBouncer : MonoBehaviour
{
    [Header("---- Settings ----")]
    public float jumpHeight = 0.5f;   // Tinggi loncatan
    public float jumpDuration = 0.4f; // Cepat/Lambatnya loncat

    // Kurva biar loncatnya melengkung bagus (Naik Cepat, Turun Lambat)
    public AnimationCurve jumpCurve = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.5f, 1),
        new Keyframe(1, 0)
    );

    [Header("---- Sprites (GAMBAR) ----")]
    [Tooltip("Masukkan gambar posisi DIAM di sini")]
    public Sprite idleSprite;
    [Tooltip("Masukkan gambar posisi ANGKAT TANGAN di sini")]
    public Sprite jumpSprite;

    private Vector3 startPos;
    private bool isJumping = false;
    private SpriteRenderer sr; // Komponen untuk ganti gambar

    void Start()
    {
        startPos = transform.localPosition; // Simpan posisi awal
        sr = GetComponent<SpriteRenderer>(); // Ambil komponen SpriteRenderer

        // Pastikan mulai dengan gambar idle
        if (sr != null && idleSprite != null)
        {
            sr.sprite = idleSprite;
        }
    }

    public void DoJump(float delay, int times = 1)
    {
        if (!isJumping) StartCoroutine(JumpRoutine(delay, times));
    }

    IEnumerator JumpRoutine(float delay, int times)
    {
        isJumping = true;

        // 1. Tunggu Delay (Biar gak barengan persis sama temannya)
        yield return new WaitForSeconds(delay);

        // ==================================================
        // 🚀 MULAI LONCAT: GANTI GAMBAR JADI ANGKAT TANGAN
        // ==================================================
        if (sr != null && jumpSprite != null)
        {
            sr.sprite = jumpSprite;
        }

        // 2. Loop loncat berapa kali?
        for (int i = 0; i < times; i++)
        {
            float timer = 0;
            while (timer <= jumpDuration)
            {
                timer += Time.deltaTime;
                float progress = timer / jumpDuration;

                // Hitung posisi Y berdasarkan kurva
                float height = jumpCurve.Evaluate(progress) * jumpHeight;
                transform.localPosition = new Vector3(startPos.x, startPos.y + height, startPos.z);

                yield return null;
            }
            // Pastikan balik napak tanah pas selesai 1 lompatan
            transform.localPosition = startPos;
        }

        // ==================================================
        // 🛬 SELESAI LONCAT: KEMBALI KE GAMBAR DIAM
        // ==================================================
        if (sr != null && idleSprite != null)
        {
            sr.sprite = idleSprite;
        }

        isJumping = false;
    }
}