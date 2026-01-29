using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;

    [Header("Components")]
    public Animator anim;
    public Rigidbody2D rb;

    private float moveInput;

    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. Input Gerak (-1 untuk Kiri, 1 untuk Kanan, 0 Diam)
        moveInput = Input.GetAxisRaw("Horizontal");

        // 2. Update Animasi
        // Pastikan di Animator kamu punya parameter Float bernama "InputX"
        if (anim != null)
        {
            anim.SetFloat("InputX", moveInput);
        }
    }

    void FixedUpdate()
    {
        // 3. Eksekusi Gerak Fisika
        // Kita ubah kecepatan X sesuai input, tapi biarkan Y apa adanya (biar gravitasi tetap jalan)
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    // --- ANIMASI REAKSI (Dipanggil script lain) ---

    public void PlayHappyAnim()
    {
        if (anim != null)
        {
            anim.SetTrigger("TriggerHappy");
        }
    }

    public void PlayHurtAnim()
    {
        if (anim != null)
        {
            anim.SetTrigger("TriggerHurt");
        }
    }
}