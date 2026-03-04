using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(InputRouter))]
public class CameraFollow2D : MonoBehaviour
{
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

    void Awake()
    {
        if (!target) target = transform;

        cam = Camera.main;
        if (!pixelPerfectCamera && cam)
            pixelPerfectCamera = cam.GetComponent<PixelPerfectCamera>();

        input = GetComponent<InputRouter>();
        targetRb = target.GetComponent<Rigidbody2D>();
    }

    public void TickFixed()
    {
        if (!cam || !target) return;

        bool scoped = input ? input.IsScoped : false;

        Vector3 targetPos = targetRb ? (Vector3)targetRb.position : target.position;
        targetPos.z = 0f;

        Vector3 offset = Vector3.zero;

        if (scoped && Mouse.current != null)
        {
            Vector2 mouseScreen = Mouse.current.position.ReadValue();
            Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);
            mouseWorld.z = 0f;

            offset = mouseWorld - targetPos;
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