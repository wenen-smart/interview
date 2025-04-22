using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Resolution.Scripts.Map
{
    [RequireComponent(typeof(BoxCollider))]
    public class SpawnArea : MonoBehaviour
    {
        protected BoxCollider areaRange;
        //-1为无限
        public int maxFillCount = -1;
        private int fillCount = 0;
        private List<Vector3> aliveEntityPoints=new List<Vector3>();

        public void Awake()
        {
            if (areaRange==null)
            {
                GetAreaInfo();
            }
        }
        private void GetAreaInfo()
        {
            areaRange = GetComponent<BoxCollider>();
            areaRange.isTrigger = true;
            areaRange.enabled = false;
        }
        public bool IsCanSpawn()
        {
            if (maxFillCount==-1)
            {
                return true;
            }
            return fillCount < maxFillCount;
        }
        public Vector3 GetSpawnPointInXZ(float minSpawnPadding,int MaxIterationCount)
        {
            if (areaRange == null)
            {
                GetAreaInfo();
            }
            var range = transform.localScale;
            range.x *= areaRange.size.x;
            range.z *= areaRange.size.z;
            var halfRange = range / 2;
            var xoffset = UnityEngine.Random.Range(-halfRange.x, halfRange.x);
            var zoffset = UnityEngine.Random.Range(-halfRange.z, halfRange.z);
            var point = transform.position+areaRange.center;
            point.y = transform.position.y;
            point += transform.right*xoffset;
            point += transform.forward*zoffset;
            if (aliveEntityPoints.Count!=0&&minSpawnPadding>0&&MaxIterationCount>1)
            {
                MaxIterationCount -= 1;
                for (int i = 0; i < MaxIterationCount; i++)
                {
                    //Return the last result in the loop  when current index over MaxIterationCount;
                    Vector3 temp = GetSpawnPointInXZ(0,0);
                    bool isSatisfy = aliveEntityPoints.All((pos) => (pos - temp).magnitude > minSpawnPadding);
                    if (isSatisfy)
                    {
                        point = temp;
                        break;
                    }
                }
            }
            return point;
        }
        public bool RecordSpawnedInHere()
        {
            if (IsCanSpawn())
            {
                fillCount += 1;
                return true;
            }
            return false;
        }
        public bool SpawnInTheArea(out Vector3 point,float minSpawnPadding,int MaxIterationCount)
        {
            if (IsCanSpawn())
            {
               point = GetSpawnPointInXZ(minSpawnPadding,MaxIterationCount);
               aliveEntityPoints.Add(point);
            }
            else
            {
                point = Vector3.zero;
            }
            return RecordSpawnedInHere();
        }
        public void ClearFillCount()
        {
            fillCount = 0;
        }
    }
}
