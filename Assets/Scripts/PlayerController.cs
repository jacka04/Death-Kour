using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed = 6f;
    
    public float acceleration = 25f; 
    public float deceleration = 30f; 
    public float velPower = 0.9f;    
    
    public float jumpForce = 12f;
    
    public float jumpCutMultiplier = 0.3f;
    
    public float jumpHoldGravityScale = 0.4f; 
    private float originalGravityScale; 
    
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
        originalGravityScale = rb.gravityScale;
    }

    void FixedUpdate()
    {
        moveInput = Input.GetAxisRaw("Horizontal"); 

        float targetSpeed = moveInput * speed;

        float speedDif = targetSpeed - rb.linearVelocity.x;

        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        
        float movement = speedDif * accelRate;
        
        movement = Mathf.Pow(Mathf.Abs(movement), velPower) * Mathf.Sign(movement);
        
        rb.AddForce(movement * Vector2.right);
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

        if (!Input.GetKey(KeyCode.Space) || rb.gravityScale == originalGravityScale || rb.linearVelocity.y < 0)
        {
            rb.gravityScale = originalGravityScale; 

            if (rb.linearVelocity.y < 0)
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.deltaTime;
            }
            else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space)) 
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.deltaTime;
            }
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
                rb.gravityScale = originalGravityScale * jumpHoldGravityScale;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
                rb.gravityScale = originalGravityScale; 
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
            rb.gravityScale = originalGravityScale; 
            
            if (rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            }
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