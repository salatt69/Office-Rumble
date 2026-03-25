using UnityEngine;

[CreateAssetMenu(fileName = "NotificationSettings", menuName = "Office Rumble/Notification Settings")]
public class NotificationSettings : ScriptableObject
{
    public float displayDuration = 2f;
    public float fadeOutDuration = 0.3f;
}
