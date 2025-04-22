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
            recoilEffect.y = -Mathf.Clamp(-recoilEffect.y,0,Mathf.Abs(recoilLimit.y));//��ֱ����ֻ�����ϵĺ�����
        }
        if (HorizontalRecoil)
        {
            //���� ����˳�����recoil�ź�->����recoil�ź�->���player����shotNum��¼������ÿ�ο�ʼshot_numΪ0�����������
            recoilEffect.x += recoilData.Get(shot_Num)*recoilSpeed;
            Debug.Log("recoilDataDir:"+recoilData.Get(shot_Num));
            recoilEffect.x = Mathf.Clamp(recoilEffect.x,-recoilLimit.x,recoilLimit.x);
        }
        
    }
}
