using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Resolution.Scripts.Weapon
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public abstract class ThrowItemAmmoEntity : AmmoEntity
    {
        private Rigidbody _rigid;
        private Collider _collider;
        public float LifeTime = 10;
        public MyTimer LifeTimer;
        public MyTimer activateTimer;
        public ThrowItemTriggerType throwItemTriggerType;
        [HideInInspector]public ThrowItemDataConfig throwItemData;
        [SerializeField] [Tooltip("Explode activation timer in seconds")] protected float activateTime = 3f;
        [SerializeField] [Tooltip("Explode particles that will be enabled")] protected GameObject[] triggerEffectPrefabs;
        [SerializeField] [Tooltip("Explosion radius")] protected float radius = 20f;
        private int damage;
        private bool damageIsRangeDecline;
        private bool ignoreAllCollision;
        public bool IgnoreAllCollision
        {
            get => ignoreAllCollision;
            set
            {
                ignoreAllCollision = value;
            }
        }

        public Collider ItemCollider
        {
            get
            {
                if (_collider==null)
                {
                    _collider = GetComponent<Collider>();
                }
                return _collider;
            }
        }

        public void SetDamage(int tmp_damage)
        {
            damage = tmp_damage;
        }
        public void SetDamageDeclineBaseDistance(bool isUseDecline)
        {
            damageIsRangeDecline = isUseDecline;
        }
        private void Awake()
        {
            _rigid = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            LifeTimer = TimeSystem.Instance.CreateTimer();
            activateTimer = TimeSystem.Instance.CreateTimer();
            LifeTimer.OnFinish =()=>{ GameObjectFactory.Instance.PushItem(gameObject); };
            activateTimer.OnFinish = Activate;
        }
        /// <summary>
        /// 准备扔，还没扔。
        /// </summary>
        public void PreThrow(RoleController _user,ThrowItemDataConfig tmp_throwItemData)
        {
            RegisterUser(_user);
            throwItemData=tmp_throwItemData;
            Init();
        }
        public void ThrowOut(Vector3 dir,float force)
        {
            _rigid.useGravity = true;
            _rigid.isKinematic = false;
            ItemCollider.isTrigger = IgnoreAllCollision;
            
            //
            Vector3 normalize = dir.normalized;
            _rigid.AddForce(dir*force,ForceMode.Impulse);
            LifeTimer.Go(LifeTime);
            IgnoreCollision(user.characterCollider);
            if (throwItemTriggerType==ThrowItemTriggerType.Time)
            {
                activateTimer.Go(activateTime);
            }
        }
        public void IgnoreCollision(Collider collider)
        {
            Physics.IgnoreCollision(collider, _collider);
        }
        private void Init()
        {
            //忽略重力 和 碰撞
            _rigid.useGravity = false;
            _rigid.isKinematic = true;
            ItemCollider.isTrigger = true;
        }
        private void OnCollisionEnter(Collision other)
        {
            if (throwItemTriggerType==ThrowItemTriggerType.Collision)
            {
                if (user.IsBodyCollider(other.collider) == false)
                {
                    Activate();
                }
            }
        }

        protected abstract void Activate();
        protected virtual void InstantiateEffect()
        {
            foreach (var effect in triggerEffectPrefabs)
            {
                GameObject go = GameObjectFactory.Instance.PopItem(effect);
                go.transform.position = transform.position;
                LifeTimer lifeTimer = effect.GetComponent<LifeTimer>();
                lifeTimer.isAwakeDelayDestory = false;
                TimeSystem.Instance.AddTimeTask(lifeTimer.lifeTime,()=> { GameObjectFactory.Instance.PushItem(go); },PETime.PETimeUnit.Seconds);
            }
        }
        
        public int CalcuateDamage(Vector3 hitterPos)
        {
            float dis = Vector3.Distance(hitterPos,transform.position);
            float percent = 1 - dis / radius;
            if (percent<0)
            {
                percent = 0;
            }
            int damageResult = (int)(damage * percent);
            return damageResult;
        }
    }
}

public enum ThrowItemTriggerType
{
    Collision,
    Time
}
