using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] float MoveSpeed;

    [Header("References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer playerSprite;
    [SerializeField] InputActionAsset controls;

    [Header("Camera Settings")]
    [SerializeField, Range(0f, 1f)] float lagTime;
    [SerializeField] float cursorOffsetStrength;
    [SerializeField] float maxOffsetDistance;

    Vector3 velocity = Vector3.zero;

    GameObject hand;
    Vector3 localHandPos;
    float localHandPosX, localHandPosXInverted;

    bool isScoped = false;
    PixelPerfectCamera pixelPerfectCamera;

    private float knockbackTimer = 0f;
    private bool isKnockedBack => knockbackTimer > 0f;

    Vector2 moveInput;

    PlayerInteraction playerInteraction;
    Inventory inventory;

    void Awake()
    {
        // parent has to have 'Hand' child for this to work
        hand = GameObject.Find("Hand");

        playerInteraction = GetComponent<PlayerInteraction>();
        inventory = GetComponent<Inventory>();

        pixelPerfectCamera = Camera.main.GetComponent<PixelPerfectCamera>();

        BindInputActions();
    }

    void OnEnable()
    {
        if (controls != null)
           EnableInputActions(true);
    }

    void OnDisable()
    {
        if (controls != null)
            EnableInputActions(false);
    }

    void Start()
    {
        localHandPos = hand.transform.localPosition;
        localHandPosX = localHandPos.x;
        localHandPosXInverted = -localHandPosX;

        inventory.SelectSlot(inventory.SelectedIndex);
    }

    // Update is called once per frame
    void Update()
    {
        // Set conditional values for animations
        animator.SetFloat("Horizontal", moveInput.x);
        animator.SetFloat("Vertical", moveInput.y);
        animator.SetFloat("Speed", moveInput.sqrMagnitude);
    }

    void FixedUpdate()
    {
        UpdateCameraAndHand();

        if (isKnockedBack)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector2 move = moveInput * MoveSpeed * Time.fixedDeltaTime;

        rb.AddForce(move * MoveSpeed * Time.fixedDeltaTime);
    }


    void UpdateCameraAndHand()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        Vector3 offset = new Vector3();

        if (isScoped)
        {
            mouseWorld.z = 0f;

            offset = mouseWorld - transform.position;
            if (offset.magnitude > 0.001f)
                offset = offset.normalized * Mathf.Min(offset.magnitude, maxOffsetDistance) * cursorOffsetStrength;

            pixelPerfectCamera.assetsPPU = 24;
        }
        else
        {
            pixelPerfectCamera.assetsPPU = 32;
        }

        Vector3 desiredPosition = transform.position + offset + new Vector3(0f, 0f, -1f);
        Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, desiredPosition, ref velocity, lagTime);

        bool isLeft = mouseWorld.x < transform.position.x;
        playerSprite.flipX = isLeft;

        var holder = hand.GetComponentInChildren<ItemHolder>();
        holder.FlipY(isLeft);

        float handX = isLeft ? localHandPosXInverted : localHandPosX;
        hand.transform.localPosition = new Vector3(handX, localHandPos.y, localHandPos.z);
    }

    public void SetHitFrameSprite()
    {
    }

    public void ApplyKnockbackLock(float duration)
    {
        knockbackTimer = duration;
    }

    void TryInteract()
    {
        if (playerInteraction.GetCurrentInteractable() != null)
        {
            playerInteraction.GetCurrentInteractable().Interact(this);
        }
        else
        {
            Debug.LogWarning("Nothing to interact with.");
        }
    }

    void TryFire()
    {
        if (hand == null)
        {
            Debug.LogWarning("Hand not found!");
            return;
        }

        var holder = hand.GetComponentInChildren<ItemHolder>();
        if (holder != null)
        {
            holder.UseCurrentItem();
        }
        else
        {
            Debug.LogWarning("Can't fire! No weapon equipped.");
        }
    }

    #region Inputs

    void BindInputActions()
    { 
        var move = controls.FindActionMap("Player", true);
        move.FindAction("Move").performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        move.FindAction("Move").canceled += ctx => moveInput = Vector2.zero;
        move.FindAction("Interact").performed += _ => TryInteract();
        move.FindAction("Attack").performed += _ => TryFire();
        move.FindAction("Alternative").performed += _ => isScoped = true;
        move.FindAction("Alternative").canceled += _ => isScoped = false;

        var inv = controls.FindActionMap("Inventory", true);
        inv.FindAction("Slot1").performed += _ => inventory.SelectSlot(0);
        inv.FindAction("Slot2").performed += _ => inventory.SelectSlot(1);
        inv.FindAction("Slot3").performed += _ => inventory.SelectSlot(2);
        inv.FindAction("Drop Item").performed += _ => inventory.Drop(transform.position);
        inv.FindAction("Select Next").performed += ctx =>
        {
            Vector2 s = ctx.ReadValue<Vector2>();
            float v = s.y;
            if (v < 0)
                inventory.SelectNext();
            if (v > 0)
                inventory.SelectPrevious();
        };
    }

    void EnableInputActions(bool enable)
    {
        var move = controls.FindActionMap("Player", false);
        if (move == null) return;
        if (enable) move.Enable(); else move.Disable();

        var inv = controls.FindActionMap("Inventory", false);
        if (inv == null) return;
        if (enable) inv.Enable(); else inv.Disable();
    }

    #endregion
}
