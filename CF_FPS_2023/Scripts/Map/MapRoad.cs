using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Resolution.Scripts.Map
{
    [System.Serializable]
    public class MapRoad
    {
        public Transform[] toTargets;
        public Transform point;
    }
    public class RoadPointNode
    {
        public RoadPointNode[] nexts;
        public List<RoadPointNode> priors;
        public Transform point;
        public RoadPointNode NextRandom(NextRoadTreeType nextRoadTreeType)
        {
            switch (nextRoadTreeType)
            {
                case NextRoadTreeType.NextTree:
                    return nexts[UnityEngine.Random.Range(0, nexts.Length)];
                case NextRoadTreeType.PriorTree:
                    return priors[UnityEngine.Random.Range(0, priors.Count)];
                case NextRoadTreeType.None:
                default:
                    break;
            }
            return null;
        }
        public bool IsExistTheNodeInNextTree(RoadPointNode target)
        {
            if (nexts==null||nexts.Length==0)
            {
                return false;
            }
            if (nexts.Contains(target))
            {
                return true;
            }
            foreach (var node in nexts)
            {
                if (node.IsExistTheNodeInNextTree(target))
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsExistTheNodeInPriorTree(RoadPointNode target)
        {
            if (priors == null || priors.Count == 0)
            {
                return false;
            }
            if (priors.Contains(target)) 
            {
                return true;
            }
            foreach (var node in priors)
            {
                if (node.IsExistTheNodeInPriorTree(target))
                {
                    return true;
                }
            }
            return false;
        }
    }
    public enum NextRoadTreeType
    {
        None,
        NextTree,
        PriorTree,
    }
}
