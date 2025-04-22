using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ListenAnimationDistinct
{
    public int layer;
    public int shortNameHash;
    public float startNormalizeTime;
    public float endNormalizeTime;
    public bool onlyOnce;
    public Action callback;
    public ListenAnimationDistinct(int layer, int shortNameHash, float startNormalizeTime, float endNormalizeTime, bool onlyOnce, Action callback)
    {
        this.layer = layer;
        this.shortNameHash = shortNameHash;
        this.startNormalizeTime = startNormalizeTime;
        this.endNormalizeTime = endNormalizeTime;
        this.onlyOnce = onlyOnce;
        this.callback = callback;
    }
}

public class ListenAnimationClip : MonoBehaviour
{
    //layer->stateName ->listenData
    Dictionary<int, Dictionary<int,LinkedList<ListenAnimationDistinct>>> ListenAnimationDistinctsLayerDic = new Dictionary<int, Dictionary<int,LinkedList<ListenAnimationDistinct>>>();
    Animator animator;
	List<int> willRemove = new List<int>();
	private AnimatorStateInfo currentStateInfo;
	AnimatorStateInfo temp_nextStateInfo;
	int LID = -1;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void Listen(ListenAnimationDistinct listenAnimationDistinct)
    {
        //思路：先记录，然后在帧中检测 监听的动画是否是当前动画，如果不是就立即移出（不允许提前监听，即不允许去监听将来的动画状态）。
        //一个动画允许有多个监听。
        Dictionary<int,LinkedList<ListenAnimationDistinct>> listenAnimationDistinctsDic = null;
		if (ListenAnimationDistinctsLayerDic.ContainsKey(listenAnimationDistinct.layer)==false)
		{
            listenAnimationDistinctsDic = new Dictionary<int,LinkedList<ListenAnimationDistinct>>();
            ListenAnimationDistinctsLayerDic.Add(listenAnimationDistinct.layer,listenAnimationDistinctsDic);
		}
        else
		{
            listenAnimationDistinctsDic = ListenAnimationDistinctsLayerDic[listenAnimationDistinct.layer];
		}

		LinkedList<ListenAnimationDistinct> listenAnimationDistinctsLink = null;
		if (listenAnimationDistinctsDic.ContainsKey(listenAnimationDistinct.shortNameHash) == false)
		{
			listenAnimationDistinctsLink = new LinkedList<ListenAnimationDistinct>();
			
			listenAnimationDistinctsDic.Add(listenAnimationDistinct.shortNameHash, listenAnimationDistinctsLink);
		}
		else
		{
			listenAnimationDistinctsLink = listenAnimationDistinctsDic[listenAnimationDistinct.shortNameHash];
		}

        listenAnimationDistinctsLink.AddLast(listenAnimationDistinct);
    }

	bool StateIsExistListener(int layer,int shortNameHash)
	{
		return ListenAnimationDistinctsLayerDic.ContainsKey(layer) && ListenAnimationDistinctsLayerDic[layer].ContainsKey(shortNameHash);
	}

	public void RemoveListener(ListenAnimationDistinct listenAnimationDistinct)
	{
		int layer = listenAnimationDistinct.layer;
		int shortNameHash =listenAnimationDistinct.shortNameHash;
		if (StateIsExistListener(layer,shortNameHash))
		{
			ListenAnimationDistinctsLayerDic[layer][shortNameHash].Remove(listenAnimationDistinct);
		}
	}
	public void RemoveListener(int layer,int shortNameHash,float startNormalized)
	{
		if (StateIsExistListener(layer, shortNameHash))
		{
			LinkedList<ListenAnimationDistinct> listenAnimationDistincts = ListenAnimationDistinctsLayerDic[layer][shortNameHash];
			if (listenAnimationDistincts!=null)
			{
				LinkedListNode<ListenAnimationDistinct> listenAnimationDistinctNode = listenAnimationDistincts.First;
				while (listenAnimationDistinctNode!=null)
				{
					ListenAnimationDistinct distinct = listenAnimationDistinctNode.Value;
					if (distinct.layer == layer&&distinct.shortNameHash==shortNameHash&&MathfTools.GetNormalizeValue(distinct.startNormalizeTime-startNormalized)==0)
					{
						listenAnimationDistincts.Remove(listenAnimationDistinctNode);
					}
					listenAnimationDistinctNode = listenAnimationDistinctNode.Next;
					if (listenAnimationDistinctNode==null)
					{
						break;
					}
				}
			}
		}
	}

	public void LateUpdate()
    {
        if (ListenAnimationDistinctsLayerDic.Count>0)
        {
			foreach (var LayerPair in ListenAnimationDistinctsLayerDic)
			{
				int layer = LayerPair.Key;
                Dictionary<int,LinkedList<ListenAnimationDistinct>> ListenAnimationDistinctsDic =  LayerPair.Value;

				if (ListenAnimationDistinctsDic.Count==0)
				{
					continue;
				}

				var currentStateInfo = animator.GetCurrentAnimatorStateInfo(layer);
				bool isInTransition = animator.IsInTransition(0);
				
				if (isInTransition)
				{
					temp_nextStateInfo = animator.GetNextAnimatorStateInfo(layer);
				}
				foreach (var StatePair in ListenAnimationDistinctsDic)
				{
					//TODO : 还需要考虑过渡 A->B过渡 过渡区间内 CurState记录的A。
					//过渡区间应该 需要监听AB两个动画状态。
					if (isInTransition)
					{
						if (currentStateInfo.shortNameHash != StatePair.Key&&temp_nextStateInfo.shortNameHash != StatePair.Key)
						{
							willRemove.Add(StatePair.Key);
							continue;
						}
					}
					else
					{
						if (currentStateInfo.shortNameHash != StatePair.Key)
						{
							willRemove.Add(StatePair.Key);
							continue;
						}
					}
                    LinkedList<ListenAnimationDistinct> listenAnimationDistincts = ListenAnimationDistinctsDic[StatePair.Key];

					if (listenAnimationDistincts.Count==0)
					{
						continue;
					}
                    LinkedListNode<ListenAnimationDistinct> listenAnimationDistinctNode = listenAnimationDistincts.First;

					AnimatorStateInfo relativeStateInfo = currentStateInfo;
					if (currentStateInfo.shortNameHash != StatePair.Key && isInTransition)
					{
						relativeStateInfo = temp_nextStateInfo;
					}

					while (listenAnimationDistinctNode!=null)
					{
						ListenAnimationDistinct listenAnimationDistinct = listenAnimationDistinctNode.Value;
						bool isInDistinct = relativeStateInfo.normalizedTime >= listenAnimationDistinct.startNormalizeTime;
						if (isInDistinct)
						{
							//在区间
							listenAnimationDistinct.callback?.Invoke();
							if (listenAnimationDistinct.onlyOnce || relativeStateInfo.normalizedTime > listenAnimationDistinct.endNormalizeTime)
							{
                                listenAnimationDistincts.Remove(listenAnimationDistinctNode);
							}
						}
						listenAnimationDistinctNode = listenAnimationDistinctNode.Next;
					}
				}

				for (int i = willRemove.Count-1; i >=0; --i)
				{
					ListenAnimationDistinctsDic.Remove(willRemove[i]);
					willRemove.RemoveAt(i);
				}
			}
        }
    }
}
