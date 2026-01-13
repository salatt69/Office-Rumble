using UnityEngine;

public class HUD : MonoBehaviour
{
    void Awake()
    {
        GameManager.Instance.RegisterHUD(this);
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
