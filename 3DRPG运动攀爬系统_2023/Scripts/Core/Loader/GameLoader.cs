using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class SCENE_NAME
{
    public const string GAMELOADER = "GameLoader";
    public const string TEST = "0";
    public const string HOME = "Home";
    public const string Map = "Map1";//暂时只有地图1
}
public class GameLoader : MonoSingleTon<GameLoader>
{
    public static bool isOpenLoaderPopup = true;
    public static bool isLoading = false;
    private static float loadedProgress=0;
    private static AsyncOperation asyncOperation = null;
    public GameLoaderPopup gameLoaderPopup;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        if (EditorPrefs.GetBool(Application.productName+"_IsOpenDefaultLaunch", false)==false)
        {
            return;
        }
        string startSceneName = EditorPrefs.GetString(Application.productName+"_DefaultLaunchKey", "GameLoader");
        string testSceneName=EditorPrefs.GetString(Application.productName+"_DefaultTestSceneKey", "Test");
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name.Equals(startSceneName)||scene.name.Equals(testSceneName))
        {
            return;
        }
#if UNITY_EDITOR

#endif
        GameRoot.GameState_ = GameState.NONE;
        LoadScene(startSceneName);
    }
    public IEnumerator Start()
    {
        GameRoot.Instance.StartLoadScript();
        while (true)
        {
            if (GameRoot.Instance.loadScriptProgress == 1)
            {
                //string initMapName = SCENE_NAME.HOME;
                string initMapName = SCENE_NAME.TEST;
                Scene scene = SceneManager.GetSceneByName(initMapName);
                if (scene == null)
                {
                    DebugTool.DebugLogger(MyDebugType.SupperError,string.Format("加载SCENE_NAME.HOME失败，场景不存在。"));
                    yield break;
                }
                //#if UNITY_EDITOR
                //                initMapName = EditorPrefs.GetString(Application.productName + "_DefaultMapSceneKey", SCENE_NAME.HOME);
                //#endif
                asyncOperation = LoadScene(initMapName,false);
                break;
            }
            else
            {
                loadedProgress = GameRoot.Instance.loadScriptProgress * 0.2f;
            }
            yield return 0;
        }
    }
    public void Update()
    {
        if (isLoading)
        {
            if (loadedProgress<0.9f)
            {
                loadedProgress = asyncOperation.progress;
                if (isOpenLoaderPopup)
                {
                    if (gameLoaderPopup.gameObject.activeSelf)
                    {
                        gameLoaderPopup.UpdateProgressText(loadedProgress);
                    }
                    else
                    {
                        gameLoaderPopup.Enable();
                    }
                }
            }
            else
            {
                loadedProgress += Time.fixedDeltaTime;
                if (loadedProgress>=1)
                {
                    LoadProgressComplete();
                }
                loadedProgress=Mathf.Clamp01(loadedProgress);
                if (isOpenLoaderPopup)
                {
                    gameLoaderPopup.UpdateProgressText(loadedProgress);
                }
            }
            
        }
    }
    public static AsyncOperation LoadScene(string sceneName,bool _isOpenLoaderPopup=true)
    {
        
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        Instance?.StartLoadScene(ao);
        ao.completed += (_ao) => {  Instance.LoadSceneComplete(sceneName); };
        isOpenLoaderPopup = _isOpenLoaderPopup;
        
        return ao;
    }
    private  void StartLoadScene(AsyncOperation ao)
    {
        isLoading = true;
        loadedProgress = 0;
        ao.allowSceneActivation = false;
        asyncOperation = ao;
    }
    private void LoadProgressComplete()
    {
        isLoading = false;
        loadedProgress = 1;
        asyncOperation.allowSceneActivation = true;
        asyncOperation = null;
    }
    public  void LoadSceneComplete(string sceneName)
    {
        gameLoaderPopup?.Exit();
        switch (sceneName)
        {
            case SCENE_NAME.GAMELOADER:
                GameRoot.GameState_ = GameState.LOADING;
                break;
            case SCENE_NAME.HOME:
                GameRoot.GameState_ = GameState.NONE;
                UIManager.Instance.PushPanel(new PopupDefinition(UIPanelIdentity.HomeOptionPopup));
                break;
            case SCENE_NAME.Map:
                GameRoot.GameState_ = GameState.GAMEING;
                GameRoot.Instance.GameLoadComplete();
                break;
            default:
                GameRoot.GameState_ = GameState.GAMEING;
                break;
        }
    }
}
