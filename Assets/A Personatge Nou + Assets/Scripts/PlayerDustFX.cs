using UnityEngine;

/// <summary>
/// Gestiona los efectos de polvo en los pies del jugador.
/// Coloca este script en un GameObject hijo del player posicionado a la altura de los pies.
/// </summary>
public class PlayerDustFX : MonoBehaviour
{
    [Header("Polvo al Correr")]
    [SerializeField] private Color runDustColor    = new Color(0.88f, 0.90f, 0.95f, 0.85f);
    [SerializeField] private float runDustSize     = 0.28f;
    [SerializeField] private float runDustLifetime = 0.50f;
    [SerializeField] private float runDustSpeed    = 2.2f;
    [SerializeField] private int   runDustRate     = 28;

    [Header("Polvo al Saltar")]
    [SerializeField] private Color jumpDustColor    = new Color(0.60f, 0.65f, 0.80f, 0.95f);
    [SerializeField] private float jumpDustSize     = 0.40f;
    [SerializeField] private float jumpDustLifetime = 0.55f;
    [SerializeField] private float jumpDustSpeed    = 3.5f;
    [SerializeField] private int   jumpDustBurst    = 22;

    [Header("Polvo al Aterrizar")]
    [SerializeField] private Color landDustColor    = new Color(0.82f, 0.86f, 0.96f, 1.00f);
    [SerializeField] private float landDustSize     = 0.42f;
    [SerializeField] private float landDustLifetime = 0.60f;
    [SerializeField] private float landDustSpeed    = 4.2f;
    [SerializeField] private int   landDustBurst    = 30;

    private ParticleSystem runPS;
    private ParticleSystem jumpPS;
    private ParticleSystem landPS;

    private CelestePlayer player;
    private int lastFacing = 1;

    private void Awake()
    {
        player = GetComponentInParent<CelestePlayer>();

        runPS  = BuildPS("RunDust",  runDustColor,  runDustSize,  runDustLifetime, runDustSpeed,  coneAngle: 22f);
        jumpPS = BuildPS("JumpDust", jumpDustColor, jumpDustSize, jumpDustLifetime, jumpDustSpeed, coneAngle: 80f);
        landPS = BuildPS("LandDust", landDustColor, landDustSize, landDustLifetime, landDustSpeed, coneAngle: 85f);

        SetRate(runPS,  0);
        SetRate(jumpPS, 0);
        SetRate(landPS, 0);

        UpdateRunDirection();
    }

    private void Update()
    {
        if (player == null) return;

        bool isRunning = player.IsOnGround && Mathf.Abs(player.Speed.x) > 1.5f;
        SetRate(runPS, isRunning ? runDustRate : 0);

        if (player.Facing != lastFacing)
        {
            lastFacing = player.Facing;
            UpdateRunDirection();
        }
    }

    private void UpdateRunDirection()
    {
        // El cono de partículas apunta en la dirección contraria al movimiento.
        // facing = 1 (derecha) → partículas hacia la izquierda: rotación Y = +90
        // facing = -1 (izquierda) → partículas hacia la derecha: rotación Y = -90
        if (runPS != null)
            runPS.transform.localRotation = Quaternion.Euler(75f, lastFacing * -90f, 0f);
    }

    /// <summary>Llamado desde CelestePlayer cuando el jugador salta.</summary>
    public void PlayJump()
    {
        jumpPS.Emit(jumpDustBurst);
    }

    /// <summary>Llamado desde CelestePlayer cuando el jugador aterriza.</summary>
    public void PlayLand()
    {
        landPS.Emit(landDustBurst);
    }

    private static void SetRate(ParticleSystem ps, float rate)
    {
        var em = ps.emission;
        em.rateOverTime = rate;
    }

    private ParticleSystem BuildPS(string psName, Color col, float size, float lifetime, float speed, float coneAngle)
    {
        var go = new GameObject(psName);
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;

        var ps   = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.loop            = true;
        main.playOnAwake     = false;
        main.startLifetime   = new ParticleSystem.MinMaxCurve(lifetime * 0.6f, lifetime);
        main.startSize       = new ParticleSystem.MinMaxCurve(size * 0.5f, size);
        main.startSpeed      = new ParticleSystem.MinMaxCurve(speed * 0.4f, speed);
        main.startColor      = col;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles    = 80;
        main.gravityModifier = 0.4f;
        main.startRotation   = new ParticleSystem.MinMaxCurve(0f, 2f * Mathf.PI);

        var shape       = ps.shape;
        shape.enabled   = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle     = coneAngle;
        shape.radius    = 0.08f;

        var sol     = ps.sizeOverLifetime;
        sol.enabled = true;
        sol.size    = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0f));

        var colOverLife     = ps.colorOverLifetime;
        colOverLife.enabled = true;
        var grad            = new Gradient();
        grad.SetKeys(
            new[] { new GradientColorKey(col, 0f), new GradientColorKey(col, 1f) },
            new[] { new GradientAlphaKey(col.a, 0f), new GradientAlphaKey(col.a * 0.6f, 0.5f), new GradientAlphaKey(0f, 1f) }
        );
        colOverLife.color = grad;

        var rend        = go.GetComponent<ParticleSystemRenderer>();
        rend.renderMode = ParticleSystemRenderMode.Billboard;
        rend.material   = CreateDustMaterial(col);
        rend.sortingOrder = 2;

        ps.Play();
        return ps;
    }

    private static Material CreateDustMaterial(Color color)
    {
        Shader sh = Shader.Find("Particles/Standard Unlit")
                 ?? Shader.Find("Legacy Shaders/Particles/Alpha Blended")
                 ?? Shader.Find("Sprites/Default");

        var mat = new Material(sh != null ? sh : Shader.Find("Standard"));
        mat.color       = color;
        mat.renderQueue = 3000;
        return mat;
    }
}
