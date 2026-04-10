using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(InputRouter))]
public class CameraFollow2D : MonoBehaviour
{
    public static CameraFollow2D Instance { get; private set; }

    [SerializeField] Transform target;
    [SerializeField, Range(0f, 1f)] float lagTime = 0.15f;
    [SerializeField] float cursorOffsetStrength = 1f;
    [SerializeField] float maxOffsetDistance = 2f;

    [SerializeField] int scopedPPU = 24;
    [SerializeField] int unscopedPPU = 32;

    Camera cam;
    Vector3 velocity;
    InputRouter input;
    Rigidbody2D targetRb;
    PixelPerfectCamera pixelPerfectCamera;

    Vector2? forcedTargetPosition;
    bool isReturningToPlayer;
    [SerializeField, Range(0f, 1f)] float playerFollowWeight = 0.3f;
    [SerializeField, Range(0.5f, 5f)] float snapSpeed = 2f;
    [SerializeField, Range(0f, 1f)] float playerOffsetStrength = 0.25f;
    float blendAmount;

    void Awake()
    {
        if (!target) target = transform;

        input = GetComponent<InputRouter>();
        targetRb = target.GetComponent<Rigidbody2D>();

        if (Instance == null) Instance = this;
    }

    public void BindCamera(Camera cameraRef)
    {
        cam = cameraRef;

        if (cam)
            pixelPerfectCamera = cam.GetComponent<PixelPerfectCamera>();
    }

    public void SnapToRoom(Vector2 roomCenter)
    {
        forcedTargetPosition = roomCenter;
        isReturningToPlayer = false;
        blendAmount = 0f;
    }

    public void ReturnToPlayer()
    {
        isReturningToPlayer = true;
    }

    public bool IsReturningToPlayer => isReturningToPlayer;

    public void TickFixed()
    {
        if (!cam || !target) return;

        bool scoped = input ? input.IsScoped : false;

        if (forcedTargetPosition.HasValue)
        {
            blendAmount = Mathf.MoveTowards(blendAmount, isReturningToPlayer ? 1f : 0f, snapSpeed * Time.fixedDeltaTime);
        }
        else
        {
            blendAmount = Mathf.MoveTowards(blendAmount, 1f, snapSpeed * Time.fixedDeltaTime);
        }

        Vector3 targetPos;
        Vector3 playerPos = targetRb ? (Vector3)targetRb.position : target.position;
        playerPos.z = 0f;

        if (forcedTargetPosition.HasValue)
        {
            Vector3 roomPos = (Vector3)forcedTargetPosition.Value;
            roomPos.z = 0f;

            targetPos = Vector3.Lerp(roomPos, playerPos, blendAmount);

            if (isReturningToPlayer && blendAmount >= 0.99f)
            {
                forcedTargetPosition = null;
                isReturningToPlayer = false;
            }
        }
        else
        {
            targetPos = playerPos;
        }

        Vector3 offset = Vector3.zero;

        if (forcedTargetPosition.HasValue)
        {
            Vector3 roomPos = (Vector3)forcedTargetPosition.Value;
            roomPos.z = 0f;
            Vector3 toPlayer = playerPos - roomPos;
            offset = Vector3.ClampMagnitude(toPlayer, maxOffsetDistance) * playerOffsetStrength;
        }
        else if (scoped && Mouse.current != null)
        {
            Vector2 mouseScreen = Mouse.current.position.ReadValue();
            Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);
            mouseWorld.z = 0f;

            offset = mouseWorld - playerPos;
            if (offset.magnitude > 0.001f)
                offset = offset.normalized * Mathf.Min(offset.magnitude, maxOffsetDistance) * cursorOffsetStrength;

            if (pixelPerfectCamera) pixelPerfectCamera.assetsPPU = scopedPPU;
        }
        else
        {
            if (pixelPerfectCamera) pixelPerfectCamera.assetsPPU = unscopedPPU;
        }

        Vector3 desired = targetPos + offset + new Vector3(0f, 0f, -1f);

        cam.transform.position = Vector3.SmoothDamp(
            cam.transform.position,
            desired,
            ref velocity,
            lagTime,
            Mathf.Infinity,
            Time.fixedDeltaTime
        );
    }
}