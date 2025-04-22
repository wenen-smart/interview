using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ExcelDataObject : ScriptableObject
{
    public string sheetName;
    public List<ExcelRowData> _Rows;
    public List<string> _Col_Head;

    public bool HasHeadCol(string colHeadName)
    {
        return _Col_Head != null && _Col_Head.Contains(colHeadName);
    }
}
[Serializable]
public class ExcelRowData
{
    public string[] _Data;
    public void CreateRow(string[] data)
    {
        _Data = data;
    }
    public string this[int i]
    {
        get
        {
            return _Data[i];
        }
    }
}

