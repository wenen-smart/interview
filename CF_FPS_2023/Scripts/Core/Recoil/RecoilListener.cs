using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilListener : MonoBehaviour
{
    public Transform recoilImpactX;
    public Transform recoilImpactY;
    public Vector2 recoilEffect;
    public float recoilSpeed = 1.5f;
    public float recoilRecoverSpeed = 5;
    public Vector2 recoilLimit;
    public bool VerticalRecoil = true;
    public bool HorizontalRecoil = true;
    [HideInInspector]public int shot_Num;
    
    public void Update()
    {
        if (recoilEffect==Vector2.zero)
        {
            return;
        }
        recoilEffect.x = Mathf.MoveTowards(recoilEffect.x,0,recoilRecoverSpeed*Time.deltaTime);
        recoilEffect.y = Mathf.MoveTowards(recoilEffect.y,0,recoilRecoverSpeed*Time.deltaTime);
        if (recoilEffect.magnitude < 0.001f)
        {
            recoilEffect = Vector2.zero;
        }
        recoilImpactX.transform.localEulerAngles=recoilImpactX.transform.localEulerAngles.SetY(recoilEffect.x);
        recoilImpactY.transform.localEulerAngles=recoilImpactY.transform.localEulerAngles.SetX(recoilEffect.y);
    }
    public void ReceiveRecoil(RecoilData recoilData)
    {
        if (VerticalRecoil)
        {
            recoilEffect.y -= recoilSpeed * recoilData.GetRecoilYMult(shot_Num);
            recoilEffect.y = -Mathf.Clamp(-recoilEffect.y,0,Mathf.Abs(recoilLimit.y));//垂直方向只有向上的后坐力
        }
        if (HorizontalRecoil)
        {
            //根据 传递顺序产生recoil信号->接收recoil信号->最后player更新shotNum记录。所以每次开始shot_num为0，符合情况。
            recoilEffect.x += recoilData.Get(shot_Num)*recoilSpeed;
            Debug.Log("recoilDataDir:"+recoilData.Get(shot_Num));
            recoilEffect.x = Mathf.Clamp(recoilEffect.x,-recoilLimit.x,recoilLimit.x);
        }
        
    }
}
