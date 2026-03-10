using UnityEngine;
using UnityEngine.UI;

public class ProgressRingUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Canvas worldCanvas;
    [SerializeField] Transform followTarget;
    [SerializeField] Image fillImage;
    [SerializeField] RepeatedFlashing repeatedFlashing;

    [Header("Follow")]
    [SerializeField] Vector3 worldOffset = new(0f, 1f, 0f);
    [SerializeField] bool billboardToCamera = false;

    Camera mainCam;

    bool isPlaying;
    float duration;
    float elapsed;

    public bool IsVisible => worldCanvas != null && worldCanvas.enabled;
    public bool IsPlaying => isPlaying;
    public float NormalizedProgress => duration <= 0f ? 0f : Mathf.Clamp01(elapsed / duration);

    void Awake()
    {
        mainCam = Camera.main;

        if (!worldCanvas)
            worldCanvas = GetComponentInChildren<Canvas>(true);

        if (!fillImage)
            Debug.LogWarning($"{name}: ProgressRingUI has no fillImage assigned.");

        if (!repeatedFlashing)
            repeatedFlashing = GetComponentInChildren<RepeatedFlashing>(true);

        repeatedFlashing?.SetFlashing(false);

        SetVisible(false);
        SetProgress(0f);
    }

    void Update()
    {
        if (isPlaying)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            SetProgress(t);

            if (elapsed >= duration)
            {
                isPlaying = false;
                elapsed = duration;
                SetProgress(1f);
                OnCompleted();
            }
        }
    }

    void LateUpdate()
    {
        if (!followTarget) return;

        transform.position = followTarget.position + worldOffset;

        if (billboardToCamera && mainCam)
            transform.rotation = mainCam.transform.rotation;
    }

    public void SetTarget(Transform target)
    {
        followTarget = target;
    }

    public void SetOffset(Vector3 offset)
    {
        worldOffset = offset;
    }

    public void SetProgress(float normalized)
    {
        if (!fillImage) return;

        normalized = Mathf.Clamp01(normalized);
        fillImage.fillAmount = normalized;

        if (repeatedFlashing)
            repeatedFlashing.SetFlashing(normalized >= 1f);
    }

    public void SetVisible(bool visible)
    {
        if (worldCanvas)
            worldCanvas.enabled = visible;
        else
            gameObject.SetActive(visible);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        SetVisible(true);
    }

    public void Hide()
    {
        repeatedFlashing?.SetFlashing(false);
        gameObject.SetActive(false);
        SetVisible(false);
    }

    public void Play(float newDuration, bool resetToZero = true)
    {
        duration = Mathf.Max(0.0001f, newDuration);

        if (resetToZero)
            elapsed = 0f;

        isPlaying = true;
        Show();

        if (resetToZero)
            SetProgress(0f);
    }

    public void Stop(bool hide = true, bool reset = false)
    {
        isPlaying = false;
        repeatedFlashing?.SetFlashing(false);

        if (reset)
        {
            elapsed = 0f;
            SetProgress(0f);
        }

        if (hide)
            Hide();
    }

    public void Complete(bool hide = true)
    {
        isPlaying = false;
        elapsed = duration;
        SetProgress(1f);

        if (hide)
            Hide();
    }

    protected virtual void OnCompleted()
    {
        // default behavior: hide when done
        Hide();
    }
}