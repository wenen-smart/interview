using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace UnityEditor
{

    public class ItemConfigAssetsPostprocessor : ImportAssetsPostprocessor
    {
        //任何资源（包括文件夹）导入都会被调用的方法
        void OnPreprocessAsset()
        {
            if (!this.assetPath.EndsWith(".json"))
                return;
            UpdateItemConfigAsset(this.assetPath);
        }

        [MenuItem("Tools/Config/更新ItemAsset配置")]
        public static void UpdateItemConfigAsset()
        {
            string sourcePath = Path.Combine(Application.dataPath, "Miracle_Data/Source/ItemInfo.json");
            UpdateItemConfigAsset(sourcePath);
        }
        public static void UpdateItemConfigAsset(string sourcePath)
        {
            string msg = File.ReadAllText(sourcePath, Encoding.UTF8);
            JsonData jsonObj = JsonMapper.ToObject(msg);
            if (jsonObj==null)
            {
                Debug.LogWarning($"查询不到{sourcePath}JSON对象 内容：\n{msg}");
                return;
            }
            JsonData item = jsonObj["JsonInfo"]["Version"];

            
            if (item != null && item.IsString)
            {
                ItemConfigContainer itemConfigContainer = JsonMapper.ToObject<ItemConfigContainer>(msg);
                if (itemConfigContainer)
                {
                    List<ItemConfigsJsonInfo> itemconfigCreateQueue = new List<ItemConfigsJsonInfo>();
                    bool isUpdateJson = false;
                    bool isUpdateGUID = false;
                    bool isRestartup = false;
                    bool update = EditorUtility.DisplayDialog("更新物品配置", "物品配置JSON文件有改动，请确认是否更新？", "Sure", "Wait");
                    if (update == false)
                    {
                        return;
                    }
                    EditorUtility.DisplayProgressBar("物品Asset配置更新中", "正在更新武器配置文件", 0);
                    int progressIndex = 0;
                    HandlerItemConfigReturnResult handlerWeaponConfigReturnResult = CheckItem<WeaponDataConfig>(itemConfigContainer.WeaponDataConfigList,PathDefine.weaponDataConfigFolder,"武器");
                    if (isRestartup==false)
                    {
                        isRestartup = handlerWeaponConfigReturnResult.isRestartup;
                    }
                    if (isUpdateJson==false)
                    {
                        isUpdateJson = handlerWeaponConfigReturnResult.isUpdateJson;
                    }

                    HandlerItemConfigReturnResult handlerAlimentConfigReturnResult = CheckItem<AlimentDataConfig>(itemConfigContainer.AlimentDataConfigList, PathDefine.alimentDataConfigFolder, "食物");
                    if (isRestartup == false)
                    {
                        isRestartup = handlerAlimentConfigReturnResult.isRestartup;
                    }
                    if (isUpdateJson == false)
                    {
                        isUpdateJson = handlerAlimentConfigReturnResult.isUpdateJson;
                    }

                    HandlerItemConfigReturnResult handlerOrnamentConfigReturnResult = CheckItem<OrnamentDataConfig>(itemConfigContainer.OrnamentDataConfigList, PathDefine.OrnamentDataConfigFolder, "装饰物");
                    if (isRestartup == false)
                    {
                        isRestartup = handlerOrnamentConfigReturnResult.isRestartup;
                    }
                    if (isUpdateJson == false)
                    {
                        isUpdateJson = handlerOrnamentConfigReturnResult.isUpdateJson;
                    }

                    AssetDatabase.CreateAsset(itemConfigContainer, Path.Combine("Assets/" + "Resources", PathDefine.itemConfigDataContainerPath));
                    EditorUtility.DisplayProgressBar("Miracle - Processing Game Data", "Saving...", 1f);
                    EditorUtility.DisplayDialog("更新完成", "更新成功", "I Sure");


                    UnityEngine.Object itemContainerAssetObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(Path.Combine("Assets/" + "Resources", PathDefine.itemConfigDataContainerPath));

                    //在Project面板自动选中，并在Inspector面板显示详情
                    UnityEditor.Selection.activeObject = itemContainerAssetObject;
                    UnityEditor.EditorGUIUtility.PingObject(itemContainerAssetObject);
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                    EditorUtility.ClearProgressBar();
                    CollapseFolderEditor.CollapseFolder(Path.Combine("Assets/" + "Resources", PathDefine.weaponDataConfigFolder));
                    CollapseFolderEditor.CollapseFolder(Path.Combine("Assets/" + "Resources", PathDefine.alimentDataConfigFolder));
                    if (isUpdateJson==false)
                    {
                        isUpdateJson=EditorUtility.DisplayDialog("更新Json文件","没有检测到当前Meta变化,无需自动更新。但如若你的数据在结构上发生变化可以选择本次更新Json,映射正确的结构。请问你是否要更新Json文件?","更新！","下次一定");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("更新Json文件","检测到当前Meta发生变化,自动更新Json文件。","Ok");
                    }
                    if (isUpdateJson)
                    {
                        EditorUtility.DisplayDialog("Json文件需要更新", "Asset配置文件Meta信息发生变化", "I Sure");
                        StreamWriter streamWriter = new StreamWriter(Path.Combine(string.Format(@"{0}..\..\", Application.dataPath), sourcePath));
                        JsonWriter jsonWriter = new JsonWriter(streamWriter);
                        JsonMapper.ToJson(itemConfigContainer, jsonWriter);
                        streamWriter.Dispose();
                    }
                    if (isRestartup)
                    {
                        EditorApplication.OpenProject(Application.dataPath.Replace("Assets", ""));
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("物品配置JSON文件有误", "物品配置JSON文件有误,请检查", "OK");
                }
            }
        }

        public static bool JudgeFieldIsNoCoverWhenJsonReplaceObj(FieldInfo fieldInfo)
        {
            IEnumerable<Attribute> attitudes = fieldInfo.GetCustomAttributes(typeof(JsonParseNoCoverAttitude));
            return attitudes != null && attitudes.GetEnumerator().MoveNext() == true;
        }

        public static HandlerItemConfigReturnResult CheckItem<T>(ItemDataConfigInfo<T>[] itemConfigList,string saveTo,string targetName) where T:ItemDataConfig
        {
            int progressIndex = 0;
            HandlerItemConfigReturnResult handlerItemConfigReturnResult = new HandlerItemConfigReturnResult();
            if (itemConfigList==null)
            {
                return handlerItemConfigReturnResult;
            }
            foreach (var configInfo in itemConfigList)
            {
                if (string.IsNullOrEmpty(configInfo.config.name))
                {
                    EditorUtility.ClearProgressBar();
                    throw new Exception("配置文件名没配置");
                }
                bool isUpdateGUID = false;
                progressIndex += 1;
                string nowAssetPath = "";
                if (Directory.Exists(Path.Combine(Path.Combine(Application.dataPath, "Resources\\"), saveTo)) == false)
                {
                    Directory.CreateDirectory(Path.Combine(Path.Combine(Application.dataPath, "Resources\\"), saveTo));
                }
                string assetPath = Path.Combine("Assets/" + "Resources", saveTo + "/" + configInfo.config.name + ".asset");
                ItemDataConfig oldItemData = null;
                if (string.IsNullOrEmpty(configInfo.guid) == false)
                {
                    //之前文件的GUID 有记录 
                    //如果json中更新了文件名，这会导致文件新建，新建后需要将新guid更新回原guid。
                    nowAssetPath = AssetDatabase.GUIDToAssetPath(configInfo.guid); //当文件丢失了 guid也不为空，所以说这里guid的算法不是依赖meta的
                    if (string.IsNullOrEmpty(nowAssetPath) == false)
                    {
                        oldItemData = AssetDatabase.LoadAssetAtPath(nowAssetPath, configInfo.config.GetType()) as ItemDataConfig;
                        if (oldItemData == null)
                        {
                            isUpdateGUID = true;
                            handlerItemConfigReturnResult.isRestartup = true;
                        }
                    }
                }

                if (string.IsNullOrEmpty(configInfo.guid) || oldItemData == null)
                {

                }
                else
                {
                    if (nowAssetPath != assetPath)
                    {
                        isUpdateGUID = true;
                    }
                    Type configType = configInfo.config.GetType();
                    ItemDataConfig oldItemDataConfig = AssetDatabase.LoadAssetAtPath(nowAssetPath, configType) as ItemDataConfig;
                    Type currentType = configType;
                    while (currentType != null)
                    {
                        FieldInfo[] fieldInfos = currentType.GetFields((BindingFlags)(-1));
                        foreach (var fieldInfo in fieldInfos)
                        {
                            if (JudgeFieldIsNoCoverWhenJsonReplaceObj(fieldInfo))
                            {
                                fieldInfo.SetValue(configInfo.config, fieldInfo.GetValue(oldItemDataConfig));
                            }
                        }
                        if (currentType == typeof(ItemDataConfig))
                        {
                            break;
                        }
                        currentType = currentType.BaseType;
                    }

                    AssetDatabase.DeleteAsset(nowAssetPath);
                }
                AssetDatabase.CreateAsset(configInfo.config, assetPath);
                EditorUtility.SetDirty(configInfo.config);
                if (isUpdateGUID)
                {
                    string metaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(assetPath);
                    GUID gUID = AssetDatabase.GUIDFromAssetPath(assetPath);
                    DirectoryInfo directoryInfo = new DirectoryInfo(string.Format(@"{0}..\..\", Application.dataPath));
                    string content = File.ReadAllText(Path.Combine(directoryInfo.FullName, metaPath));
                    content = content.Replace(gUID.ToString(), configInfo.guid);
                    File.WriteAllText(Path.Combine(directoryInfo.FullName, metaPath), content);
                }
                else
                {
                    if (string.IsNullOrEmpty(configInfo.guid) || oldItemData == null)
                    {
                        GUID gUID = AssetDatabase.GUIDFromAssetPath(assetPath);
                        if (gUID != null && string.IsNullOrEmpty(gUID.ToString()) == false)
                        {
                            configInfo.guid = gUID.ToString();
                            handlerItemConfigReturnResult.isUpdateJson = true;
                        }
                    }
                }
                EditorUtility.DisplayProgressBar("物品Asset配置更新中", $"正在创建{configInfo.config.name}targetName配置文件", progressIndex * 1.0f / itemConfigList.Length);
                
            }
            return handlerItemConfigReturnResult;
        }
        public struct HandlerItemConfigReturnResult
        {
            public bool isRestartup;
            public bool isUpdateJson;
        }
    }
}