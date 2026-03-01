using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[RequireComponent(typeof(EnemySensor))]
[RequireComponent(typeof(EnemyMover))]
public class EnemyBrain : MonoBehaviour
{
    public EnemySensor sensor { get; private set; }
    public EnemyMover mover { get; private set; }
    public EnemyShooter shooter { get; private set; }

    [Header("Tuning")]
    public float chaseDistance = 10f;
    public float preferredShootDistance = 6f;
    public float fleeDistance = 3f;
    public float activationRadius = 8f;

    IEnemyState state;

    void Awake()
    {
        sensor = GetComponent<EnemySensor>();
        mover = GetComponent<EnemyMover>();
        shooter = GetComponent<EnemyShooter>(); // optional
    }

    void Start()
    {
        SetState(new IdleState());
    }

    void Update()
    {
        state?.Tick(this);
    }

    public void SetState(IEnemyState newState)
    {
        state?.Exit(this);
        state = newState;
        state?.Enter(this);
    }
}