using UnityEngine;


public class SoundCueBehavior : StateMachineBehaviour
{
    [Tooltip("Game Object to Store the Audio Source Component. This allows Animation States to share the same AudioSource")]
    public string m_source = "Animator Cue";

    public AudioClipGroup clipGroup;
    public AudioSourceConfig soureConfig;

    public bool playOnEnter = true;
    //public bool Loop = false;
    public bool stopOnExit;
    [Hide("playOnEnter", true, true)]
    public bool playOnTime;
    [Hide("playOnEnter", true, true)]
    [Range(0, 1)]
    public float NormalizedTime = 0.5f;
    [Space]
    [MinMaxRange(-3, 3)]
    public RangedFloat pitch = new RangedFloat(1, 1);
    [MinMaxRange(0, 1)]
    public RangedFloat volume = new RangedFloat(1, 1);

    private AudioClipCue ClipCue;
    private AudioSource _audio;
    private Transform ClipCueTransform;
    private float lastTimePlaySound;

    private void CheckAudioSource(Animator animator)
    {
        if (ClipCueTransform == null)
        {
            var goName = m_source;

            if (string.IsNullOrEmpty(goName)) goName = "Animator Cue";

            ClipCueTransform = animator.transform.DeepFind(goName);
                        
            if (!ClipCueTransform)
            {
                var go = new GameObject() { name = goName };
                ClipCueTransform = go.transform;
                ClipCueTransform.parent = animator.transform;
            }

            ClipCue = ClipCueTransform.GetComponent<AudioClipCue>();

            if (!ClipCue)
            {
                ClipCue = ClipCueTransform.gameObject.AddComponent<AudioClipCue>();
            }
        }
    }



    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CheckAudioSource(animator);

        if (playOnEnter)
        {
            PlaySound();
            playOnTime = false; //IMPORTANT
        }
        else playOnTime = true;
    }



    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnTime)
        {
            var cur = Mathf.Repeat(stateInfo.normalizedTime, 1);
            if (cur > NormalizedTime && !ClipCue.IsPlay && !animator.IsInTransition(layerIndex))
            {
                PlaySound();
                playOnTime = false;
                lastTimePlaySound = cur;
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stopOnExit && animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash != stateInfo.fullPathHash) //dont stop the current animation if is this same animation
            ClipCue?.PauseAudio();
    }

    public virtual void PlaySound()
    {
        ClipCue.SourceConfig = soureConfig;
        ClipCue.ClipConfig = clipGroup;
        ClipCue.Play();
    }
}
