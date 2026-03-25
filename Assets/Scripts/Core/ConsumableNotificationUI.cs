using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class ConsumableNotificationUI : MonoBehaviour
{
    [SerializeField] VisualTreeAsset notificationAsset;
    [SerializeField] int poolSize = 4;
    [SerializeField] NotificationSettings settings;

    UIDocument notificationDocument;
    VisualElement[] notificationPool;
    int currentIndex = 0;

    void Awake()
    {
        if (!notificationDocument)
            notificationDocument = GetComponent<UIDocument>();

        var templateElement = notificationDocument.rootVisualElement.Q<VisualElement>("consumable-notification");
        if (templateElement != null)
            templateElement.style.display = DisplayStyle.None;

        notificationPool = new VisualElement[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            var clone = notificationAsset.CloneTree();
            clone.style.display = DisplayStyle.None;
            notificationDocument.rootVisualElement.Add(clone);
            notificationPool[i] = clone;
        }
    }

    public void Show(ConsumableData data, string effectText)
    {
        if (data == null) return;

        var notification = notificationPool[currentIndex];
        var thisIndex = currentIndex;
        currentIndex = (currentIndex + 1) % poolSize;

        var notificationRoot = notification.Q<VisualElement>("consumable-notification");
        if (notificationRoot == null) notificationRoot = notification;

        var icon = notificationRoot.Q<VisualElement>("icon");
        var effectLabel = notificationRoot.Q<Label>("effect-text");

        if (icon != null && data.icon != null)
            icon.style.backgroundImage = new StyleBackground(data.icon);

        if (effectLabel != null)
            effectLabel.text = effectText;

        notification.style.display = DisplayStyle.Flex;
        notificationRoot.AddToClassList("visible");

        StartCoroutine(HideNotificationAfterDelay(notification, notificationRoot, thisIndex, settings.displayDuration, settings.fadeOutDuration));
        RepositionNotifications();
    }

    IEnumerator HideNotificationAfterDelay(VisualElement notification, VisualElement notificationRoot, int index, float delay, float fadeOut)
    {
        yield return new WaitForSeconds(delay);

        notificationRoot.RemoveFromClassList("visible");

        yield return new WaitForSeconds(fadeOut);
        notification.style.display = DisplayStyle.None;
        RepositionNotifications();
    }

    void RepositionNotifications()
    {
        int yOffset = 0;
        for (int i = 0; i < poolSize; i++)
        {
            if (notificationPool[i].style.display == DisplayStyle.Flex)
            {
                notificationPool[i].style.top = 16 + yOffset;
                notificationPool[i].style.alignSelf = Align.Center;
                yOffset += 55;
            }
        }
    }

    public static void Notify(ConsumableData data, string effectText)
    {
        var notifier = FindAnyObjectByType<ConsumableNotificationUI>();
        notifier?.Show(data, effectText);
    }
}
