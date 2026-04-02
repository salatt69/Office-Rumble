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
    Coroutine[] hideCoroutines;
    int currentIndex = 0;
    readonly List<int> visibleOrder = new();

    void Awake()
    {
        if (!notificationDocument)
            notificationDocument = GetComponent<UIDocument>();

        var templateElement = notificationDocument.rootVisualElement.Q<VisualElement>("consumable-notification");
        if (templateElement != null)
            templateElement.style.display = DisplayStyle.None;

        notificationPool = new VisualElement[poolSize];
        hideCoroutines = new Coroutine[poolSize];
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

        int thisIndex = GetAvailableIndex();

        if (visibleOrder.Contains(thisIndex))
        {
            if (hideCoroutines[thisIndex] != null)
                StopCoroutine(hideCoroutines[thisIndex]);
            visibleOrder.Remove(thisIndex);
        }

        var notification = notificationPool[thisIndex];

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

        visibleOrder.Add(thisIndex);

        hideCoroutines[thisIndex] = StartCoroutine(HideNotificationAfterDelay(notification, notificationRoot, thisIndex, settings.displayDuration, settings.fadeOutDuration));
        BringToFront(notification);
        RepositionNotifications();
    }

    int GetAvailableIndex()
    {
        int startIndex = currentIndex;
        do
        {
            if (notificationPool[currentIndex].style.display == DisplayStyle.None)
            {
                int result = currentIndex;
                currentIndex = (currentIndex + 1) % poolSize;
                return result;
            }
            currentIndex = (currentIndex + 1) % poolSize;
        } while (currentIndex != startIndex);

        int result2 = currentIndex;
        currentIndex = (currentIndex + 1) % poolSize;
        return result2;
    }

    void BringToFront(VisualElement notification)
    {
        notification.RemoveFromHierarchy();
        notificationDocument.rootVisualElement.Add(notification);
    }

    IEnumerator HideNotificationAfterDelay(VisualElement notification, VisualElement notificationRoot, int index, float delay, float fadeOut)
    {
        yield return new WaitForSeconds(delay);

        notificationRoot.RemoveFromClassList("visible");

        yield return new WaitForSeconds(fadeOut);
        notification.style.display = DisplayStyle.None;
        visibleOrder.Remove(index);
        hideCoroutines[index] = null;
        RepositionNotifications();
    }

    void RepositionNotifications()
    {
        int yOffset = 0;
        foreach (int i in visibleOrder)
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
