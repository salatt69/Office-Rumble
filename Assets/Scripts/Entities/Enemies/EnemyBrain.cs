using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(EnemySensor))]
public class EnemyBrain : MonoBehaviour
{
    [Header("Loot")]
    [SerializeField] Vector2 scrapOnDeathRange;

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

        if (canPathfind)
        {
            var body = GetComponent<EntityBody>();
            if (body) aiPath.maxSpeed = body.MoveSpeed;
        }

        GetComponent<Health>().OnDied += OnDeath;
    }

    void Update()
    {
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
        bool canSeeTarget = sensor.hasLineOfSight;

        bool isChasing = canPathfind && !canSeeTarget;
        if (canPathfind)
            aiPath.isStopped = !isChasing;

        if (hasFacing && (!isChasing || canSeeTarget))
            faceTarget.SetTarget(sensor.target);

        animator?.SetBool("ShouldChase", isChasing);
        animator?.SetBool("TargetInShootRange", canSeeTarget);

        if (canSeeTarget)
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

        GiveScrap();
    }

    void GiveScrap()
    {
        var player = FindFirstObjectByType<PlayerController>();
        if (player == null) return;

        var wallet = player.GetComponent<PlayerWallet>();
        if (wallet != null)
        {
            int min = (int)scrapOnDeathRange.x;
            int max = (int)scrapOnDeathRange.y;
            wallet.AddMoney(Random.Range(min, max));
        }
    }
}