using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System;
using System.Linq;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Text;
using System.Globalization;

[ScriptedImporter(1, "xlsx")]
public class ExcelObjectImport : ScriptedImporter
{

    public static string sourcePath = "/Miracle_Data/Source/";
    public static string fileExtension = ".xlsx";
    public static string assetFilePath = "Assets/Miracle_Data/ScriptableObject/Resources/";
    public static string gameEnumScriptPath="Assets/Scripts/Config/Constants/GameConfigData.cs";
    public static string gameEnumTemplateName= "Template_GameConfigEnum.txt";
    public static string ExcelAssetFileEnum = "ExcelAssetEnum";
    public override void OnImportAsset(AssetImportContext ctx)
    {

    }

    public static ExcelDataObject GeneralExcelData(ExcelTable excelTable)
    {
        string tableName = excelTable._tableName;
        ExcelDataObject excelDataObject=ExcelDataObject.CreateInstance<ExcelDataObject>();
        excelDataObject._Rows = new List<ExcelRowData>();
        excelDataObject._Col_Head = new List<string>();
        string[,] _Rows = excelTable._Rows;
        if (_Rows==null)
        {
            return null;
        }
        int colNumber = _Rows.GetLength(1);
        
        for (int i = 0; i < colNumber; i++)
        {
            string colHead=_Rows[0, i];//第一行
            colHead=ExcelHelper.MatchTitle(colHead).Item1;
            if (string.IsNullOrEmpty(colHead)||colHead.StartsWith("#"))
            {
                continue;
            }
            excelDataObject._Col_Head.Add(colHead);
        }
        int vaildColNum = excelDataObject._Col_Head.Count;
        //Filters
        for (int r = 1; r <_Rows.GetLength(0); r++)
        {
            if (r==1)
            {
                //忽略第二行内容   -- 第二行为模板内容不加载进scriptObject文件
                continue;
            }
            ExcelRowData excelRowData = new ExcelRowData();
            string[] colsData = new string[colNumber];
            int emptyCount = 0;
            for (int i =0; i < colNumber; i++)
            {
                string cellData = _Rows[r,i];
                string colHead = _Rows[0, i];//第一行
                if (excelDataObject.HasHeadCol(ExcelHelper.MatchTitle(colHead).Item1) ==false)
                {
                    continue;
                }
                if (string.IsNullOrEmpty(cellData)||string.IsNullOrWhiteSpace(cellData))
                {
                    emptyCount++;
                    continue;
                }
                colsData[i] = cellData;
            }
            if (emptyCount>= vaildColNum)
            {
                continue;
            }
            excelRowData.CreateRow(colsData);
            excelDataObject._Rows.Add(excelRowData);
        }
        return excelDataObject;


    }
    public static void GenerateScriptEnumName(List<ExcelDataObject> excelDataAssets)
    {
        if (Directory.Exists(PathDefine.TemplateFileDirectory)==false)
        {
            Directory.CreateDirectory(PathDefine.TemplateFileDirectory);
        }
        FileInfo fileInfo = new FileInfo(gameEnumScriptPath);
        if (fileInfo.Exists)
        {
            fileInfo.Delete();
        }
        FileStream fileStream=fileInfo.Create();
        FileInfo templateInfo = new FileInfo(PathDefine.TemplateFileDirectory+ gameEnumTemplateName);
        FileStream templateInfoReader=templateInfo.OpenRead();
        byte[] context = new byte[1024];
        int offset = 0;
        int length = context.Length;
        while (true)
        {
            int _len= templateInfoReader.Read(context, offset, length);
            fileStream.Write(context,offset,_len);
            offset += _len;
            if (_len < length)
            {
                break;
            }
            length -= _len;
            if (length<=0)
            {
                length = context.Length;
                offset = 0;
            }
        }
        templateInfoReader.Close();

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"\n\npublic enum {ExcelAssetFileEnum}");
        stringBuilder.AppendLine("{");
        int assetIndex = 0;
        foreach (var asset in excelDataAssets)
        {
            stringBuilder.AppendLine($"\t\t{asset.name}={assetIndex},");
            assetIndex++;
        }
        stringBuilder.AppendLine("}\n\n");
        foreach (var asset in excelDataAssets)
        {
            stringBuilder.AppendLine($"\n\npublic enum {asset.name}");
            stringBuilder.AppendLine("{");
            for (int i = 0; i < asset._Col_Head.Count; i++)
            {
                string headTitle=asset._Col_Head[i];
                stringBuilder.AppendLine($"\t\t{headTitle}={i},");
            }
            stringBuilder.AppendLine("}\n\n");
        }
        offset = 0;
        string str = stringBuilder.ToString();
        int writeOnce = 100;
        byte[] appendBytes=Encoding.UTF8.GetBytes(str);
        while (true)
        {
            int remain = appendBytes.Length - offset;
            if (remain<=writeOnce)
            {
                writeOnce = remain;
            }
            fileStream.Write(appendBytes, offset, writeOnce);
            offset += writeOnce;
            if (offset>=appendBytes.Length)
            {
                break;
            }
        }
        
