using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBarEntity : MonoBehaviour
{
    [SerializeField]
    private UILabel _nameLabel;
    [SerializeField]
    private UIProgressBar _UISlider;
    private Transform followTarget;
    public void SetFollowHead(Transform trans)
    {
        followTarget=trans;
    }
    public void Update()
    {
        if (followTarget)
        {
            transform.position = CameraMgr.WorldPointToNGUIWorldPoint(followTarget.position+Vector3.up*0.5f);
        }
    }

    public void SetInfo(string entityName,IDamageable damageable)
    {
        _nameLabel.text = entityName;
        UpdateHpBar(damageable);
    }
    public void UpdateHpBar(IDamageable damageable)
    {
        _UISlider.value = damageable.hp*1.0f / damageable.MaxHP;
    }
}
