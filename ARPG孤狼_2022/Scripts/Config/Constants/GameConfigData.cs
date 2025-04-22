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
		_Data_AirComboAttack=0,
		_Data_AnimatorRelationRootMotion=1,
		_Data_Buff=2,
		_Data_BuffEvent=3,
		_Data_TrollRootMotion=4,
		_Data_Skeleton_ArchierRootMotion=5,
		_Data_Skeleton_FootmanRootMotion=6,
		_Data_MotionModifyConfig=7,
		_Data_RoleObjectDefinition=8,
		_Data_PlayerRootMotion=9,
		_Data_ScriptManagerConfig=10,
}




public enum _Data_AirComboAttack
{
		Role_ID=0,
		Skill_ID=1,
		UpForce=2,
}




public enum _Data_AnimatorRelationRootMotion
{
		Animator_Name=0,
		RootMotionConfig=1,
}




public enum _Data_Buff
{
		BID=0,
		BuffClassType=1,
		Priority=2,
		ActionID=3,
		MID=4,
		BuffActionEffect=5,
		BuffBenefitType=6,
		BuffTag=7,
		BuffImmunityTag=8,
		Currency=9,
		Duration=10,
		BuffExecuteType=11,
		IntervalExecute=12,
		ExecuteCount=13,
		BuffOverrideType=14,
		MaxOverrideLayer=15,
}




public enum _Data_BuffEvent
{
		ActionID=0,
		OnBuffAwake=1,
		OnBuffStart=2,
		OnBuffRefresh=3,
		OnBuffRemove=4,
		OnBuffDestory=5,
		OnSkillExecuted=6,
		OnBeforeGiveDamage=7,
		OnAfterGiveDamage=8,
		OnBeforeTakeDamage=9,
		OnAfterTakeDamage=10,
		OnBeforeDead=11,
		OnAfterDead=12,
		OnBeforeKill=13,
		OnAfterKill=14,
		OnIntervalThink=15,
		OnIntervalThinkLifeFinish=16,
}




public enum _Data_TrollRootMotion
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




public enum _Data_Skeleton_ArchierRootMotion
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




public enum _Data_Skeleton_FootmanRootMotion
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




public enum _Data_MotionModifyConfig
{
		MID=0,
		MotionType=1,
		MotionOverride=2,
		VariableType=3,
		targetVariable=4,
		UnInterruptedCalcuateVelocity=5,
}




public enum _Data_RoleObjectDefinition
{
		ID=0,
		Name=1,
		Description=2,
		Prefab_Path=3,
		DropItemListID=4,
		DropKindRandomRange=5,
		HaveInvertory=6,
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


