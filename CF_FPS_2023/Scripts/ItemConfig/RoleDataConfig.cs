using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class RoleDataConfig:ScriptableConfig
{
	public RoleType roleType;
	//public abstract void StateMachineInit();
	//public abstract void StateMachineUpdate();
}

public enum RoleType
{
Human,
Monster
}
