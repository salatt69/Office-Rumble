using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(EnemySensor))]
public class EnemyBrain : MonoBehaviour
{
    EnemySensor sensor;
    EnemyShooter shooter;
    FaceTarget faceTarget;
    HurtboxGroup hurtboxGroup;
    Animator animator;

    AIPath aiPath;
    AIDestinationSetter destinationSetter;

    bool canShoot;
    bool hasFacing;
    bool canPathfind;

    void Awake()
    {
        sensor = GetComponent<EnemySensor>();
        shooter = GetComponent<EnemyShooter>();
        faceTarget = GetComponentInChildren<FaceTarget>();
        hurtboxGroup = GetComponentInChildren<HurtboxGroup>();
        animator = GetComponentInChildren<Animator>();
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();

        canShoot = shooter != null;
        hasFacing = faceTarget != null;
        canPathfind = aiPath != null && destinationSetter != null;

        // Sync EntityBody.MoveSpeed to AIPath
        if (canPathfind)
        {
            var body = GetComponent<EntityBody>();
            if (body) aiPath.maxSpeed = body.MoveSpeed;
        }

        GetComponent<Health>().OnDied += OnDeath;
    }

    void Update()
    {
        // Feed target into A* or clear it
        if (canPathfind)
            destinationSetter.target = sensor.target;

        if (sensor.target == null)
        {
            if (canPathfind) aiPath.isStopped = true;
            animator?.SetBool("ShouldChase", false);
            animator?.SetBool("TargetInShootRange", false);
            return;
        }

        float dist = sensor.distanceToTarget;
        bool inAttackRange = canShoot && dist <= sensor.AttackRadius;

        // Stop moving when in attack range, chase otherwise
        if (canPathfind)
            aiPath.isStopped = inAttackRange;

        bool isChasing = canPathfind && !inAttackRange;

        if (hasFacing && (isChasing || inAttackRange))
            faceTarget.SetTarget(sensor.target);

        animator?.SetBool("ShouldChase", isChasing);
        animator?.SetBool("TargetInShootRange", inAttackRange);

        if (inAttackRange)
            shooter.TryShootAt(sensor.target);
    }

    void OnDeath()
    {
        sensor.ForgetTarget();
        sensor.enabled = false;

        if (canPathfind)
        {
            aiPath.isStopped = true;
            aiPath.enabled = false;
            destinationSetter.enabled = false;
        }

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