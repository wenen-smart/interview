using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Table;
using System.ComponentModel;
using System.Data;
using OfficeOpenXml.DataValidation.Contracts;
using System;
using System.Linq;
using System.Reflection;
using OfficeOpenXml.DataValidation;
using System.Text.RegularExpressions;
using System.Text;

public class ExcelHelper 
{
    public static readonly string pattern_excelTitle= "(?<title>.+?)";
    public static readonly string pattern_excelTitle2 = "(?<title>.+?)\\((?<param>.+?)\\)$";
    public static readonly string patternAttribute_excelTitle1="(?<title>.+?)\\((?<attribue>.+?)\\)"; 
    
    public static Excel LoadExcel(string filePath)
    {
        FileInfo fileInfo = new FileInfo(filePath);//完整路径
        ExcelPackage excelPackage = new ExcelPackage(fileInfo);//Excel包体
        ExcelWorksheets excelWorksheets = excelPackage.Workbook.Worksheets;
        ExcelEdit(excelPackage);
        Excel excel = new Excel();
        excel.Tables = new List<ExcelTable>();
        for (int i = 1; i < excelWorksheets.Count+1; i++)
        {
            ExcelWorksheet sheet=excelWorksheets[i];//i号表
            int row=sheet.Dimension.End.Row;
            int col=sheet.Dimension.End.Column;

            ExcelTable excelTable = new ExcelTable(sheet);
            if (excelTable != null)
            {
                excel.Tables.Add(excelTable);
            }
        }
        excelPackage.Save();
        excelPackage.Dispose();
        return excel;
        
    }
    public static void ExcelEdit(ExcelPackage excelPackage)
    {
        ExcelWorksheets excelWorksheets = excelPackage.Workbook.Worksheets;
        for (int i = 1; i < excelWorksheets.Count+1; i++)
        {
            ExcelWorksheet sheet = excelWorksheets[i];//i号表
            HandlerExcelDataValidation(excelPackage,sheet,sheet.Name);
        }
    }
    public static void HandlerExcelDataValidation(ExcelPackage excelPackage,ExcelWorksheet sheet, ExcelAssetEnum excelEnum)
    {
        try
        {
            switch (excelEnum)
            {
                default:

                    sheet.DataValidations.RemoveAll(x => { return true; });
                    sheet.DataValidations.Clear();

                    excelPackage.Save();
                    //Array excel_Buff=Enum.GetNames(typeof(_Data_Buff));
                    object[,] cell = sheet.Cells.Value as object[,];
                    int colNum = cell.GetLength(1);
                    for (int i = 0; i < colNum; i++)
                    {
                        string titleStr = cell[0, i].ToString();
                        var match = MatchTitle(titleStr);
                        titleStr = match.Item1;
                        if (string.IsNullOrEmpty(match.Item2) == false)
                        {
                            List<string> paramStr = MatchParam(match.Item2);
                            HandleExcelTitle(sheet, i, titleStr, paramStr);
                        }
                    }
                    break;
            }
        }
        catch (Exception e)
        {
            MyDebug.DebugSupperError($"{sheet.Name}表数据验证发生错误");
            throw e;
        }
    }
    public static void HandlerExcelDataValidation(ExcelPackage excelPackage, ExcelWorksheet sheet, string sheetName)
    {
        sheetName = "_Data_" + sheetName;
        ExcelAssetEnum excelAssetEnum;
        if (Enum.TryParse<ExcelAssetEnum>(sheetName, out excelAssetEnum))
        {
            HandlerExcelDataValidation(excelPackage,sheet,excelAssetEnum);

        }
    }
    public static void AddListDataValidation(ExcelWorksheet sheet,int colIndex,List<string> titleEnums)
    {
        if (sheet.Dimension.End.Row<2)
        {
            return;
        }
        var dataVali = sheet.Cells[2, colIndex+1, sheet.Dimension.End.Row, colIndex+1].DataValidation;
        var valiadtion = dataVali.AddListDataValidation();
        valiadtion.AllowBlank = true;
        foreach (var item in titleEnums)
        {
            valiadtion.Formula.Values.Add(item);
        }
    }
    public static void AddListDataValidation(ExcelWorksheet sheet, int rowIndex, int colIndex, List<string> titleEnums)
    {
        if (sheet.Dimension.End.Row < 2)
        {
            return;
        }
        var dataVali = sheet.Cells[2, colIndex + 1, rowIndex, colIndex + 1].DataValidation;
        var valiadtion = dataVali.AddListDataValidation();
        valiadtion.AllowBlank = true;
        foreach (var item in titleEnums)
        {
            valiadtion.Formula.Values.Add(item);
        }
    }