        fileStream.Close();

    }
    public static string FilterHandlerAssetName(string tableName)
    {
        tableName=tableName.TrimStart('_');
        return "_Data_"+tableName+".asset";
    }
    public static string FilterHandlerScriptName(string tableName)
    {

        return tableName;
    }
}


/// <summary>
/// 对导入的资源做设置
/// </summary>
public class ImportAssetsPostprocessor : AssetPostprocessor
{
    public static bool isShow = false;
    public static double lastSaveTime = 0;
    public static bool lastManualExecute;
    //所有的资源的导入，删除，移动，都会调用此方法，注意，这个方法是static的
    public static void OnPostprocessAllAssets(string[] importedAsset, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (isShow || PETime.PETimeTools.ConvertToSeconds(PETime.PETimeTools.GetUTCMillSeconds() -lastSaveTime,PETime.PETimeUnit.MillSeconds)<1f)
        {
            return;
        }
        var gameDataImport = false;
        var processedGameDataAsset = new List<string>();
        foreach (var asset in importedAsset.ToList())
        {
            if (asset.StartsWith($"Assets{ExcelObjectImport.sourcePath}")&&asset.EndsWith(ExcelObjectImport.fileExtension)&&(asset.Contains("~$")==false))
            {
                gameDataImport = true;
                isShow = true;
                processedGameDataAsset.Add(Path.GetFileNameWithoutExtension(asset));
            }
        }
        if (gameDataImport)
        {
            if (!EditorUtility.DisplayDialog("ExcelImport", "检测到Excel数据修改，是否重新生成配置?", "Sure","Cancel"))
            {
                isShow = false;
                return;
            }
            var excelTableList = new Dictionary<string, List<ExcelDataObject>>();
            EditorUtility.ClearProgressBar();//清空进度
            EditorUtility.DisplayProgressBar("Miracle - Importing Game Data","importing excel and covert to .asset file",0);
            EditorUtility.DisplayProgressBar("Miracle - Processing Game Data", "Loading...", 1f);
            var gameDataFiles = Directory.GetFiles($"Assets{ExcelObjectImport.sourcePath}","*.xlsx",SearchOption.AllDirectories).Where(name
                =>name.Contains("~$")==false).ToList();
            var index = 0;
            if (Directory.Exists(ExcelObjectImport.assetFilePath))
            {
                Directory.Delete(ExcelObjectImport.assetFilePath,true);
            }
            List<ExcelDataObject> excelDataAssets = new List<ExcelDataObject>();
            foreach (var file in gameDataFiles)
            {
                var basicFile = Path.GetFileNameWithoutExtension(file);
                EditorUtility.DisplayProgressBar("Miracle - Processing Game Data", $"({index+1}/{gameDataFiles.Count}){file}", index*1.0f/gameDataFiles.Count-1);
                Excel excel=ExcelHelper.LoadExcel(file);
                foreach (ExcelTable table in excel.Tables)
                {
                    var tableName = table._tableName;
                    if (tableName.StartsWith("Sheet"))
                    {
                        continue;
                    }
                    var assetName = ExcelObjectImport.FilterHandlerAssetName(tableName);
                    
                    ExcelDataObject excelDataObject = ExcelObjectImport.GeneralExcelData(table);
                    excelDataObject.sheetName = tableName;
                    if (Directory.Exists(ExcelObjectImport.assetFilePath)==false)
                    {
                        Directory.CreateDirectory(ExcelObjectImport.assetFilePath);
                    }
                    excelDataAssets.Add(excelDataObject);
                    AssetDatabase.CreateAsset(excelDataObject,$"{ExcelObjectImport.assetFilePath}{assetName}");
                    EditorUtility.SetDirty(excelDataObject);
                    
                }
                index++;
            }
            if (excelDataAssets!=null&& excelDataAssets.Count>0)
            {
                ExcelObjectImport.GenerateScriptEnumName(excelDataAssets);
            }
            EditorUtility.DisplayProgressBar("Miracle - Processing Game Data", "Saving...", 1f);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
            if (EditorUtility.DisplayDialog("ExcelImport", "数据表导入成功", "Sure"))
            {
                lastSaveTime = PETime.PETimeTools.GetUTCMillSeconds();
                isShow = false;
                return;
            }
        }
    }

