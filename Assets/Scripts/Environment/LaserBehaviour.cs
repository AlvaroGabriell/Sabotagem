using FMOD.Studio;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(CapsuleCollider))]
public class LaserBehaviour : MonoBehaviour, IDamageSource
{
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private LayerMask hitMask;
    private GameObject[] players;

    private LineRenderer lineRenderer;
    private CapsuleCollider capsuleCollider;

    private Vector3 lastDir, lastPos, lastEndPoint;
    private float checkInterval = 0.05f; // 20x por segundo
    private float checkTimer = 0f;

    private EventInstance laserSound = default;
    private bool _active = true;
    public bool Active
    {
        get => _active;
        set
        {
            _active = value;
            lineRenderer.enabled = value;
            capsuleCollider.enabled = value;

            if (value)
            {
                laserSound = AudioManager.Instance.PlayInstancedSound(AudioEvents.SFX.Laser.LaserEvent, gameObject.transform.position);
            } else
            {
                if(!laserSound.isValid()) return;
                laserSound.stop(STOP_MODE.ALLOWFADEOUT);
                laserSound.release();
                laserSound = default;
            }
        }
    }

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        laserSound = AudioManager.Instance.PlayInstancedSound(AudioEvents.SFX.Laser.LaserEvent, gameObject.transform.position);
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    void Update()
    {
        if (!_active) return;
        UpdateVolume();

        checkTimer -= Time.deltaTime;
        if (checkTimer > 0f) return;
        checkTimer = checkInterval;

        Vector3 currentPos = transform.position;
        Vector3 currentDir = transform.parent.forward;
        
        // Checa hit atual pra ver se algo passou na frente do laser
        Vector3 start = currentPos;
        Vector3 direction = currentDir;
        Vector3 currentEndPoint;

        if(Physics.Raycast(start, direction, out RaycastHit hit, maxDistance, hitMask)) currentEndPoint = hit.point;
        else currentEndPoint = start + direction * maxDistance;

        if(currentDir != lastDir || currentPos != lastPos || currentEndPoint != lastEndPoint)
        {
            lastDir = currentDir;
            lastPos = currentPos;
            lastEndPoint = currentEndPoint;
            FireLaser(start, currentEndPoint);
        }
    }

    private void FireLaser(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        UpdateCollider(start, end);
    }

    private void UpdateCollider(Vector3 start, Vector3 end)
    {
        Vector3 center = (start + end) / 2f;
        float length = Vector3.Distance(start, end);

        capsuleCollider.center = transform.InverseTransformPoint(center);
        capsuleCollider.height = length;
        capsuleCollider.radius = lineRenderer.startWidth / 2f;
        capsuleCollider.direction = 2;

        transform.LookAt(end);
    }

    private void UpdateVolume()
    {
        float nearestDistance = float.MaxValue;

        foreach(var player in players)
        {
            float distance = Vector3.Distance(
                player.transform.position,
                transform.position
            );

            nearestDistance = Mathf.Min(nearestDistance, distance);
        }

        float volume = Mathf.Clamp01(1f - nearestDistance / 14);
        
        laserSound.setVolume(volume);
    }

    public float GetDamage() => 1;
    public DamageSource GetDamageSource() => DamageSource.ENVIRONMENT;

    void OnTriggerEnter(Collider other)
    {
        if(!_active) return;
        if(other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(this);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }
}
