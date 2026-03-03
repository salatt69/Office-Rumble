// PlayerAimAndHand.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimAndHand : MonoBehaviour
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
        cam = Camera.main;

        if (!hand) hand = transform.Find("Hand");
        if (!playerSprite) playerSprite = GetComponentInChildren<SpriteRenderer>(true);

        if (!holder && hand)
            holder = hand.GetComponentInChildren<ItemHolder>(true);
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

        // use mouse position only for facing/hand flip (even if you later aim differently)
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