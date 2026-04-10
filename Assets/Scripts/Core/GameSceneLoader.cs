using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSceneLoader : MonoBehaviour
{
    public static GameSceneLoader Instance { get; private set; }

    GameObject sceneTransitionsPrefab;
    GameObject sceneTransitionsInstance;
    Animator transitionsAnimator;

    GameObject loadingScreen;
    GameObject loadingScreenInstance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void AutoInit()
    {
        if (Instance == null)
        {
            var go = new GameObject(nameof(GameSceneLoader));
            go.AddComponent<GameSceneLoader>();
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sceneTransitionsPrefab = Resources.Load<GameObject>("Prefabs/Scenes/SceneTransitions");
        if (sceneTransitionsPrefab == null)
        {
            Debug.LogError("Scene transitions prefab not found in Resources/Prefabs/Scenes/SceneTransitions");
        }
        else if (sceneTransitionsInstance == null)
        {
            sceneTransitionsInstance = Instantiate(sceneTransitionsPrefab);
            DontDestroyOnLoad(sceneTransitionsInstance);

            transitionsAnimator = sceneTransitionsInstance.GetComponentInChildren<Animator>();
            if (transitionsAnimator == null)
                Debug.LogError("Transition Animator not found on SceneTransitions prefab.");
        }

        loadingScreen = Resources.Load<GameObject>("Prefabs/Scenes/LoadingScreen");
        if (loadingScreen == null)
        {
            Debug.LogError("Loading screen prefab not found in Resources/Prefabs/Scenes/LoadingScreen");
        }
    }

    public void LoadSceneAsync(string levelName)
    {
        StartCoroutine(AsyncLoad(levelName));
    }

    IEnumerator AsyncLoad(string sceneName)
    {
        if (transitionsAnimator != null)
        {
            transitionsAnimator.SetTrigger("Start");
            yield return null;
            AnimatorStateInfo stateInfo = transitionsAnimator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(stateInfo.length);
        }

        if (loadingScreen != null)
        {
            loadingScreenInstance = Instantiate(loadingScreen);
            loadingScreenInstance.SetActive(true);
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
            yield return null;

        yield return new WaitForSeconds(0.5f);

        transitionsAnimator?.SetTrigger("End");
    }
}