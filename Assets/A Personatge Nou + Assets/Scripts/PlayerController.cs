using UnityEngine;
using System.Collections;

public class MadelineController : MonoBehaviour
{
    // --- CONSTANTES ORIGINALES DE CELESTE ---
    private const float MaxFall = 1.6f;
    private const float Gravity = 9.0f;
    private const float HalfGravThreshold = 0.4f;
    private const float MaxRun = 0.9f;
    private const float RunAccel = 10.0f;
    private const float RunReduce = 4.0f;
    private const float AirMult = 0.65f;
    private const float JumpSpeed = 1.05f;
    private const float VarJumpTime = 0.2f;
    private const float DashSpeed = 2.4f;
    private const float DashTime = 0.15f;
    private const float ClimbMaxStamina = 110f;
    private const float ClimbUpSpeed = 0.45f;
    private const float ClimbDownSpeed = 0.8f;
    private const float WallJumpHSpeed = MaxRun + 0.4f;

    // --- ESTADOS ---
    public enum State { Normal, Climb, Dash }
    public State currentState = State.Normal;

    // --- VARIABLES DE CONTROL ---
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private float stamina = ClimbMaxStamina;
    private float dashCooldownTimer;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private float varJumpTimer;
    private int dashes;
    private int facing = 1;

    [Header("Detection")]
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask solidLayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Manejamos la gravedad manualmente como en el original
    }

    void Update()
    {
        Inputs();
        StateMachine();
    }

    void Inputs()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (moveInput.x != 0) facing = (int)moveInput.x;

        // Timers de asistencia
        if (isGrounded)
        {
            coyoteTimer = 0.1f;
            dashes = 1;
            stamina = ClimbMaxStamina;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump")) jumpBufferTimer = 0.1f;
        else jumpBufferTimer -= Time.deltaTime;
    }

    void StateMachine()
    {
        switch (currentState)
        {
            case State.Normal: NormalUpdate(); break;
            case State.Climb: ClimbUpdate(); break;
            case State.Dash: break; // Manejado por Corrutina
        }
    }

    // --- L�GICA DE MOVIMIENTO NORMAL ---
    void NormalUpdate()
    {
        // 1. Check Transiciones
        if (Input.GetKeyDown(KeyCode.X) && dashes > 0) StartCoroutine(DashRoutine());
        if (Input.GetKey(KeyCode.Z) && IsTouchingWall() && stamina > 0)
        {
            currentState = State.Climb;
            return;
        }

        // 2. Movimiento Horizontal con Inercia de Celeste
        float mult = isGrounded ? 1 : AirMult;
        float targetSpeed = moveInput.x * MaxRun;
        float accel = Mathf.Abs(rb.linearVelocity.x) > MaxRun && Mathf.Sign(rb.linearVelocity.x) == moveInput.x ? RunReduce : RunAccel;

        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accel * mult * Time.deltaTime);

        // 3. Gravedad Variable (Apex del salto es m�s ligero)
        float grav = Gravity;
        if (!isGrounded && Mathf.Abs(rb.linearVelocity.y) < HalfGravThreshold && Input.GetButton("Jump")) grav *= 0.5f;

        float newY = Mathf.MoveTowards(rb.linearVelocity.y, -MaxFall, grav * Time.deltaTime);

        rb.linearVelocity = new Vector2(newX, newY);

        // 4. Salto / Variable Jump
        if (jumpBufferTimer > 0 && coyoteTimer > 0)
        {
            MadelineJump();
        }

        if (varJumpTimer > 0)
        {
            if (Input.GetButton("Jump")) rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpSpeed);
            else varJumpTimer = 0;
            varJumpTimer -= Time.deltaTime;
        }

        CollisionCheck();
    }

    // --- L�GICA DE ESCALADA (CLIMB) ---
    void ClimbUpdate()
    {
        if (!Input.GetKey(KeyCode.Z) || !IsTouchingWall() || stamina <= 0)
        {
            currentState = State.Normal;
            return;
        }

        float climbDir = moveInput.y;
        stamina -= (climbDir > 0 ? 45f : 10f) * Time.deltaTime;

        rb.linearVelocity = new Vector2(0, climbDir * (climbDir > 0 ? ClimbUpSpeed : -ClimbDownSpeed));

        if (Input.GetButtonDown("Jump"))
        {
            stamina -= 27f;
            rb.linearVelocity = new Vector2(-facing * WallJumpHSpeed, JumpSpeed);
            currentState = State.Normal;
        }
    }

    // --- DASH (CORRUTINA ID�NTICA) ---
    IEnumerator DashRoutine()
    {
        currentState = State.Dash;
        dashes--;
        Vector2 dir = moveInput == Vector2.zero ? new Vector2(facing, 0) : moveInput.normalized;

        rb.linearVelocity = dir * DashSpeed;

        // El "Freeze" frame de Celeste
        Time.timeScale = 0.05f;
        yield return new WaitForSecondsRealtime(0.05f);
        Time.timeScale = 1f;

        yield return new WaitForSeconds(DashTime);

        rb.linearVelocity = dir * MaxRun; // Salida del dash
        currentState = State.Normal;
    }

    void MadelineJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpSpeed);
        jumpBufferTimer = 0;
        coyoteTimer = 0;
        varJumpTimer = VarJumpTime;
    }

    // --- DETECCI�N PIXEL-PERFECT (SIMULADA) ---
    void CollisionCheck()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(0.8f, 0.1f), 0, solidLayer);

        // Corner Correction (Empuje hacia arriba si golpeas la cabeza en una esquina)
        if (rb.linearVelocity.y > 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.up * 0.5f, Vector2.up, 0.2f, solidLayer);
            if (hit)
            {
                // Si hay espacio a los lados, desplazar Madeline lateralmente para "esquivar" la esquina
                transform.position += new Vector3(facing * -0.1f, 0, 0);
            }
        }
    }

    bool IsTouchingWall() => Physics2D.OverlapCircle(wallCheck.position, 0.2f, solidLayer);
}