using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Components")]
    public Animator anim;
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private float moveInput;
    private bool isGrounded;
    private bool jumpRequest;

    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. Input Gerak (-1 untuk Kiri, 1 untuk Kanan, 0 Diam)
        moveInput = Input.GetAxisRaw("Horizontal");

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpRequest = true;
        }

        // --- UPDATE ANIMATOR ---
        // Kita kirim nilai mentah (-1, 0, 1) ke parameter "InputX"
        // Biarkan Animator yang menentukan mau putar klip Maju atau Mundur
        anim.SetFloat("InputX", -moveInput);
    }

    void FixedUpdate()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
        }

        if (jumpRequest)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            jumpRequest = false;
        }
    }
}