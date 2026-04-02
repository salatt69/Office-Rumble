using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] InputRouter input;
    [SerializeField] EntityMotor2D motor;
    [SerializeField] PlayerHandAim aim;
    [SerializeField] CameraFollow2D cam;
    [SerializeField] PlayerCombat combat;
    [SerializeField] Inventory inventory;
    [SerializeField] Animator animator;

    [Header("Pickup")]
    [SerializeField] float defaultPickupCooldown = 2f;

    [Header("Death Settings")]
    [SerializeField] GameObject[] objectsToDisableOnDeath;

    GameObject mainCameraPrefab;
    GameObject mainCameraInstance;

    float pickupCooldownExitTime;
    public bool CanPickup => Time.time >= pickupCooldownExitTime;
    public bool IsDead { get; private set; }

    void Awake()
    {
        if (!input) input = GetComponent<InputRouter>();
        if (!motor) motor = GetComponent<EntityMotor2D>();
        if (!aim) aim = GetComponent<PlayerHandAim>();
        if (!cam) cam = GetComponent<CameraFollow2D>();
        if (!combat) combat = GetComponent<PlayerCombat>();
        if (!inventory) inventory = GetComponent<Inventory>();
        if (!animator) animator = GetComponentInChildren<Animator>(true);

        mainCameraPrefab = Resources.Load<GameObject>("Prefabs/Entities/Player/Main Camera");
        if (mainCameraPrefab)
        {
            mainCameraInstance = Instantiate(mainCameraPrefab, transform.position, Quaternion.identity);

            Camera spawnedCamera = mainCameraInstance.GetComponent<Camera>();
            if (!spawnedCamera)
                spawnedCamera = mainCameraInstance.GetComponentInChildren<Camera>(true);

            if (cam && spawnedCamera)
                cam.BindCamera(spawnedCamera);

            if (aim && spawnedCamera)
                aim.BindCamera(spawnedCamera);
        }

        if (input) input.SetOwner(this);
    }

    void Start()
    {
        inventory?.SelectSlot(inventory.SelectedIndex);
    }

    void Update()
    {
        if (IsDead) return;

        if (input && input.AttackHeld)
        {
            inventory?.TryUseSelected();
        }
        else
        {
            inventory?.SetUseHeld(false);
        }

        if (animator && input)
        {
            Vector2 m = input.MoveInput;
            animator.SetFloat("Horizontal", m.x);
            animator.SetFloat("Vertical", m.y);
            animator.SetFloat("Speed", m.sqrMagnitude);
        }
    }

    void FixedUpdate()
    {
        if (IsDead) return;

        if (motor && input)
            motor.SetMoveInput(input.MoveInput);

        cam?.TickFixed();
        motor?.FixedTick();
    }

    void LateUpdate()
    {
        if (IsDead) return;

        if (aim && input)
            aim.SetScoped(input.IsScoped);

        aim?.Tick();
    }

    public void AddItemPickupCooldown(float cooldownTime = -1f)
    {
        float cd = cooldownTime >= 0f ? cooldownTime : defaultPickupCooldown;
        pickupCooldownExitTime = Mathf.Max(pickupCooldownExitTime, Time.time + cd);
    }

    public Vector3 GetItemDropPosition()
    {
        return aim ? aim.GetItemDropPosition() : transform.position;
    }

    public void ApplyKnockbackLock(float duration)
    {
        if (motor) motor.ApplyKnockbackLock(duration);
    }

    public void TryDrop()
    {
        if (!inventory) return;
        if (!CanPickup) return;

        Vector3 dropPos = GetItemDropPosition();
        inventory.Drop(dropPos);
        AddItemPickupCooldown();
    }

    public void SelectSlot(int index) => inventory?.SelectSlot(index);
    public void SelectNext() => inventory?.SelectNext();
    public void SelectPrevious() => inventory?.SelectPrevious();

    public void SetDead()
    {
        IsDead = true;

        if (input != null)
            input.BlockItemSelection = true;

        foreach (var obj in objectsToDisableOnDeath)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        if (animator != null)
            animator.SetTrigger("Died");

        UpdateSpriteFlipToMouse();
    }

    public void UpdateSpriteFlipToMouse()
    {
        if (aim != null && aim.Camera != null)
        {
            Vector2 mouseScreen = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
            Vector3 mouseWorld = aim.Camera.ScreenToWorldPoint(mouseScreen);
            mouseWorld.z = 0f;

            bool isLeft = mouseWorld.x < transform.position.x;
            aim.SetFacing(isLeft);
        }
    }
}