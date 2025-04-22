using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ExcelTable
    {
    public string _tableName;
    public string[,] _Rows;

    public ExcelTable(ExcelWorksheet worksheet)
    {
        this._tableName = worksheet.Name;
        object[,] objRows = worksheet.Cells.Value as object[,];
        int rowNum = objRows.GetLength(0);
        int colNum = objRows.GetLength(1);
        _Rows = new string[rowNum,TableVaildColCount(objRows)];
        int filterCount = 0;

        for (int j = 0; j < colNum; j++)
        {
            for (int i = 0; i < rowNum; i++)
            {
                string content = Convert.ToString(objRows[i, j]);
                if (i == 0)
                {
                    if (IsFilter(content))
                    {
                        filterCount++;
                        break;
                    }
                }

                _Rows[i, j-filterCount] = content;
            }
        }

        if (_Rows==null)
        {
            Debug.LogError($"{_tableName}-_Rows: is NullReference");
        }
    }
    public int TableVaildColCount(object[,] objRows)
    {
        int rowNum = objRows.GetLength(0);
        int colNum = objRows.GetLength(1);
        int vaildCol = colNum;
        for (int j = 0; j < colNum; j++)
        {
            string content = Convert.ToString(objRows[0, j]);
            if (IsFilter(content))
            {
                vaildCol--;
                continue;
            }
        }
        return vaildCol;
    }
    public bool IsFilter(string content)
    {
        if (content.StartsWith("#"))
        {
            return true;
        }
        return false;
    }

}

