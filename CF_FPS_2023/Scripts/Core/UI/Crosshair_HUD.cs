using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Crosshair_HUD : MonoBehaviour
{
    public UIWidget crosshairEntity;//ctlr zoom
    public UISprite up;
    public UISprite down;
    public UISprite left;
    public UISprite right;
    private CrosshairSetting setting;
    public bool Activation;
    public bool isBreathe = false;
    private int SpaceInBreathe;
    private float _lastBreatheStartTime;
    private IEnumerator renewToDefaultCoroutineObj;
    public void UpdateState()
    {
        if (setting==null||setting.minEdge<=0||setting.maxEdge<=0)
        {
            Activation = false;
        }
        else
        {
            Activation = true;
        }
        if (Activation)
        {
            up.width = setting.minEdge;
            up.height = setting.maxEdge;
            down.width = setting.minEdge;
            down.height = setting.maxEdge;

            left.width = setting.maxEdge;
            left.height = setting.minEdge;
            right.width = setting.maxEdge;
            right.height = setting.minEdge;

            up.color = setting.color;
            down.color = setting.color;
            left.color = setting.color;
            right.color = setting.color;
            SpaceInBreathe = Mathf.RoundToInt(setting.spaceFactorInBreatheState * setting.spaceInDefaultState);
        }
        gameObject.SetActive(Activation);
    }
    public void CalcualteSpaceInDefaultState()
    {

    }
    public void StartBreathe()
    {
        if (setting==null||setting.breatheSpeed <= 0)
        {
            return;
        }
        if (isBreathe==false)
        {
            _lastBreatheStartTime = Time.time;
        }
        isBreathe = true;
    }
    public void StopBreathe()
    {
        isBreathe = false;
        if (renewToDefaultCoroutineObj != null)
        {
            StopCoroutine(renewToDefaultCoroutineObj);
            renewToDefaultCoroutineObj = null;
        }
        renewToDefaultCoroutineObj = RenewToDefault();
        StartCoroutine(renewToDefaultCoroutineObj);
    }
    public void SetCrosshair(CrosshairSetting tmp_setting)
    {
        setting = tmp_setting;
        UpdateState();
    }
    public void CloseCrossHair()
    {
        setting = null;
        UpdateState();
    }

    public void Update()
    {
        if (isBreathe)
        {
            //s=vt
            float duration = (Time.time - _lastBreatheStartTime);
            int increment = Mathf.RoundToInt(setting.breatheSpeed * duration);
            if (setting.spaceInDefaultState + increment<SpaceInBreathe)
            {
                crosshairEntity.width = setting.spaceInDefaultState + increment;
            }
            else
            {
                crosshairEntity.width = SpaceInBreathe;
                StopBreathe();
            }
        }
    }
    public IEnumerator RenewToDefault()
    {
        float timer=0;
        int startWidth = crosshairEntity.width;
        if (startWidth <= setting.spaceInDefaultState)
        {
            yield break;
        }
        float duration =  ( startWidth - setting.spaceInDefaultState)/setting.breatheSpeed;
        
        while (timer<duration)
        {
            timer += Time.deltaTime;
            crosshairEntity.width = startWidth - Mathf.RoundToInt(setting.breatheSpeed * timer);
            yield return 0;
        }
        crosshairEntity.width = setting.spaceInDefaultState;
    }
}
[System.Serializable]
public class CrosshairSetting
{
    public int minEdge = 5;
    public int maxEdge = 25;
    public float spaceFactorInBreatheState = 2;//在呼吸下的间距系数。factor*spaceInDefaultState
    public int spaceInDefaultState = 10;//默认下的间距
    public float breatheSpeed=1;
    public Color color = Color.green;
}