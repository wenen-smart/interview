using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAlimentDataConfig", menuName = "CreateConfig/CreateAlimentDataConfig", order = 1)]
public class AlimentDataConfig:ItemDataConfig
{
    public int addHp;
    public int addMp;
}