    [MenuItem("Tools/Config/更新XLSX配置文件")]
    public static void OnUpdateExcelAsset()
    {
        if (!EditorUtility.DisplayDialog("ExcelImport", "检测到Excel数据修改，是否重新生成配置?", "Sure", "Cancel"))
        {
            return;
        }
        var excelTableList = new Dictionary<string, List<ExcelDataObject>>();
        EditorUtility.ClearProgressBar();//清空进度
        EditorUtility.DisplayProgressBar("Miracle - Importing Game Data", "importing excel and covert to .asset file", 0);
        EditorUtility.DisplayProgressBar("Miracle - Processing Game Data", "Loading...", 1f);
        var gameDataFiles = Directory.GetFiles($"Assets{ExcelObjectImport.sourcePath}", "*.xlsx", SearchOption.AllDirectories).Where(name
              => name.Contains("~$") == false).ToList();
        var index = 0;
        if (Directory.Exists(ExcelObjectImport.assetFilePath))
        {
            Directory.Delete(ExcelObjectImport.assetFilePath, true);
        }
        List<ExcelDataObject> excelDataAssets = new List<ExcelDataObject>();
        foreach (var file in gameDataFiles)
        {
            var basicFile = Path.GetFileNameWithoutExtension(file);
            EditorUtility.DisplayProgressBar("Miracle - Processing Game Data", $"({index + 1}/{gameDataFiles.Count}){file}", index * 1.0f / gameDataFiles.Count - 1);
            Excel excel = ExcelHelper.LoadExcel(file);
            foreach (ExcelTable table in excel.Tables)
            {
                var tableName = table._tableName;
                if (tableName.StartsWith("Sheet"))
                {
                    continue;
                }
                var assetName = ExcelObjectImport.FilterHandlerAssetName(tableName);

                ExcelDataObject excelDataObject = ExcelObjectImport.GeneralExcelData(table);
                excelDataObject.sheetName = tableName;
                if (Directory.Exists(ExcelObjectImport.assetFilePath) == false)
                {
                    Directory.CreateDirectory(ExcelObjectImport.assetFilePath);
                }
                excelDataAssets.Add(excelDataObject);
                AssetDatabase.CreateAsset(excelDataObject, $"{ExcelObjectImport.assetFilePath}{assetName}");
                EditorUtility.SetDirty(excelDataObject);

            }
            index++;
        }
        if (excelDataAssets != null && excelDataAssets.Count > 0)
        {
            ExcelObjectImport.GenerateScriptEnumName(excelDataAssets);
        }
        EditorUtility.DisplayProgressBar("Miracle - Processing Game Data", "Saving...", 1f);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
        lastSaveTime = PETime.PETimeTools.GetUTCMillSeconds();
        if (EditorUtility.DisplayDialog("ExcelImport", "数据表导入成功", "Sure"))
        {
            return;
        }
    }
}

