using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    public Transform target { get; private set; }
    public float distanceToTarget { get; private set; }
    public bool hasLineOfSight { get; private set; }

    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask obstructionMask;
    [SerializeField] float acquireRadius = 12f;
    [SerializeField] float loseRadius = 15f;
    [SerializeField] float attackRadius = 5f;
    [SerializeField] float sightThickness = 0.5f;
    [SerializeField] bool drawGizmos = false;

    RoomRuntime currentRoom;
    public RoomRuntime CurrentRoom => currentRoom;

    public float AcquireRadius => acquireRadius;
    public float LoseRadius => loseRadius;
    public float AttackRadius => attackRadius;

    public bool TargetInAttackRange => target != null && distanceToTarget <= attackRadius;

    public void SetRoom(RoomRuntime room) => currentRoom = room;

    void Update()
    {
        if (currentRoom != null && !currentRoom.IsPlayerInside)
        {
            ForgetTarget();
            return;
        }

        if (target == null)
        {
            TryAcquireTarget();
        }

        if (target != null)
        {
            distanceToTarget = Vector2.Distance(transform.position, target.position);
            hasLineOfSight = CheckLineOfSight();

            if (distanceToTarget > loseRadius)
                ForgetTarget();
        }
    }

    void TryAcquireTarget()
    {
        if (currentRoom != null && !currentRoom.IsPlayerInside)
            return;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, acquireRadius, targetMask);
        if (hit != null)
        {
            target = hit.transform;
        }
    }

    bool CheckLineOfSight()
    {
        if (target == null) return false;

        Vector2 start = transform.position;
        Vector2 end = target.position;
        Vector2 direction = (end - start).normalized;
        float distance = Vector2.Distance(start, end);

        RaycastHit2D hit = Physics2D.BoxCast(start, Vector2.one * sightThickness, 0, direction, distance + sightThickness, obstructionMask);
        
        if (hit.collider == null)
            return true;

        return hit.transform == target;
    }

    public void ForgetTarget()
    {
        target = null;
        hasLineOfSight = false;
    }

    void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            DrawGizmos.Circle(transform.position, acquireRadius, Color.cyan);
            DrawGizmos.Circle(transform.position, loseRadius, Color.blue);
            DrawGizmos.Circle(transform.position, attackRadius, Color.green);
        }
    }
}
