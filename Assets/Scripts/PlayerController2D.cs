using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Moviment")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("Detecció de terra")]
    public Transform groundCheck;      // Punt sota el jugador
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;      // Assigna el layer "Ground"

    private Rigidbody2D rb;
    private bool isGrounded;

    private float moveInput;
    private bool jumpPressed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;  // No es tomba
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        // Llegeix l’input horitzontal
        moveInput = Input.GetAxisRaw("Horizontal");

        // Comprova salt
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
    }

    void FixedUpdate()
    {
        // Comprova si toca terra
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Mou el jugador
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Salta
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        jumpPressed = false; // reset
    }

    void OnDrawGizmosSelected()
    {
        // Dibuixa el cercle de detecció del terra (només visible a l'editor)
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
