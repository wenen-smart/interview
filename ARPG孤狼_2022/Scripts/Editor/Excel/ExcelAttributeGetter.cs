using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ExcelAttributeGetter
{
    public static List<string> GetEnumSelectDataValidation(Type enumType,string attribute)
    {
        if (enumType==null)
        {
            return null;
        }
        List<string> titleEnums =Enum.GetNames(enumType).ToList();
        if (ExcelHelper.IsHaveShowNumberAttribute(attribute)&&ExcelHelper.IsHaveMultAttribute(attribute))
        {
            for (int i = 0; i < titleEnums.Count; i++)
            {
                int num = (int)Enum.Parse(enumType, titleEnums[i]);
                titleEnums[i] += " = " + num;
            }
        }
        return titleEnums;
    } 
}

