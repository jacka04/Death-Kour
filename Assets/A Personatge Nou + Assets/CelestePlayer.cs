using System.Collections;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class CelestePlayer : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // ESTADOS
    // -------------------------------------------------------------------------
    private enum State
    {
        Normal,
        Dash,
        Climb,
        WallSlide
    }

    // -------------------------------------------------------------------------
    // CONSTANTES — valores originales de Player.cs de Celeste
    // -------------------------------------------------------------------------

    // Gravedad
    private const float Gravity            = 75f;
    private const float HalfGravThreshold  = 40f;   // si Speed.Y < esto y Jump held → mitad de gravedad
    private const float MaxFall            = 20f;
    private const float FastMaxFall        = 200f;
    private const float FastMaxAccel       = 250f;

    // Correr
    private const float MaxRun    = 12f;
    private const float RunAccel  = 950f;
    private const float RunReduce = 450f;
    private const float AirMult   = 0.65f;

    // Salto normal
    private const float JumpSpeed      = 15f;
    private const float JumpHBoost     = 8f;
    private const float VarJumpTime    = 0.2f;
    private const float JumpGraceTime  = 1f;   // coyote time
    private const int   UpwardCornerCorrection = 4;

    // Wall Jump
    private const int   WallJumpCheckDist  = 3;
    private const float WallJumpForceTime  = 0.16f;
    private const float WallJumpHSpeed     = MaxRun + JumpHBoost;  // 130 u/s
    private const float WallSpeedRetentionTime = 0.06f;

    // Wall Slide
    private const float WallSlideStartMax = 10f;
    private const float WallSlideTime     = 1.2f;

    // Super Wall Jump
    private const float SuperWallJumpSpeed   = -160f;
    private const float SuperWallJumpVarTime = 0.25f;
    private const float SuperWallJumpForceTime = 0.2f;
    private const float SuperWallJumpH       = MaxRun + JumpHBoost * 2f; 

    // Climb
    private const float ClimbMaxStamina    = 110f;
    private const float ClimbUpCost        = 100f / 2.2f;
    private const float ClimbStillCost     = 100f / 10f;
    private const float ClimbJumpCost      = 110f / 4f;
    private const int   ClimbCheckDist     = 2;
    private const float ClimbNoMoveTime    = 0.05f;
    public  const float ClimbTiredThreshold = 20f;
    private const float ClimbUpSpeed       = 4.5f;
    private const float ClimbDownSpeed     = -8f;
    private const float ClimbSlipSpeed     = -3f;
    private const float ClimbAccel         = 900f;
    private const float ClimbGrabYMult     = 0.2f;
    private const float ClimbHopY          = 12f;
    private const float ClimbHopX          = 100f;
    private const float ClimbHopForceTime  = 0.2f;
    private const float ClimbJumpBoostTime = 0.2f;

    // Dash
    private const float DashSpeed              = 20f;
    private const float EndDashSpeed           = 9f;
    private const float EndDashUpMult          = 1f;
    private const float DashTime               = 0.10f;
    private const float DashCooldown           = 0.4f;
    private const float DashRefillCooldown     = 0.1f;
    private const int   DashCornerCorrection    = 4;
    private const float DashAttackTime         = 0.3f;
    private const float DodgeSlideSpeedMult    = 1.2f;

    //animmacions
    [Header("Animación")]
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer sprite;

    // -------------------------------------------------------------------------
    // INSPECTOR
    // -------------------------------------------------------------------------
    [Header("Refs")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Estado (solo lectura)")]
    [SerializeField] private State currentState = State.Normal;
    [SerializeField] private float stamina;
    [SerializeField] private int   dashes = 1;

    // -------------------------------------------------------------------------
    // COMPONENTES
    // -------------------------------------------------------------------------
    private CharacterController cc;

    // -------------------------------------------------------------------------
    // RUNTIME — velocidad y timers
    // -------------------------------------------------------------------------

    private const float GroundedGraceTime = 0.08f;
private float coyoteGroundedTimer;
private bool animIsGrounded;
    private Vector2 speed;          // velocidad lógica (unidades/segundo)
    private int     facing = 1;     // 1 = derecha, -1 = izquierda

    // Salto / coyote
    private float jumpGraceTimer;
    private float varJumpTimer;
    private float varJumpSpeed;
    private bool  onGround;

    // Wall slide
    private float wallSlideTimer = WallSlideTime;
    private int   wallSlideDir;

    // Wall speed retention
    private float wallSpeedRetentionTimer;
    private float wallSpeedRetained;

    // Wall boost (climb jump lateral)
    private int   wallBoostDir;
    private float wallBoostTimer;

    // Force move X (post wall-jump)
    private int   forceMoveX;
    private float forceMoveXTimer;

    // Dash
    private int   dashCount = 1;
    private float dashCooldownTimer;
    private float dashRefillCooldownTimer;
    private float dashAttackTimer;
    private Vector2 dashDir;
    private bool  dashStartedOnGround;
    private bool  wasDashStarted;

    // Climb
    private float climbNoMoveTimer;
    private int   lastClimbMove;
    private bool  isTouchingWall;
    private int   wallDir;           // dirección del muro tocado
    private float climbTimer;        // para evitar trepar al instante al agarrar

    // -------------------------------------------------------------------------
    // RESPAWN / MUERTE
    // -------------------------------------------------------------------------
    private Vector3 respawnPoint;
    private bool isDead = false;

    private void Start()
    {
        respawnPoint = transform.position;
    }

    public void ActualizarCheckpoint(Vector3 newPosition)
    {
        respawnPoint = newPosition;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        // Aquí puedes añadir: animación de muerte, fade, etc.
        cc.enabled = false;
        speed = Vector2.zero;

        yield return new WaitForSeconds(0.8f); // pausa antes de reaparecer

        transform.position = respawnPoint;
        cc.enabled = true;

        // Resetear estado
        currentState = State.Normal;
        stamina = ClimbMaxStamina;
        dashes = dashCount;
        speed = Vector2.zero;
        varJumpTimer = 0f;
        jumpGraceTimer = 0f;
        dashCooldownTimer = 0f;

        isDead = false;
    }
    // Dash coroutine
    private Coroutine dashCoroutine;

    // Max fall dinámico (fast fall)
    private float maxFall;

    // -------------------------------------------------------------------------
    // INPUT (simple wrappers — sustituye por tu Input System favorito)
    // -------------------------------------------------------------------------
    private float InputX      => Input.GetAxisRaw("Horizontal");
    private float InputY      => Input.GetAxisRaw("Vertical");
    private bool  JumpPressed => Input.GetButtonDown("Jump");
    private bool  JumpHeld    => Input.GetButton("Jump");
    private bool  GrabHeld    => Input.GetButton("Fire1");   // asigna "Fire1" al grab
    private bool  DashPressed => Input.GetButtonDown("Fire2"); // asigna "Fire2" al dash

    private int MoveX
    {
        get
        {
            if (forceMoveXTimer > 0) return forceMoveX;
            return Mathf.RoundToInt(InputX);
        }
    }

    // -------------------------------------------------------------------------
    // UNITY LIFECYCLE
    // -------------------------------------------------------------------------
    private void Awake()
    {
        cc       = GetComponent<CharacterController>();
        stamina  = ClimbMaxStamina;
        maxFall  = MaxFall;
        dashes   = dashCount;
    }

    private void Update()
    {
        UpdateTimers();
        UpdateGroundCheck();
        UpdateWallCheck();
        UpdateDashRefill();

        switch (currentState)
        {
            case State.Normal:    UpdateNormal();    break;
            case State.Dash:      /* manejado por coroutine */ break;
            case State.Climb:     UpdateClimb();     break;
            case State.WallSlide: UpdateWallSlide(); break;
        }

        // Aplica movimiento al CharacterController
        cc.Move(new Vector3(speed.x, speed.y, 0f) * Time.deltaTime);
        Vector3 pos = transform.position;
pos.z = 0f;
transform.position = pos;
    }
    private void LateUpdate() 
    {
        UpdateAnimations();
    }

   private void UpdateAnimations()
{
    if (anim == null || sprite == null) return;

    sprite.flipX = (facing == -1);

    bool isClimbing = currentState == State.Climb;
bool isRunning = onGround && Mathf.Abs(speed.x) > 0.1f;

    anim.SetBool("isRunning",      isRunning);
    anim.SetBool("isGrounded",     animIsGrounded);
    anim.SetFloat("verticalSpeed", speed.y);
    anim.SetBool("isClimbing",     isClimbing);

    if (isClimbing)
        anim.speed = Mathf.Abs(InputY) > 0.1f ? 1f : 0f;
    else
        anim.speed = 1f;
}
    

    // -------------------------------------------------------------------------
    // TIMERS GENERALES
    // -------------------------------------------------------------------------
    private void UpdateTimers()
    {
        float dt = Time.deltaTime;

        if (jumpGraceTimer    > 0) jumpGraceTimer    -= dt;
        if (varJumpTimer      > 0) varJumpTimer      -= dt;
        if (dashCooldownTimer > 0) dashCooldownTimer -= dt;
        if (dashRefillCooldownTimer > 0) dashRefillCooldownTimer -= dt;
        if (dashAttackTimer   > 0) dashAttackTimer   -= dt;
        if (forceMoveXTimer   > 0) forceMoveXTimer   -= dt;
        if (wallBoostTimer    > 0)
        {
            wallBoostTimer -= dt;
            // Si el jugador pulsa hacia el lado del wall boost, convertir en wall jump speed
            if (MoveX == wallBoostDir)
            {
                speed.x      = WallJumpHSpeed * MoveX;
                stamina      += ClimbJumpCost;
                wallBoostTimer = 0f;
            }
        }
        if (wallSpeedRetentionTimer > 0)
        {
            if (Mathf.Sign(speed.x) == -Mathf.Sign(wallSpeedRetained))
                wallSpeedRetentionTimer = 0f;
            else if (!CheckWallInDir(Mathf.RoundToInt(Mathf.Sign(wallSpeedRetained))))
            {
                speed.x = wallSpeedRetained;
                wallSpeedRetentionTimer = 0f;
            }
            else
                wallSpeedRetentionTimer -= dt;
        }

        // Wall slide decay
        if (wallSlideDir != 0)
        {
            wallSlideTimer = Mathf.Max(wallSlideTimer - dt, 0f);
            wallSlideDir   = 0;
        }
    }

    // -------------------------------------------------------------------------
    // GROUND / WALL CHECKS
    // -------------------------------------------------------------------------
    private void UpdateGroundCheck()
{
    bool rawGround = cc.isGrounded;

    if (rawGround)
        coyoteGroundedTimer = GroundedGraceTime;
    else if (coyoteGroundedTimer > 0)
        coyoteGroundedTimer -= Time.deltaTime;

    onGround       = rawGround;
    animIsGrounded = coyoteGroundedTimer > 0f;

    if (onGround)
    {
        jumpGraceTimer = JumpGraceTime;
        wallSlideTimer = WallSlideTime;
        maxFall        = MaxFall;
        if (dashRefillCooldownTimer <= 0)
            RefillDash();
        stamina = ClimbMaxStamina;
    }
}
    private void UpdateWallCheck()
    {
        isTouchingWall = false;
        wallDir        = 0;

        // Detecta muro a la derecha o izquierda con un pequeño raycast
        float checkDist = 0.9f;
        Debug.Log($"checkDist={checkDist} | radius={cc.radius}");
        Debug.DrawRay(transform.position, Vector3.right * checkDist, Color.red);
        Debug.DrawRay(transform.position, Vector3.left * checkDist, Color.blue);
        if (Physics.Raycast(transform.position, Vector3.right, checkDist, wallLayer))
        {
            isTouchingWall = true;
            wallDir        = 1;
        }
        else if (Physics.Raycast(transform.position, Vector3.left, checkDist, wallLayer))
        {
            isTouchingWall = true;
            wallDir        = -1;
        }
        Debug.DrawRay(transform.position, Vector3.right * 0.6f, Color.red);
        Debug.DrawRay(transform.position, Vector3.left * 0.6f, Color.blue);
    }

    private bool CheckWallInDir(int dir)
    {
        if (dir == 0) return false;
        float checkDist = 1.5f;
        Vector3 d = dir > 0 ? Vector3.right : Vector3.left;
        return Physics.Raycast(transform.position, d, checkDist, wallLayer);
    }

    // -------------------------------------------------------------------------
    // DASH REFILL
    // -------------------------------------------------------------------------
    private void UpdateDashRefill()
    {
        if (dashRefillCooldownTimer <= 0 && onGround && currentState != State.Dash)
            RefillDash();
    }

    private bool RefillDash()
    {
        if (dashes < dashCount)
        {
            dashes = dashCount;
            return true;
        }
        return false;
    }

    // -------------------------------------------------------------------------
    // STATE: NORMAL
    // -------------------------------------------------------------------------
    private void UpdateNormal()
    {
        Debug.Log($"GrabHeld={GrabHeld} | isTouchingWall={isTouchingWall} | wallDir={wallDir} | facing={facing} | speedY={speed.y:F1}");
        Debug.DrawRay(transform.position, Vector3.right * 0.6f, Color.red);
        Debug.DrawRay(transform.position, Vector3.left * 0.6f, Color.blue);
        float dt = Time.deltaTime;

        // --- Grab / Climb ---
        if (GrabHeld && isTouchingWall && Mathf.Sign(speed.x) != -facing)
        {
            if (CheckWallInDir(facing))
    {
        EnterClimb();
        return;
    }
}

        // --- Dash ---
        if (CanDash)
        {
            StartDash();
            return;
        }

        // --- Horizontal ---
        float mult = onGround ? 1f : AirMult;
        if (Mathf.Abs(speed.x) > MaxRun && Mathf.Sign(speed.x) == MoveX)
            speed.x = Approach(speed.x, MaxRun * MoveX, RunReduce * mult * dt);
        else
            speed.x = Approach(speed.x, MaxRun * MoveX, RunAccel * mult * dt);

        // Facing
        if (MoveX != 0) facing = MoveX;

        // --- Gravedad / Fast Fall ---
        if (!onGround)
        {
            // Fast fall
            if (InputY < -0.5f && speed.y <= -maxFall)
                maxFall = Approach(maxFall, FastMaxFall, FastMaxAccel * dt);
            else
                maxFall = Approach(maxFall, MaxFall, FastMaxAccel * dt);

            bool isDashing = currentState == State.Dash;
            float gravMult = (!isDashing && Mathf.Abs(speed.y) < HalfGravThreshold && JumpHeld) ? 0.5f : 1f;            speed.y = Approach(speed.y, -maxFall, Gravity * gravMult * dt);
        }
        else
        {
            speed.y = Mathf.Min(speed.y, 0f);
        }

        // --- Variable Jump ---
        if (varJumpTimer > 0f && currentState != State.Dash)
{
    if (JumpHeld)
        speed.y = Mathf.Min(speed.y, varJumpSpeed);
    else
        varJumpTimer = 0f;
}

        // --- Wall Slide ---
        UpdateWallSlideCheck();

        // --- Salto ---
        if (JumpPressed)
        {
            if (jumpGraceTimer > 0f)
            {
                Jump();
            }
            else
            {
                // Wall jump derecha
                if (CheckWallInDir(1))
                {
                    if (facing == 1 && GrabHeld && stamina > 0f)
                        ClimbJump();
                    else if (IsDashingUp())
                        SuperWallJump(-1);
                    else
                        WallJump(-1);
                }
                // Wall jump izquierda
                else if (CheckWallInDir(-1))
                {
                    if (facing == -1 && GrabHeld && stamina > 0f)
                        ClimbJump();
                    else if (IsDashingUp())
                        SuperWallJump(1);
                    else
                        WallJump(1);
                }
            }
        }
    }

    // -------------------------------------------------------------------------
    // WALL SLIDE (dentro de Normal)
    // -------------------------------------------------------------------------
    private void UpdateWallSlideCheck()
    {
        if ((MoveX == facing || (MoveX == 0 && GrabHeld)) && InputY >= -0.1f)
        {
            if (speed.y >= 0f && wallSlideTimer > 0f && CheckWallInDir(facing))
            {
                wallSlideDir = facing;
            }
        }

        if (wallSlideDir != 0)
        {
            float maxSlide = Mathf.Lerp(MaxFall, WallSlideStartMax, wallSlideTimer / WallSlideTime);
            speed.y = Mathf.Min(speed.y, maxSlide);
        }
    }

    private void UpdateWallSlide()
    {
        // Redirige al estado normal; la lógica de slide está en UpdateNormal
        currentState = State.Normal;
    }

    // -------------------------------------------------------------------------
    // JUMP
    // -------------------------------------------------------------------------
    private void Jump(bool particles = true)
    {
        jumpGraceTimer = 0f;
        varJumpTimer   = VarJumpTime;
        wallSlideTimer = WallSlideTime;
        wallBoostTimer = 0f;
        dashAttackTimer = 0f;

        speed.x    += JumpHBoost * MoveX;
        speed.y     = JumpSpeed;
        varJumpSpeed = speed.y;
    }

    private void WallJump(int dir)
    {
        jumpGraceTimer  = 0f;
        varJumpTimer    = VarJumpTime;
        wallSlideTimer  = WallSlideTime;
        wallBoostTimer  = 0f;
        dashAttackTimer = 0f;

        if (MoveX != 0)
        {
            forceMoveX      = dir;
            forceMoveXTimer = WallJumpForceTime;
        }

        speed.x      = WallJumpHSpeed * dir;
        speed.y      = JumpSpeed;
        varJumpSpeed = speed.y;

        facing = dir;
    }

    private void SuperWallJump(int dir)
    {
        jumpGraceTimer  = 0f;
        varJumpTimer    = SuperWallJumpVarTime;
        wallSlideTimer  = WallSlideTime;
        wallBoostTimer  = 0f;
        dashAttackTimer = 0f;

        speed.x      = SuperWallJumpH * dir;
        speed.y      = SuperWallJumpSpeed;
        varJumpSpeed = speed.y;

        facing = dir;
    }

    private void ClimbJump()
    {
        if (!onGround)
            stamina -= ClimbJumpCost;

        // Reutiliza Jump normal
        jumpGraceTimer  = 0f;
        varJumpTimer    = VarJumpTime;
        wallSlideTimer  = WallSlideTime;
        wallBoostTimer  = 0f;
        dashAttackTimer = 0f;

        speed.x      += JumpHBoost * MoveX;
        speed.y       = JumpSpeed;
        varJumpSpeed  = speed.y;

        if (MoveX == 0)
        {
            wallBoostDir   = -facing;
            wallBoostTimer = ClimbJumpBoostTime;
        }

        currentState = State.Normal;
    }

    // -------------------------------------------------------------------------
    // DASH
    // -------------------------------------------------------------------------
    private bool CanDash => DashPressed && dashCooldownTimer <= 0f && dashes > 0;

    private bool IsDashingUp() => dashDir.x == 0f && dashDir.y < 0f && dashAttackTimer > 0f;

    private void StartDash()
    {
        dashes            = Mathf.Max(0, dashes - 1);
        dashCooldownTimer = DashCooldown;
        dashRefillCooldownTimer = DashRefillCooldown;
        dashStartedOnGround = onGround;
        dashAttackTimer   = DashAttackTime;
        currentState      = State.Dash;
        speed             = Vector2.zero;

        if (dashCoroutine != null)
            StopCoroutine(dashCoroutine);
        dashCoroutine = StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        // Primer frame: calcular dirección
        yield return null;

        // Dirección de aim (8 direcciones)
        Vector2 aim = GetAimVector();
        Vector2 newSpeed = aim * DashSpeed;

        speed  = newSpeed;
        dashDir = aim;

        if (dashDir.x != 0f)
            facing = Mathf.RoundToInt(dashDir.x);

        // Dash slide: si vamos diagonal abajo en suelo, convertir en slide horizontal
        if (onGround && dashDir.x != 0f && dashDir.y > 0f && speed.y > 0f)
        {
            dashDir = new Vector2(Mathf.Sign(dashDir.x), 0f);
            speed.y = 0f;
            speed.x *= DodgeSlideSpeedMult;
        }

        // Esperar duración del dash
        float timer = 0f;
        while (timer < DashTime)
        {
            timer += Time.deltaTime;

            // Wall jump durante dash (dirección pura horizontal o vertical)
            if (dashDir.x == 0f && dashDir.y < 0f)
            {
                if (JumpPressed)
                {
                    if (CheckWallInDir(1))  { SuperWallJump(-1); currentState = State.Normal; yield break; }
                    if (CheckWallInDir(-1)) { SuperWallJump(1);  currentState = State.Normal; yield break; }
                }
            }
            else
            {
                if (JumpPressed)
                {
                    if (CheckWallInDir(1))  { WallJump(-1); currentState = State.Normal; yield break; }
                    if (CheckWallInDir(-1)) { WallJump(1);  currentState = State.Normal; yield break; }
                }
            }

            // Super Jump durante dash horizontal
            if (dashDir.y == 0f && JumpPressed && jumpGraceTimer > 0f)
            {
                SuperJump();
                currentState = State.Normal;
                yield break;
            }

            yield return null;
        }

        // Post-dash: reducir velocidad
        if (dashDir.y <= 0f)
            speed = dashDir * EndDashSpeed;
        if (speed.y < 0f)
            speed.y *= EndDashUpMult;

        currentState = State.Normal;
    }

    private void SuperJump()
    {
        jumpGraceTimer  = 0f;
        varJumpTimer    = VarJumpTime;
        wallSlideTimer  = WallSlideTime;
        wallBoostTimer  = 0f;
        dashAttackTimer = 0f;

        speed.x      = 260f * facing;
        speed.y      = JumpSpeed;
        varJumpSpeed = speed.y;
    }

    // -------------------------------------------------------------------------
    // CLIMB
    // -------------------------------------------------------------------------
    private void EnterClimb()
    {
        currentState     = State.Climb;
        speed.x          = 0f;
        speed.y         *= ClimbGrabYMult;
        wallSlideTimer   = WallSlideTime;
        climbNoMoveTimer = ClimbNoMoveTime;
        wallBoostTimer   = 0f;
        lastClimbMove    = 0;

        // Snap al muro
        for (int i = 0; i < ClimbCheckDist; i++)
        {
            if (!CheckWallInDir(facing))
                transform.position += new Vector3(facing * 0.1f, 0f, 0f);
            else
                break;
        }
    }

    private void UpdateClimb()
    {
        float dt = Time.deltaTime;
        climbNoMoveTimer -= dt;

        // Refill estamina en suelo
        if (onGround)
            stamina = ClimbMaxStamina;

        // Soltar muro
        if (!GrabHeld)
        {
            currentState = State.Normal;
            return;
        }

        // Muro desapareció
        if (!CheckWallInDir(facing))
        {
            // Climb hop (subir al borde)
            if (speed.y < 0f)
                ClimbHop();
            currentState = State.Normal;
            return;
        }

        // --- Jump desde climb ---
        if (JumpPressed)
        {
            if (MoveX == -facing)
                WallJump(-facing);
            else
                ClimbJump();
            return;
        }

        // --- Dash desde climb ---
        if (CanDash)
        {
            StartDash();
            return;
        }

        // --- Movimiento vertical ---
        float target      = 0f;
        bool  trySlip     = false;

        if (climbNoMoveTimer <= 0f)
        {
            if (InputY > 0.5f)      // arriba (InputY positivo = arriba en Unity)
            {
                target = ClimbUpSpeed;

                // Comprueba bloqueo arriba
                if (CheckCeiling())
                {
                    speed.y = 0f;
                    target  = 0f;
                    trySlip = true;
                }
            }
            else if (InputY < -0.5f) // abajo
            {
                target = ClimbDownSpeed;
                if (onGround)
                {
                    speed.y = 0f;
                    target  = 0f;
                }
            }
            else
                trySlip = true;
        }
        else
            trySlip = true;

        lastClimbMove = (int)Mathf.Sign(target);

        // Slip (resbalar si manos por encima del borde)
        if (trySlip && SlipCheck())
            target = ClimbSlipSpeed;

        speed.y = Approach(speed.y, target, ClimbAccel * dt);

        // Límite bajada voluntaria
        if (InputY >= -0.1f && speed.y > 0f && !CheckWallInDir(facing))
            speed.y = 0f;

        // --- Estamina ---
        if (climbNoMoveTimer <= 0f)
        {
            if (lastClimbMove == -1)          // subiendo (negativo = arriba)
                stamina -= ClimbUpCost * dt;
            else if (lastClimbMove == 0)
                stamina -= ClimbStillCost * dt;
        }

        // Sin estamina → soltar
        if (stamina <= 0f)
        {
            currentState = State.Normal;
            return;
        }
    }

    private void ClimbHop()
    {
        speed.x = facing * ClimbHopX;
        speed.y = Mathf.Min(speed.y, ClimbHopY);
        forceMoveX      = 0;
        forceMoveXTimer = ClimbHopForceTime;
    }
    
    // -------------------------------------------------------------------------
    // HELPERS
    // -------------------------------------------------------------------------

    /// <summary>Obtiene vector de aim en 8 direcciones normalizado.</summary>
    private Vector2 GetAimVector()
    {
        float x = InputX;
        float y = InputY;

        Vector2 aim;
        if (Mathf.Abs(x) < 0.1f && Mathf.Abs(y) < 0.1f)
            aim = new Vector2(facing, 0f);
        else
            aim = new Vector2(x, y).normalized;

        // Snap a 8 direcciones (45°)
        float angle = Mathf.Atan2(aim.y, aim.x);
        float snap  = Mathf.Round(angle / (Mathf.PI / 4f)) * (Mathf.PI / 4f);
        return new Vector2(Mathf.Cos(snap), Mathf.Sin(snap));
    }

    /// <summary>Acercamiento lineal (equivalente a Calc.Approach de Monocle).</summary>
    private static float Approach(float val, float target, float maxMove)
    {
        if (val < target) return Mathf.Min(val + maxMove, target);
        if (val > target) return Mathf.Max(val - maxMove, target);
        return target;
    }

    /// <summary>¿Hay techo encima del jugador?</summary>
    private bool CheckCeiling()
    {
        return Physics.Raycast(
            transform.position,
            Vector3.up,
            cc.height * 0.5f + 0.1f,
            groundLayer
        );
    }

    /// <summary>
    /// Slip check: ¿las manos del jugador están por encima del borde del muro?
    /// (simplificado: comprueba si no hay colisión en la parte superior del muro)
    /// </summary>
    private bool SlipCheck(float addY = 0f)
    {
        Vector3 topOffset = new Vector3(
            facing * (cc.radius + 0.1f),
            cc.height * 0.5f + addY,
            0f
        );
        return !Physics.Raycast(
            transform.position + topOffset,
            Vector3.down,
            0.2f,
            wallLayer
        );
    }

    // -------------------------------------------------------------------------
    // PROPIEDADES PÚBLICAS (útiles para animación, UI, etc.)
    // -------------------------------------------------------------------------
    public bool  IsOnGround      => onGround;
    public bool  IsDashAttacking => dashAttackTimer > 0f;
    public bool  IsClimbing      => currentState == State.Climb;
    public bool  IsWallSliding   => wallSlideDir != 0;
    public bool  IsTired         => stamina < ClimbTiredThreshold;
    public float Stamina         => stamina;
    public int   Dashes          => dashes;
    public int   Facing          => facing;
    public Vector2 Speed         => speed;
}