using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(EnemySensor))]
[RequireComponent(typeof(EnemyShooter))]
[RequireComponent(typeof(FaceTarget))]
[RequireComponent(typeof(Collider2D))]
public class TurretBrain : MonoBehaviour
{
    EnemySensor sensor;
    EnemyShooter shooter;
    FaceTarget faceTarget;
    Collider2D col;
    public Animator animator;

    bool isTargetInRange;

    [SerializeField] float activationRadius = 8f;

    void Awake()
    {
        sensor = GetComponent<EnemySensor>();
        shooter = GetComponent<EnemyShooter>();
        faceTarget = GetComponent<FaceTarget>();
        col = GetComponent<Collider2D>();

        GetComponent<Health>().OnDied += OnDeath;
    }

    void Update()
    {
        if (sensor.target == null) return;
        isTargetInRange = sensor.distanceToTarget <= activationRadius;
        animator.SetBool("TartgetInRange", isTargetInRange);

        if (isTargetInRange)
        {
            faceTarget.SetTarget(sensor.target);
            shooter.TryShootAt(sensor.target);
        }
    }

    void OnDeath()
    {
        sensor.ForgetTarget();
        sensor.enabled = false;

        shooter.enabled = false;

        faceTarget.ResetSpriteFlip();
        faceTarget.enabled = false;

        col.enabled = false;

        animator.SetTrigger("Dead");
    }
}