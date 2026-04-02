using UnityEngine;
using UnityEngine.InputSystem;

public class InputRouter : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] InputActionAsset controls;

    public Vector2 MoveInput { get; private set; }
    public bool AttackHeld { get; private set; }
    public bool IsScoped { get; private set; }
    public bool BlockItemSelection { get; set; }

    PlayerController owner;

    InputActionMap playerMap;
    InputActionMap invMap;

    InputAction moveAction;
    InputAction attackAction;
    InputAction altAction;

    InputAction slot1;
    InputAction slot2;
    InputAction slot3;
    InputAction dropItem;
    InputAction selectNext;

    bool bound;

    public void SetOwner(PlayerController controller) => owner = controller;

    void Awake()
    {
        if (!controls)
        {
            Debug.LogError($"{name}: PlayerInputRouter has no InputActionAsset assigned.");
            return;
        }

        CacheActions();
        Bind();
    }

    void OnEnable() => EnableMaps(true);
    void OnDisable() => EnableMaps(false);

    void OnDestroy() => Unbind();

    void CacheActions()
    {
        playerMap = controls.FindActionMap("Player", true);
        invMap = controls.FindActionMap("Inventory", true);

        moveAction = playerMap.FindAction("Move", true);
        attackAction = playerMap.FindAction("Attack", true);
        altAction = playerMap.FindAction("Alternative", true);

        slot1 = invMap.FindAction("Slot1", true);
        slot2 = invMap.FindAction("Slot2", true);
        slot3 = invMap.FindAction("Slot3", true);
        dropItem = invMap.FindAction("Drop Item", true);
        selectNext = invMap.FindAction("Select Next", true);
    }

    void EnableMaps(bool enable)
    {
        if (playerMap == null || invMap == null) return;
        if (enable) { playerMap.Enable(); invMap.Enable(); }
        else { playerMap.Disable(); invMap.Disable(); }
    }

    void Bind()
    {
        if (bound) return;
        bound = true;

        moveAction.performed += OnMovePerformed;
        moveAction.canceled += OnMoveCanceled;

        attackAction.performed += OnAttackPerformed;
        attackAction.canceled += OnAttackCanceled;

        altAction.performed += OnAltPerformed;
        altAction.canceled += OnAltCanceled;

        slot1.performed += _ => owner?.SelectSlot(0);
        slot2.performed += _ => owner?.SelectSlot(1);
        slot3.performed += _ => owner?.SelectSlot(2);

        dropItem.performed += _ => owner?.TryDrop();

        selectNext.performed += OnSelectNext;
    }

    void Unbind()
    {
        if (!bound) return;
        bound = false;

        if (moveAction != null)
        {
            moveAction.performed -= OnMovePerformed;
            moveAction.canceled -= OnMoveCanceled;
        }

        if (attackAction != null)
        {
            attackAction.performed -= OnAttackPerformed;
            attackAction.canceled -= OnAttackCanceled;
        }

        if (altAction != null)
        {
            altAction.performed -= OnAltPerformed;
            altAction.canceled -= OnAltCanceled;
        }

        if (selectNext != null)
            selectNext.performed -= OnSelectNext;

        // slot/drop use lambdas; OK for player lifetime.
    }

    void OnMovePerformed(InputAction.CallbackContext ctx) => MoveInput = ctx.ReadValue<Vector2>();
    void OnMoveCanceled(InputAction.CallbackContext ctx) => MoveInput = Vector2.zero;

    void OnAttackPerformed(InputAction.CallbackContext ctx) => AttackHeld = true;
    void OnAttackCanceled(InputAction.CallbackContext ctx) => AttackHeld = false;

    void OnAltPerformed(InputAction.CallbackContext ctx) => IsScoped = true;
    void OnAltCanceled(InputAction.CallbackContext ctx) => IsScoped = false;

    void OnSelectNext(InputAction.CallbackContext ctx)
    {
        if (!owner || BlockItemSelection) return;

        Vector2 s = ctx.ReadValue<Vector2>();
        float v = s.y;
        if (v < 0) owner.SelectNext();
        if (v > 0) owner.SelectPrevious();
    }
}