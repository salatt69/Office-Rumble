using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(EnemySensor))]
[RequireComponent(typeof(EnemyShooter))]
public class TurretBrain : MonoBehaviour
{
    EnemySensor sensor;
    EnemyShooter shooter;
    FaceTarget faceTarget;
    HurtboxGroup hurtboxGroup;
    public Animator animator;

    bool isTargetInRange;

    [SerializeField] float activationRadius = 8f;

    void Awake()
    {
        sensor = GetComponent<EnemySensor>();
        shooter = GetComponent<EnemyShooter>();
        faceTarget = GetComponentInChildren<FaceTarget>();
        hurtboxGroup = GetComponentInChildren<HurtboxGroup>();

        GetComponent<Health>().OnDied += OnDeath;
    }

    void Update()
    {
        if (sensor.target == null) return;
        isTargetInRange = sensor.distanceToTarget <= activationRadius;
        animator.SetBool("TargetInShootRange", isTargetInRange);

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

        if (faceTarget != null)
        {
            faceTarget.ResetSpriteFlip();
            faceTarget.enabled = false;
        }

        hurtboxGroup?.SetColliderActive(false);

        animator.SetTrigger("Dead");
    }
}