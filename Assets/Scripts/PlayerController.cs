using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Components")]
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private float moveInput;
    private bool isGrounded;
    private bool jumpRequest;

    // Batas layar (supaya tidak jatuh dari tali)
    public float xLimit = 8f;

    void Update()
    {
        // 1. Input Gerak Kiri/Kanan (A/D atau Panah Kiri/Kanan)
        moveInput = Input.GetAxisRaw("Horizontal");

        // 2. Cek apakah kaki menyentuh layer 'Ground'
        // Membuat lingkaran kecil di kaki untuk mendeteksi tanah
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        // 3. Input Lompat (Spasi / W / Panah Atas)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpRequest = true;
        }
    }

    void FixedUpdate()
    {
        // 1. Cek apakah sedang di tanah
        if (isGrounded)
        {
            // HANYA update gerak kiri/kanan kalau menapak tanah
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            // Kalo di udara, biarkan kecepatan X tetap sama seperti saat terakhir menyentuh tanah (Momentum)
            // Kita hanya update Y (biar gravitasi/lompat tetap jalan)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
        }

        // 2. Eksekusi Lompat
        if (jumpRequest)
        {
            // Tambahkan gaya ke atas
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // Matikan isGrounded sesaat supaya logika "else" di atas langsung jalan di frame berikutnya
            isGrounded = false;

            jumpRequest = false; // Reset request
        }

        // 3. Batas layar (tetap jalan baik di darat maupun udara)
        float clampedX = Mathf.Clamp(transform.position.x, -xLimit, xLimit);
        transform.position = new Vector2(clampedX, transform.position.y);
    }

    // Untuk melihat radius ground check di Scene View (Debugging)
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        }
    }
}