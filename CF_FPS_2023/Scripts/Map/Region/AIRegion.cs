using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Assets.Resolution.Scripts.Region
{
    [RequireComponent(typeof(BoxCollider))]
    public class AIRegion : MonoBehaviour
    {
        public IntoRegionPath[] intoRegion;//入口
        public Transform[] pointNodes;
        public Transform[] fallPoints;
        public Transform exit;//出口  当逃离时优先选择出口方向后退 再根据环境抉择是从跳落点跳下还是从出口出去。如果没有出口则超敌人逼近的方向后退或找到跳落点。
        private Dictionary<int,RobotController>  botOccupyDics=new Dictionary<int,RobotController>();
        public List<RobotController> stayedList=new List<RobotController>();
        public bool IsRemainCanStandPoint()
        {
            return (botOccupyDics.Count == pointNodes.Length)==false;
        }
        public Transform PreOccupy(RobotController robotController)
        {
            if (IsRemainCanStandPoint()==false)
            {
                return null;
            }
			Transform target = GetUnoccpiedPoint();
            if (target==null)
            {
                return null;
            }
            botOccupyDics.Add(target.GetInstanceID(), robotController);
            return target;
        }
		public bool PreOccupy(RobotController robotController,Transform point)
		{
            int instanceId = point.GetInstanceID();
			if (pointNodes.Contains(point)==false)
			{
                Debug.LogError("传入的点 不是区域中的PointNode");
                return false;
			}
            bool  haveOccipied = botOccupyDics.ContainsKey(instanceId);
			if (haveOccipied==false)
			{
                botOccupyDics.Add(instanceId,robotController);
                return true;
			}
			if (botOccupyDics[instanceId] == robotController)
			{
                return true;
			}
            return false;
		}

		public Transform GetUnoccpiedPoint()
		{
			if (IsRemainCanStandPoint() == false)
			{
				return null;
			}
			int instanceId = 0;
			Transform target = null;
			if (botOccupyDics.Count == 0)
			{
				int index = Random.Range(0, pointNodes.Length);
				target = pointNodes[index];
			}
			else
			{
				foreach (var node in pointNodes)
				{
					instanceId = node.GetInstanceID();
					if (botOccupyDics.ContainsKey(instanceId) == false)
					{
						target = node;
						break;
					}
				}
			}
			return target;
		}
        public void CancelOccpy(RobotController robotController)
        {
            if (botOccupyDics.ContainsValue(robotController))
            {
                var resultPair = botOccupyDics.Single((pair)=> { return pair.Value == robotController; });
                botOccupyDics.Remove(resultPair.Key);
                stayedList.Remove(robotController);
            }
        }
        public Transform GetOccupyPosition(RobotController robotController)
        {
            if (botOccupyDics.ContainsValue(robotController))
            {
                var resultPair = botOccupyDics.Single((pair) => { return pair.Value == robotController; });
                return pointNodes.Single((p)=> { return p.GetInstanceID() == resultPair.Key; });
            }
            return null;
        }
        //Trigger: Check Bot Into  this Area
        private void OnTriggerEnter(Collider other)
        {
            ActorComponent actorComponent = other.GetComponent<ActorComponent>();
            if (actorComponent)
            {
                var robotController = actorComponent.actorSystem.GetActorComponent<RobotController>();
                if (robotController)
                {
					foreach (var item in botOccupyDics)
					{
						if (item.Value==robotController)
						{
                            stayedList.Add(robotController);
                            robotController.IntoNewRegion(this); 
                            return;
						}
					}
                }
            }
        }
        //也可能die 也可能逃离该区域。
        private void OnTriggerExit(Collider other)
        {
            if (botOccupyDics.Count == 0)
            {
                return;
            }
            ActorComponent actorComponent = other.GetComponent<ActorComponent>();
            if (actorComponent)
            {
                var robotController = actorComponent.actorSystem.GetActorComponent<RobotController>();
                if (robotController&&stayedList.Contains(robotController))
                {
                    robotController.LeaveRegion(this);
                }
            }
        }
    }
}