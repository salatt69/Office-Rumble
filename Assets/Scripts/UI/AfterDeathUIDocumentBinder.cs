using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class AfterDeathUIDocumentBinder : MonoBehaviour
{
    UIDocument document;

    Button restartButton;
    Button mainMenuButton;

    void Awake()
    {
        document = GetComponent<UIDocument>();
    }

    void OnEnable()
    {
        restartButton = document.rootVisualElement.Q<Button>("RestartButton");
        mainMenuButton = document.rootVisualElement.Q<Button>("MainMenuButton");

        restartButton.RegisterCallback<ClickEvent>(RestartGame);
        mainMenuButton.RegisterCallback<ClickEvent>(GoToMainMenu);
    }

    private void RestartGame(ClickEvent evt)
    {
        GameSceneLoader.Instance.LoadSceneAsync("Level1");
    }

    private void GoToMainMenu(ClickEvent evt)
    {
        GameSceneLoader.Instance.LoadSceneAsync("MainMenu");
    }
}