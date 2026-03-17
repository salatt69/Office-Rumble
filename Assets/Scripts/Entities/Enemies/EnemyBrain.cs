using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(EnemySensor))]
public class EnemyBrain : MonoBehaviour
{
    EnemySensor sensor;
    EnemyChaser chaser;
    EnemyShooter shooter;
    FaceTarget faceTarget;
    HurtboxGroup hurtboxGroup;
    Animator animator;

    bool canChase;
    bool canShoot;
    bool hasFacing;

    void Awake()
    {
        sensor = GetComponent<EnemySensor>();
        chaser = GetComponent<EnemyChaser>();
        shooter = GetComponent<EnemyShooter>();
        faceTarget = GetComponentInChildren<FaceTarget>();
        hurtboxGroup = GetComponentInChildren<HurtboxGroup>();
        animator = GetComponentInChildren<Animator>();

        canChase = chaser != null;
        canShoot = shooter != null;
        hasFacing = faceTarget != null;

        GetComponent<Health>().OnDied += OnDeath;
    }

    void Update()
    {
        if (sensor.target == null) return;

        float dist = sensor.distanceToTarget;

        bool shouldChase = canChase && dist <= chaser.Radius;
        bool inAttackRange = canShoot && dist <= shooter.Radius;

        if (hasFacing && (shouldChase || inAttackRange))
            faceTarget.SetTarget(sensor.target);

        animator?.SetBool("ShouldChase", shouldChase && !inAttackRange);
        animator?.SetBool("TargetInShootRange", inAttackRange);

        if (shouldChase)
            chaser.ChaseTarget(sensor.target);

        if (inAttackRange)
            shooter.TryShootAt(sensor.target);
    }

    void OnDeath()
    {
        sensor.ForgetTarget();
        sensor.enabled = false;

        if (canChase) chaser.enabled = false;
        if (canShoot) shooter.enabled = false;

        if (hasFacing)
        {
            faceTarget.ResetSpriteFlip();
            faceTarget.enabled = false;
        }

        hurtboxGroup?.SetColliderActive(false);
        animator?.SetTrigger("Dead");
    }
}