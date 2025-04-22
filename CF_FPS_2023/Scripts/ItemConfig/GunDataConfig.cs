using UnityEngine;

[CreateAssetMenu(fileName = "New_GunSO", menuName = "CreateWeaponSO/CreateGunSO")]
public class GunDataConfig : WeaponDataConfig
{
    public GunType gunType;
    public PullBoltSometime SometimePullBolt;
    public bool isCanContinueShoot;//是否可连射
    public int clipCount;//弹夹个数
    public int bulletCountInEachClip;//每个弹夹子弹数
    public float spearFactor = 0.7f;//散发系数 0~spearFactor
    public float spearMax = 1;//散射值
    public RecoilData recoilData;//后坐力
    public float shootDistance;//射程
    public float bulletRadius;//子弹半径
    //[Range(0,1)]
    //public float PenetrationRate;//穿透率
    public float MaxObjectCountToPenetrate;//最大穿透的物体数
    public float PenetrationThickness;//最大可穿透的厚度
    [Range(0,1)]
    public float PerPenetrationDamageLossPercent;//每次穿透时候伤害的减少率
    public Vector3 MaxAccuracyLosePerPenetration;//每次穿透准度最大偏移值。
}
public enum PullBoltSometime//拉栓
{
    BulletClip,//一个弹夹拉一次  就是换弹夹动作
    SingleBullet,//一发子弹拉一次 额外的动作
}
