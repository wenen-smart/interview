using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_JumpLinkTraverser : MonoBehaviour
{
    RichAI ai;
    void OnEnable()
    {
        ai = GetComponent<RichAI>();
        if (ai != null) ai.onTraverseOffMeshLink += TraverseOffMeshLink;
    }

    void OnDisable()
    {
        if (ai != null) ai.onTraverseOffMeshLink -= TraverseOffMeshLink;
    }

    protected virtual IEnumerator TraverseOffMeshLink(RichSpecial linkInfo)
    {
        if (!(linkInfo.nodeLink is AI_JumpLink))
        {
            yield return null;
        }

        AI_JumpLink link=linkInfo.nodeLink as AI_JumpLink;
        var saveCurrentSpeed = ai.maxSpeed;
        //ai.maxSpeed = link.crossLinkSpeed; 
        float duration = link.crossLinkSpeed > 0 ? Vector3.Distance(linkInfo.second.position, linkInfo.first.position) / link.crossLinkSpeed : 1;
        float startTime = Time.time;
        //TODO
        bool isSuccess = link.successRate==1;
        if (isSuccess==false)
        {
            float rate = Random.Range(0, 1.0f);
            if (rate <= link.successRate)
            {
                isSuccess = true;
            }
        }
        //
        
        while (true)
        {
            var pos = Vector3.Lerp(linkInfo.first.position, linkInfo.second.position, Mathf.InverseLerp(startTime, startTime + duration, Time.time));
            if (ai.updatePosition) ai.transform.position = pos;
            else ai.simulatedPosition = pos;

            if (Time.time >= startTime + duration) break;
            yield return null;
        }
        yield return 0;
    }
}