    public static void AddListDataValidation(ExcelWorksheet sheet, int colIndex,Type type)
    {
        List<string> titleEnums = Enum.GetNames(type).ToList();
        AddListDataValidation(sheet,colIndex,titleEnums);
    }
    public static void AddListDataValidation(ExcelWorksheet sheet,int rowIndex, int colIndex, Type type)
    {
        List<string> titleEnums = Enum.GetNames(type).ToList();
        AddListDataValidation(sheet, rowIndex,colIndex, titleEnums);
    }
    public static Type GetCorrentType(string className)
    {
        return AssemblyTool.GetCorrentType(className);
    }
    public  static string IndexToColumn(int index)
    {
        if (index <= 0)
        {
            throw new Exception("Invalid parameter");
        }
        index--;
        string column = string.Empty;
        do
        {
            if (column.Length > 0)
            {
                index--;
            }
            column = ((char)(index % 26 + (int)'A')).ToString() + column;
            index = (int)((index - index % 26) / 26);
        } while (index > 0);

        return column;
    }
    public static Tuple<string,string> MatchTitle(string titleStr)
    {
        Regex regex = new Regex(pattern_excelTitle);
        Regex regex2 = new Regex(pattern_excelTitle2);
        var matches = regex2.Matches(titleStr);
        StringBuilder titleBuilder = new StringBuilder();
        StringBuilder paramBuilder = new StringBuilder();
        if (matches.Count==0)
        {
            matches = regex.Matches(titleStr);
        }
        foreach (Match match in matches)
        {
            string titleMatch = match.Groups["title"].Value;
            if (string.IsNullOrEmpty(titleMatch) == false)
            {
                titleBuilder.Append(titleMatch);
            }
            string paramMatch = match.Groups["param"].Value;
            if (string.IsNullOrEmpty(paramMatch) == false)
            {
                paramBuilder.Append(paramMatch);
            }
        }
        return new Tuple<string, string>(titleBuilder.ToString(),paramBuilder.ToString());
    }
    public static List<string> MatchParam(string paramStr)
    {
        List<string> param_PatternList = new List<string>();
        param_PatternList=paramStr.Split('#').Where((str)=> { return !string.IsNullOrEmpty(str); }).ToList();
        param_PatternList.ForEach((str)=> { str = str.Trim();});
        return param_PatternList;
    }
    public static void HandleExcelTitle(ExcelWorksheet sheet, int colIndex,string title,List<string> patternStr)
    {
        for (int i = 0; i < patternStr.Count; i++)
        {
            Regex regex = new Regex(patternAttribute_excelTitle1);
            StringBuilder patternBuilder = new StringBuilder();
            StringBuilder attributeBuilder = new StringBuilder(); 
            var matches = regex.Matches(patternStr[i]);
            if (matches.Count>0)
            {
                foreach (Match match in matches)
                {
                    string pattern_temp = match.Groups["title"].Value;
                    if (string.IsNullOrEmpty(pattern_temp) == false)
                    {
                        patternBuilder.Append(pattern_temp);
                    }
                    string attribute_Temp = match.Groups["attribue"].Value;
                    if (string.IsNullOrEmpty(attribute_Temp) == false)
                    {
                        attributeBuilder.Append(attribute_Temp);
                    }
                }
            }
            else
            {
                patternBuilder.Append(patternStr[i]);
            }
            string pattern =patternBuilder.ToString();
            string[] arrayStr = pattern.Split('=');
            string attributeStr=attributeBuilder.ToString();
            var patternName=arrayStr[0];
            var patternValue = "";
            if (arrayStr.Length>1)
            {
                patternValue = arrayStr[1];
            }
            switch (patternName)
            {
                case "select":
                    if (string.IsNullOrEmpty(patternValue))
                    {
                        patternValue = title;
                    }
                    SelectLogic(sheet,colIndex,patternValue,attributeStr);
                    break;
                case "class":
                    ClassSelectLogic(sheet,colIndex,patternValue,attributeStr);
                    break;
                default:
                    break;
            }
        }
    }

    public  static void SelectLogic(ExcelWorksheet sheet, int colIndex,string patternValue,string attribute)
    {
        Type titleEnum = GetCorrentType(patternValue);
        List<string> dataValidations = ExcelAttributeGetter.GetEnumSelectDataValidation(titleEnum,attribute);
        if (IsHaveMultAttribute(attribute))
        {
            AddListDataValidation(sheet,2,colIndex, dataValidations);
        }
        else
        {
            if (titleEnum != null)
            {
                AddListDataValidation(sheet, colIndex, dataValidations);
            }
        }
    }
    public static void ClassSelectLogic(ExcelWorksheet sheet, int colIndex,string patternValue,string attribute)
    {
        //patternValue这里是基类类名 找出所有子类

        Type baseType = AssemblyMainTool.GetConfigTypeByAssembley(patternValue);
        Assembly assembly = baseType.Assembly;
        Type[] Types = assembly.GetTypes();
        List<string> typeNames = Types.Where((item) => { return item.IsSubclassOf(baseType); }).Select<Type, string>((item) => { return item.FullName; }).ToList();
        if (baseType.IsAbstract == false && baseType.IsInterface == false)
        {
            typeNames.Insert(0, patternValue);
        }
        AddListDataValidation(sheet, colIndex, typeNames);
    }
    public static List<string> GetAttribute(string attributeStr)
    {
        if (string.IsNullOrEmpty(attributeStr) == false)
        {
            return attributeStr.Split('^').ToList();
        }
        return null;
    }
    public static bool IsHaveMultAttribute(string pattern_Value)
    {
        List<string> attributes= GetAttribute(pattern_Value);
        return IsContainAttribute(attributes,"Mult");
    }
    public static bool IsHaveShowNumberAttribute(string pattern_Value)
    {
        List<string> attributes = GetAttribute(pattern_Value);
        return IsContainAttribute(attributes, "ShowNumber");
    }
    public static bool IsContainAttribute(List<string> attributes, string attribute)
    {
        if (attributes == null)
        {
            return false;
        }
        if (attributes.Contains(attribute))
        {
            return true;
        }
        return false;
    }
}
