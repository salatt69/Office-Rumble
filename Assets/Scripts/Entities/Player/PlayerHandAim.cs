using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHandAim : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform hand;
    [SerializeField] SpriteRenderer playerSprite;
    [SerializeField] ItemHolder holder;

    [Header("Hand Local Position")]
    [SerializeField] bool autoCacheHandLocalPosOnStart = true;

    Camera cam;

    Vector3 localHandPos;
    float localHandPosX;
    float localHandPosXInverted;

    bool isScoped;

    public bool IsLeftFacing { get; private set; }

    void Awake()
    {
        if (!hand) hand = transform.Find("Hand");
        if (!playerSprite) playerSprite = GetComponentInChildren<SpriteRenderer>(true);

        if (!holder && hand)
            holder = hand.GetComponentInChildren<ItemHolder>(true);
    }

    public void BindCamera(Camera cameraRef)
    {
        cam = cameraRef;
    }

    void Start()
    {
        if (hand && autoCacheHandLocalPosOnStart)
        {
            localHandPos = hand.localPosition;
            localHandPosX = localHandPos.x;
            localHandPosXInverted = -localHandPosX;
        }
    }

    public void SetScoped(bool scoped) => isScoped = scoped;

    public void Tick()
    {
        if (!cam || !playerSprite) return;

        Vector2 mouseScreen = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        bool isLeft = mouseWorld.x < transform.position.x;
        IsLeftFacing = isLeft;

        playerSprite.flipX = isLeft;

        if (holder != null)
            holder.FlipY(isLeft);

        if (hand)
        {
            float handX = isLeft ? localHandPosXInverted : localHandPosX;
            hand.localPosition = new Vector3(handX, localHandPos.y, localHandPos.z);
        }
    }

    public Vector3 GetItemDropPosition()
    {
        return transform.position + (IsLeftFacing ? Vector3.left : Vector3.right) * 0.5f;
    }
}