using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(CapsuleCollider))]
public class LaserBehaviour : MonoBehaviour, IDamageSource
{
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private LayerMask hitMask;

    private LineRenderer lineRenderer;
    private CapsuleCollider capsuleCollider;

    private Vector3 lastDirection;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        if(transform.parent.forward != lastDirection)
        {
            lastDirection = transform.parent.forward;
            FireLaser();
        }
    }

    private void FireLaser()
    {
        Vector3 start = transform.position;
        Vector3 direction = transform.parent.forward;
        Vector3 end;

        if (Physics.Raycast(start, direction, out RaycastHit hit, maxDistance, hitMask)) end = hit.point;
        else end = start + direction * maxDistance;

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

    public float GetDamage()
    {
        return 1;
    }
    public DamageSource GetDamageSource()
    {
        return DamageSource.ENVIRONMENT;
    }

    void OnTriggerEnter(Collider other)
    {
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
