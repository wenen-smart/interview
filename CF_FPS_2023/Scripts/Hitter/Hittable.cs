
using Knife.Effects;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable
{
    public void Damage(DamageInfo damageInfo);
}
public class Hittable:MonoBehaviour,IHittable
{
    public HittableEffectSettingSO HESSO;
    public System.Action<GameObject> attachAsChildHandler =null;
    public List<LifeTimer> allEffects = new List<LifeTimer>();
    public virtual void Damage(DamageInfo damage)
    {
        HittableEffectSetting EffectSetting = null;
        HESSO?.GetHittableEffectSetting(damage.casterWeaponInfo.id,out EffectSetting);
        if (EffectSetting!=null)
        {
            if (/*damage.damageType == DamageTypes.Bullet*/true)
            {
                var point = damage.hitPoint + damage.hitNormal * Random.Range(EffectSetting.offsetMin, EffectSetting.offsetMax);
                if (EffectSetting.decalPrefabs != null && EffectSetting.decalPrefabs.Length > 0)
                {
                    var decalInstance = GameObjectFactory.Instance.PopItem(EffectSetting.decalPrefabs[Random.Range(0, EffectSetting.decalPrefabs.Length)]);

                    var decal = decalInstance.GetComponent<SimpleDecal>();

                    LifeTimer lifeTimer = decalInstance.GetComponent<LifeTimer>();
                    lifeTimer.isAwakeDelayDestory = false;
                    TimeSystem.Instance.AddTimeTask(lifeTimer.lifeTime, () =>
                    {
                        if (allEffects.Contains(lifeTimer))
                        {
                            allEffects.Remove(lifeTimer); GameObjectFactory.Instance.PushItem(decalInstance);
                        }
                    }, PETime.PETimeUnit.Seconds);

                    bool canRotate = decal == null || decal.CanRotate;

                    decalInstance.transform.position = point;
                    decalInstance.transform.rotation = Quaternion.LookRotation(damage.hitNormal);

                    if (EffectSetting.randomRotation && canRotate)
                        decalInstance.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), damage.hitNormal) * decalInstance.transform.rotation;

                    decalInstance.transform.localScale = Vector3.one * EffectSetting.size * damage.ammo_size;

                    if (EffectSetting.autoParent)
                    {
                        if (attachAsChildHandler != null)
                        {
                            attachAsChildHandler.Invoke(decalInstance);
                        }
                        else
                        {
                            decalInstance.transform.SetParent(transform);
                        }
                    }
                    allEffects.Add(lifeTimer);
                }

                if (EffectSetting.impactPrefabs != null && EffectSetting.impactPrefabs.Length > 0)
                {
                    var impactInstance = GameObjectFactory.Instance.PopItem(EffectSetting.impactPrefabs[Random.Range(0, EffectSetting.impactPrefabs.Length)]);
                    impactInstance.transform.position = point;
                    impactInstance.transform.up = damage.hitNormal;

                    LifeTimer lifeTimer = impactInstance.GetComponent<LifeTimer>();
                    lifeTimer.isAwakeDelayDestory = false;
                    TimeSystem.Instance.AddTimeTask(lifeTimer.lifeTime, () =>
                    {

                        if (allEffects.Contains(lifeTimer))
                        {
                            allEffects.Remove(lifeTimer); GameObjectFactory.Instance.PushItem(impactInstance);
                        }
                    }, PETime.PETimeUnit.Seconds);

                    if (EffectSetting.randomRotation)
                        impactInstance.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), damage.hitNormal) * impactInstance.transform.rotation;

                    impactInstance.transform.localScale = Vector3.one * EffectSetting.impactSize * damage.ammo_size;
                    if (EffectSetting.autoParent)
                    {
                        if (attachAsChildHandler != null)
                        {
                            attachAsChildHandler.Invoke(impactInstance);
                        }
                        else
                        {
                            impactInstance.transform.SetParent(transform);
                        }
                    }
                    allEffects.Add(lifeTimer);
                }
            }
        }
        
    }
    public void DestoryAllEffect()
    {
        for (int i = 0; i < allEffects.Count;)
        {
            GameObjectFactory.Instance.PushItem(allEffects[0].gameObject);
            allEffects.RemoveAt(0);
        }
    }
    public  void OnDestory()
    {
        DestoryAllEffect();
    }
}
[System.Serializable]
public class HittableEffectSetting
{
    /// <summary>
    /// Decal prefabs array.
    /// </summary>
    [SerializeField] [Tooltip("Decal prefabs array")] public GameObject[] decalPrefabs;
    /// <summary>
    /// Impact prefabs array.
    /// </summary>
    [SerializeField] [Tooltip("Impact prefabs array")] public GameObject[] impactPrefabs;
    /// <summary>
    /// Minimum random offset from surface to decal position.
    /// </summary>
    [SerializeField] [Tooltip("Minimum random offset from surface to decal position")] public float offsetMin = 0.02f;
    /// <summary>
    /// Maximum random offset from surface to decal position.
    /// </summary>
    [SerializeField] [Tooltip("Maximum random offset from surface to decal position")] public float offsetMax = 0.04f;
    /// <summary>
    /// Size of decals.
    /// </summary>
    [SerializeField] [Tooltip("Size of decals")] public float size = 0.2f;
    /// <summary>
    /// Size of impacts.
    /// </summary>
    [SerializeField] [Tooltip("Size of impacts")] public float impactSize = 1f;
    /// <summary>
    /// If flag is enabled all decals will be attached to surface as child objects.
    /// </summary>
    [SerializeField] [Tooltip("If flag is enabled all decals will be attached to surface as child objects")] public bool autoParent = false;
    /// <summary>
    /// Random rotation of decals along normal vector.
    /// </summary>
    [SerializeField] [Tooltip("Random rotation of decals along normal vector")] public bool randomRotation = true;

    /// <summary>
    /// IHittable.TakeDamage implementation. Spawns decals based on damage data.
    /// </summary>
    /// <param name="damage">array of damages</param>
}

public enum HittableTag
{
    Sand,
    Wood,
    Concrete
}
