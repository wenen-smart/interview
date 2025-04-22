using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    /// <summary>
    /// 场景配置
    /// </summary>
    [System.Serializable]
    public class SceneConfig
    {
        public string SceneName;//场景名称
        public int CustomDuration;//自定义持续时间
    }
    /// <summary>
    /// 场景切换间隔
    /// </summary>
    public int SceneSwitchInterval = 5;
    /// <summary>
    /// 下次切换时间
    /// </summary>
    public float TimeUntilNextSwitch = 0.0f;
    /// <summary>
    /// 当前场景索引
    /// </summary>
    public int CurrentSceneIndex = 0;
    /// <summary>
    /// 是否已摧毁实体
    /// </summary>
    public bool EntitiesDestroyed = false;
    /// <summary>
    /// 场景配置
    /// </summary>
    public SceneConfig[] SceneConfigs;

    // Use this for initialization
    void Start()
    {
      //  DontDestroyOnLoad(this);
      //  LoadNextScene();
    }
    /// <summary>
    /// 摧毁场景中的所有实体
    /// </summary>
    public  void DestroyAllEntitiesInScene()
    {
        //1.获取实体管理器
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //2.获取所有实体
        var entities = entityManager.GetAllEntities();
        //3.摧毁之
        entityManager.DestroyEntity(entities);
        //4.释放实体管理器
        entities.Dispose();
        //5.已摧毁
        EntitiesDestroyed = true;
    }
    /// <summary>
    /// 加载下一个场景
    /// </summary>
    public  void LoadNextScene()
    {
        //获取在构建设置里的场景数量
        var sceneCount = SceneManager.sceneCountInBuildSettings;
        var nextIndex = CurrentSceneIndex + 1;
        if (nextIndex >= sceneCount)
        {

           nextIndex = 0;
          
            return;
        }

        var nextScene = SceneUtility.GetScenePathByBuildIndex(nextIndex);
        TimeUntilNextSwitch = GetSceneDuration(nextScene);
        CurrentSceneIndex = nextIndex;

        SceneManager.LoadScene(nextIndex);
        EntitiesDestroyed = false;
    }

    private int GetSceneDuration(string scenePath)
    {
        foreach (var scene in SceneConfigs)
        {
            if (!scenePath.EndsWith(scene.SceneName + ".unity"))
                continue;
            if (scene.CustomDuration <= 0)
                continue;
            return scene.CustomDuration;
        }

        return SceneSwitchInterval;
    }

    public  void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }

    // Update is called once per frame
    //void Update()
    //{
    //    TimeUntilNextSwitch -= Time.deltaTime;
    //    if (TimeUntilNextSwitch > 0.0f)
    //        return;

    //    if (!EntitiesDestroyed)
    //    {
    //        DestroyAllEntitiesInScene();
    //    }
    //    else
    //    {
    //        DestroyAllEntitiesInScene();
    //        LoadNextScene();
    //    }
    //}
}