using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenuEvents : MonoBehaviour
{
    //[SerializeField] Animator animator;

    UIDocument document;

    Button continueButton;
    Button startButton;
    Button settingsButton;
    Button exitButton;

    GameObject loadingScreen;
    GameObject loadingScreenInstance;

    void Awake()
    {
        //loadingScreen = Resources.Load<GameObject>("Prefabs/Scenes/LoadingScreen");
        //if (!loadingScreen)
        //{
        //    Debug.LogError("Loading screen prefab not found in Resources/Prefabs/Scenes/LoadingScreen");
        //}

        document = GetComponent<UIDocument>();

        continueButton = document.rootVisualElement.Q<Button>("ContinueButton");
        startButton = document.rootVisualElement.Q<Button>("StartButton");
        settingsButton = document.rootVisualElement.Q<Button>("SettingsButton");
        exitButton = document.rootVisualElement.Q<Button>("ExitButton");

        startButton.RegisterCallback<ClickEvent>(StartGame);
    }

    private void StartGame(ClickEvent evt)
    {
        //StartCoroutine(LoadLevelAsync("Level1"));
        GameSceneLoader.Instance.LoadSceneAsync("Level1");
    }
}
