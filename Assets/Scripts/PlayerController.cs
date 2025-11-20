using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed = 6f;
    public float jumpForce = 12f;
    private float moveInput;

    public Transform feetPos;
    public float checkRadius = 0.15f;//
    public LayerMask whatIsGround; //DETECTOR DE TERRA
    private bool isGrounded;

    public float coyoteTime = 0.15f;// coyote jump
    private float coyoteTimeCounter; 

    public float jumpTime = 0.18f; //jump hold
    private float jumpTimeCounter;
    private bool isJumping;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime; 
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (moveInput > 0) transform.eulerAngles = new Vector3(0, 0, 0);
        else if (moveInput < 0) transform.eulerAngles = new Vector3(0, 180, 0);

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.deltaTime;
        }

     
        if (coyoteTimeCounter > 0f && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            coyoteTimeCounter = 0f;
        }

        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimeCounter > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (feetPos != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(feetPos.position, checkRadius);
        }
    }
}