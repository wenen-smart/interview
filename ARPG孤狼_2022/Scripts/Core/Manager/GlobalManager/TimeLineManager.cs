using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Playables;

public class TimeLineManager:InstanceMono<TimeLineManager>
    {
    private bool timeLineIsPlay;
    private PlayableDirector EndKillTimeLine;
    public bool TimeLineIsPlay { get => EndKillTimeLine.state==PlayState.Playing; set => timeLineIsPlay = value; }
    public override void Awake()
    {
        EndKillTimeLine = EndSkillTimeLineManager.Instance.director;
        base.Awake();
    }

}
