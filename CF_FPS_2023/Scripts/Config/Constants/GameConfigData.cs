using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameConfigEnum
{
//Empty
}
//There Under are _GameConfigDataEnum



public enum ExcelAssetEnum
{
		_Data_AnimatorRelationRootMotion=0,
		_Data_BodyDamageRateConfig=1,
		_Data_PlayerRootMotion=2,
		_Data_ScriptManagerConfig=3,
}




public enum _Data_AnimatorRelationRootMotion
{
		Animator_Name=0,
		RootMotionConfig=1,
}




public enum _Data_BodyDamageRateConfig
{
		ID=0,
		Head=1,
		Body=2,
		Arm=3,
		Thigh=4,
		LowerLeg=5,
}




public enum _Data_PlayerRootMotion
{
		AnimClipJudgeWay=0,
		HandlerAim=1,
		Priority=2,
		Name=3,
		Tag=4,
		Mult=5,
		PosCalcuateModel=6,
		RotationCalculateModel=7,
		PosProcessClear=8,
		RotationProcessClear=9,
		Debug=10,
}




public enum _Data_ScriptManagerConfig
{
		Script_Name=0,
		Load_Sequence=1,
		IsNewContrust=2,
}


