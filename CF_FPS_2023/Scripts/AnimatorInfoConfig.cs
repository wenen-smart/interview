using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AnimatorInfoConfig : ScriptableObject
{
	public List<StateTransition> transitions = new List<StateTransition>();
	private Dictionary<int,Dictionary<int, List<StateTransition>>> transitionsDic = new Dictionary<int,Dictionary<int, List<StateTransition>>>();

	public void OnEnable()
	{
		int len = transitions.Count;
		for (int i = 0; i < len; i++)
		{
			int layer = transitions[i].layer;
			int shortNameHash = transitions[i].curStateShortNameHash;
			Dictionary<int, List<StateTransition>> statesTransDic = null;
			if (transitionsDic.ContainsKey(layer)==false)
			{
				statesTransDic = new Dictionary<int, List<StateTransition>>();
				transitionsDic.Add(layer,statesTransDic);
			}
			else
			{
				statesTransDic = transitionsDic[layer];
			}

			List<StateTransition> stateTransitions = null;
			if (statesTransDic.ContainsKey(shortNameHash) == false)
			{
				stateTransitions = new List<StateTransition>();
				statesTransDic.Add(shortNameHash, stateTransitions);
			}
			else
			{
				stateTransitions = statesTransDic[shortNameHash];
			}
			stateTransitions.Add(transitions[i]);
		}
	}

	public StateTransition GetStateTransition(int layer,int currentStateNameHash,int destinationStateNameHash)
	{

		if (!transitionsDic.ContainsKey(layer))
		{
			return null;
		}
		if (!transitionsDic[layer].ContainsKey(currentStateNameHash))
		{
			return null;
		}
		List<StateTransition> stateTransitions = transitionsDic[layer][currentStateNameHash];
		if (stateTransitions==null||stateTransitions.Count==0)
		{
			return null;
		}
		int len = stateTransitions.Count;
		for (int i = 0; i < len; i++)
		{
			if (stateTransitions[i].destStateShortNameHash==destinationStateNameHash)
			{
				return stateTransitions[i];
			}
		}
		return null;
	}
}

[Serializable]
public class StateTransition
{
	public int layer;
	public float exitTime;
	public string currentStateName;
	public int curStateShortNameHash; 
	//
	// 摘要:
	//     The duration of the transition.  单位时间
	public float duration;
	//
	// 摘要:
	//     The time at which the destination state will start. 单位时间
	public float offset;

	public string destinationStateName;
	public int destStateShortNameHash;
}
