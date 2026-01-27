using System;
using UnityEngine;

namespace PlayerSystem
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private ScriptableStats _stats;
        [SerializeField] private CoinManager _coinManager;

        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private Renderer _renderer;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;

        private bool _isDead = false;
        private Vector2 _respawnPoint;

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;

        private float _time;

        private bool _dashToConsume;
        private bool _isDashing;
        private int _dashesRemaining;
        private float _timeDashStarted;
        private Vector2 _dashDirection;

        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;
        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<CapsuleCollider2D>();
            _renderer = GetComponent<Renderer>();
            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
            _dashesRemaining = _stats.MaxAirDashes;

            _respawnPoint = transform.position;

            if (_coinManager == null)
            {
                _coinManager = FindFirstObjectByType<CoinManager>();
            }
        }

        private void Update()
        {
            if (_isDead) return;

            _time += Time.deltaTime;
            GatherInput();
        }

        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
                DashDown = Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.X),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };

            if (_stats.SnapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
            }

            if (_frameInput.DashDown && _dashesRemaining > 0)
            {
                _dashToConsume = true;
            }
        }

        private void FixedUpdate()
        {
            if (_isDead) return;

            CheckCollisions();
            HandleJump();
            HandleDash();
            HandleDirection();
            HandleGravity();
            ApplyMovement();
        }

        public void ApplyExternalImpulse(float force)
        {
            _endedJumpEarly = false;
            _frameVelocity.y = force;
        }

        public void Die()
        {
            if (_isDead) return;

            _isDead = true;
            _rb.linearVelocity = Vector2.zero;
            _frameVelocity = Vector2.zero;
            _rb.bodyType = RigidbodyType2D.Static;

            if (_renderer != null) _renderer.enabled = false;

            Invoke(nameof(Respawn), 0.5f);
        }

        private void Respawn()
        {
            transform.position = _respawnPoint;

            _isDead = false;
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.linearVelocity = Vector2.zero;
            _frameVelocity = Vector2.zero;
            _dashesRemaining = _stats.MaxAirDashes;
            _isDashing = false;

            if (_renderer != null) _renderer.enabled = true;
        }

        public void ActualizarCheckpoint(Vector2 nuevaPos)
        {
            _respawnPoint = nuevaPos;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Spike") || collision.CompareTag("Trap"))
            {
                Die();
            }
            else if (collision.CompareTag("Coin"))
            {
                if (_coinManager != null)
                {
                    _coinManager.AddCoin();
                    Destroy(collision.gameObject);
                }
            }
        }

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);

            if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

            if (!_grounded && groundHit)
            {
                _grounded = true;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                _dashesRemaining = _stats.MaxAirDashes;
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            }
            else if (_grounded && !groundHit)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

        private void HandleJump()
        {
            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.linearVelocity.y > 0) _endedJumpEarly = true;

            if (!_jumpToConsume && !HasBufferedJump) return;

            if (_grounded || CanUseCoyote) ExecuteJump();

            _jumpToConsume = false;
        }

        private void ExecuteJump()
        {
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _frameVelocity.y = _stats.JumpPower;
            Jumped?.Invoke();
        }

        private void HandleDash()
        {
            if (_dashToConsume && _dashesRemaining > 0)
            {
                if (_frameInput.Move.sqrMagnitude > 0.001f)
                {
                    _dashDirection = _frameInput.Move.normalized;
                }
                else
                {
                    _dashDirection = new Vector2(Mathf.Sign(transform.localScale.x), 0);
                }

                _isDashing = true;
                _dashesRemaining--;
                _timeDashStarted = _time;
                _endedJumpEarly = false;
            }

            _dashToConsume = false;

            if (_isDashing)
            {
                _frameVelocity = _dashDirection * _stats.DashSpeed;

                if (_time >= _timeDashStarted + _stats.DashDuration)
                {
                    _isDashing = false;
                    _frameVelocity *= 0.5f;
                }
            }
        }

        private void HandleDirection()
        {
            if (_isDashing && !_stats.DashAllowSteer) return;

            if (_frameInput.Move.x == 0)
            {
                var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
            }
        }

        private void HandleGravity()
        {
            if (_isDashing && _stats.DashCancelsGravity)
            {
                return;
            }

            if (_grounded && _frameVelocity.y <= 0f)
            {
                _frameVelocity.y = -0.1f;
            }
            else
            {
                var inAirGravity = _stats.FallAcceleration;
                if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;

                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        private void ApplyMovement()
        {
            _rb.linearVelocity = _frameVelocity;
        }
    }

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public bool DashDown;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;
        public Vector2 FrameInput { get; }
    }
}