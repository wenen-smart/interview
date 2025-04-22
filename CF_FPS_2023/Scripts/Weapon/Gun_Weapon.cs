using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Resolution.Scripts.Weapon
{
    public class Gun_Weapon : BaseWeapon
    {
        public GunDataConfig GunData;
        public override int baseDamage => GunData.baseDamage;
        public override WeaponType weaponType => GunData.weaponType;
        public float intervalTimeSingleFire => GunData.useIntervalTime;
        public bool isCanContinueShoot => GunData.isCanContinueShoot;
        public int clipCount => GunData.clipCount;
        public int bulletCountInEachClip => GunData.bulletCountInEachClip;
        public float spearFactor => GunData.spearFactor;//散发
        public float spearMax => GunData.spearMax;
        [HideInInspector] public RecoilData recoil => GunData.recoilData;//后坐力
        public float shootDistance => GunData.shootDistance;
        public float bulletRadius => GunData.bulletRadius;


        //public float rechargeTime = 2;
        private MyTimer intervalTimer_SingleFire;
        public Transform gunpoint;
        private RecoilSource recoilSource;
        private int hitScansMask;
        public bool fireIsCDFinish =>intervalTimer_SingleFire.timerState!=MyTimer.TimerState.Run;

        public Action OnFireEventHandler;
        public Action FireCDFinishEventhandler;
        public Action OnRechargeEventHandler;
        public Action OnRechargeFinishEventHandler;
        public Action OnJustEmptyAmmoHandler;
        public Action OnPullBoltEventHandler;
        public bool JustEmptyAmmo { get; protected set; }
        private int ammoCount;
        private int remainClipCount;
        public int clipRemainAmmoCount { get; protected set; }
        public bool HaveAmmo { get { return AmmoCount > 0; } }
        public bool CurrentClipHaveAmmo { get { return AmmoCount > 0&&clipRemainAmmoCount>0; } }
        public bool IsRecharging
        {
            get
            {
                //TODO
                return user.isRecharging;
            }
        }
        public bool isPullBoltting
        {
            get
            {
                return user.isPullBoltting||IsRecharging;
            }
        }

        public int AmmoCount { get => ammoCount;protected set => ammoCount = value; }


#if UNITY_EDITOR
        public void OnDrawGizmosSelected()
        {

            if (GunData!=null&&gunpoint&&gunpoint.gameObject.activeInHierarchy)
            {
                Handles.color = new Color(1, 0, 0, 0.4f);
                Handles.DrawSolidArc(gunpoint.position, gunpoint.forward, gunpoint.up, 360, spearMax);
                Handles.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.4f);
                Handles.DrawSolidArc(gunpoint.position, gunpoint.forward, gunpoint.up, 360, spearMax * spearFactor);
                Handles.color = Color.black;
                Handles.DrawLine(gunpoint.position, gunpoint.GetCirclePoint(15, AxisType.Forward,spearMax));
                Handles.DrawLine(gunpoint.position,gunpoint.position+gunpoint.up*spearMax);
            }

        }
        #endif
        public override void Awake()
        {
            base.Awake();
            intervalTimer_SingleFire=TimeSystem.Instance.CreateTimer();
            intervalTimer_SingleFire.OnFinish += () => { FireCDFinishEventhandler?.Invoke(); };
            DebugTool.DrawLine(transform.position, transform.up, 2, Color.red, 10);
            DebugTool.DrawLine(transform.position, transform.GetCirclePoint(15, AxisType.Forward), 2, Color.blue, 10);
            recoilSource = GetComponent<RecoilSource>();
        }
        public override void Equiped(RoleController _user)
        {
            base.Equiped(_user);
            
            //hitScansMask=userIsPlayer ?~(1<<_user.gameObject.layer):-1;
            hitScansMask=~(1<<LayerMask.NameToLayer("FullBody")|(1<<2));//忽略全身碰撞体 因为要检测击中的身体的部位
            DebugTool.DebugPrint(_user.gameObject.layer.ToString());
        }


        public override void UnEqiped()
        {
            base.UnEqiped();
        }
        public bool Shoot()
        {
            if (!HaveAmmo)
            {
                //empty ammo
                //TODO Play SOUND and Animation
                DebugTool.DebugLogger(MyDebugType.Print,"弹药为空...");
                AudioManager.Instance.PlayAudioByClipClue(transform,Const_SoundClipCue.WeaponAction_TryShotEmptyAmmoSoundIdentity,true);
                if (JustEmptyAmmo)
                {
                    OnJustEmptyAmmoHandler?.Invoke();
                    JustEmptyAmmo = false;
                }
                return false;
            }
            else if(CurrentClipHaveAmmo==false&&IsRecharging==false)
            {
                //Recharge
                DebugTool.DebugLogger(MyDebugType.Print,"正在换弹夹...");
                ToRechargeClip();
                return false;
            }
            else if(IsRecharging||isPullBoltting) return false;
            if (intervalTimer_SingleFire.timerState!=MyTimer.TimerState.Run)
            {
                //can fire
                OnFireEventHandler?.Invoke();
                DebugTool.DebugLogger(MyDebugType.Print,"Fire");
                Vector3 firePoint = gunpoint.position;
                float _spearFactor = UnityEngine.Random.Range(0,spearFactor);//散射系数
                Vector3 crossHaircastEnd=user.GetPreScansTargetPoint(shootDistance);//主角:获取屏幕中点投射到世界的点 robot:see target and return target body point
                
                
                if (crossHaircastEnd!=Vector3.zero)//判断是否在射程范围内，如果不在的话，不发射线检测。只做显示效果
                {
                    Vector3 dis = crossHaircastEnd - firePoint;
                    Vector3 scansDir = dis.normalized;
                    DebugTool.DrawLine(firePoint,crossHaircastEnd,Color.red,0.5f);
                    if (_spearFactor > 0.1f)
                    {
                        //散射角度
                        firePoint = gunpoint.GetCirclePoint(UnityEngine.Random.Range(0, 360),AxisType.Forward);
                        Vector3 fireOffsetDir=(firePoint - gunpoint.position);
                        //散射范围
                        firePoint= gunpoint.position+fireOffsetDir*fireOffsetDir.magnitude * spearMax * _spearFactor;
                    }
                    DoHitScan(firePoint,scansDir);
                }
                else
                {
                    DebugTool.DrawLine(firePoint,gunpoint.forward,2,Color.red,0.5f);
                }
                intervalTimer_SingleFire.Go(intervalTimeSingleFire);
                if (userIsPlayer)
                {
                    recoilSource?.PutRecoil(GunData.recoilData);
                    transform.GetComponent<Animator>().CrossFade(WeaponActionType.Shot.ToString(), 0);
                    AudioManager.Instance.PlayAudioByClipClue(transform,Const_SoundClipCue.WeaponAction_ShotSoundIdentity,true);
                }
                else
                {
                    AudioManager.Instance.PlayAudioByClipClue(transform,Const_SoundClipCue.WeaponAction_ShotSoundIdentity,true);
                }
                AmmoConsume();
                return true;   
            }
            return false;
        }
        private void DoHitScan(Vector3 firePoint,Vector3 scansDir,int _iterationCount=0)
        {
            if (Physics.Raycast(firePoint, scansDir,out RaycastHit hitinfo,shootDistance, hitScansMask, QueryTriggerInteraction.Collide))
            {
                AllowScansBodyPart hitter_allowScansBodyPart = null;
                RoleController hitter_roleController = null;
                ActorHealth hitter_health = null;
                //hit
                DebugTool.DebugLogger(MyDebugType.Print, "hit");
                DebugTool.DrawLine(firePoint, hitinfo.point, Color.yellow, 0.6f);
                DebugTool.DrawWireSphere(hitinfo.point, 0.1f, Color.red, 0.6f);
                //ActorSystem Actor= hitinfo.collider.GetComponent<ActorSystem>();
                //if (Actor)
                //{
                //    RoleController roleController =Actor.GetActorComponent<RoleController>();
                //    ActorHealth health = roleController.actorHealth;
                //    health.Damage(10);
                //}
                hitter_allowScansBodyPart = hitinfo.collider.GetComponent<AllowScansBodyPart>();
                if (hitter_allowScansBodyPart)
                {
                    ActorSystem Actor = hitter_allowScansBodyPart.setting.actorSystem;
                    if (Actor)
                    {
                        hitter_roleController = Actor.GetActorComponent<RoleController>();
                        hitter_health = hitter_roleController.actorHealth;
                        if (hitter_roleController.team != user.team && !hitter_health.isDeath)
                        {
                            //get damage
                            BodyPartMainType bodyPartType = BodyPartSetting.GetBodyPartMainType(hitter_allowScansBodyPart.bodyPartType);
                            var damage = GetDamage(hitinfo.point,bodyPartType,_iterationCount);
                            DebugTool.DebugPrint(string.Format("击中{0},造成伤害：{1}", bodyPartType.ToString(), damage));
                            DamageInfo damageInfo = new DamageInfo(damage, user, DamageTypes.Bullet, GunData, scansDir, hitter_allowScansBodyPart.bodyPartType, hitinfo.point, hitinfo.normal, bulletRadius, Time.time);
                            //
                            hitter_health.Damage(damageInfo);
                        }
                    }
                }
                else
                {
                    Hittable hittable = hitinfo.collider.GetComponent<Hittable>();
                    if (hittable)
                    {
                        DamageInfo damageInfo = new DamageInfo(damage: 0, user, damageType: DamageTypes.Bullet, casterWeaponInfo: GunData, castDir: scansDir, damagedBodyPart: 0, hitinfo.point, hitinfo.normal, bulletRadius, Time.time);
                        hittable.Damage(damageInfo);
                    }
                    //ray success to cast object
                    //handler penetration 
                    //Judge depth
                    if (GunData.MaxObjectCountToPenetrate>_iterationCount)
                    {
                        //check thickness 
                        Vector3 backCastOrigin = hitinfo.point + scansDir.normalized * GunData.PenetrationThickness;
                        //检测厚度是否能够满足枪械穿透的厚度
                        //如果origin在collider内部 不会检测到，就是太厚了。
                        if (Physics.Raycast(backCastOrigin, -scansDir, out RaycastHit backCastHitInfo, GunData.PenetrationThickness, hitScansMask, QueryTriggerInteraction.Collide))
                        {
                            //handler accuracy lose 
                            Vector3 nextOrigin = backCastHitInfo.point;
                            Vector3 nextCastDir = scansDir;
                            Vector3 MaxAccuracyLosePerPenetration=GunData.MaxAccuracyLosePerPenetration;
                            nextCastDir += new Vector3(
                                UnityEngine.Random.Range(-MaxAccuracyLosePerPenetration.x,MaxAccuracyLosePerPenetration.x),
                                UnityEngine.Random.Range(-MaxAccuracyLosePerPenetration.y,MaxAccuracyLosePerPenetration.y),
                                UnityEngine.Random.Range(-MaxAccuracyLosePerPenetration.z,MaxAccuracyLosePerPenetration.z)
                                );
                            DoHitScan(nextOrigin,nextCastDir,_iterationCount+1);
                        }
                    }
                }
                InstantiateEffect(hitinfo.point, hitinfo.normal, hitter_allowScansBodyPart != null);
            }
        }

        public int GetDamage(Vector3 hitpoint,BodyPartMainType hitBodyPartType,int penetrationCount)
        {
            float damage = GunData.baseDamage;
            for (int i = 1; i <= penetrationCount; i++)
            {
                damage *= (1 - GunData.PerPenetrationDamageLossPercent);
            }
            var dis = (gunpoint.position - hitpoint).magnitude;
            float damageRate = 1;
            if (GunData.affectCurve.keys.Length>1)
            {
               damageRate = GunData.affectCurve.Evaluate(Mathf.Clamp01(dis/shootDistance));//最近产生的影响越大
            }
            int result = (int)(damage *damageRate* BodyPartSetting.bodyDamageRateConfig.GetRate(hitBodyPartType));
            return result;
        }
        private void InstantiateEffect(Vector3 point,Vector3 normal,bool isActor)
        {
            //GameObject hitEffect = null;
            //if (isActor)
            //{
            //   hitEffect = GameObjectFactory.Instance.PopItem(hitActorEffectPrefab);
            //}
            //else
            //{
            //   hitEffect = GameObjectFactory.Instance.PopItem(hitEffectPrefab);
            //}

            //hitEffect.transform.position = point;
            //hitEffect.transform.forward = normal;
            ////TODO
            //TimeSystem.Instance.AddTimeTask(5, () => { GameObjectFactory.Instance.PushItem(hitEffect); }, PETime.PETimeUnit.Seconds);
            foreach (var emitter in GunData.fireEmitterPrefabs)
            {
                GameObject go = GameObjectFactory.Instance.PopItem(emitter);
                go.transform.position = gunpoint.position;
                go.transform.forward = gunpoint.forward;
                go.SetActive(false);
                go.SetActive(true);
                Knife.Effects.ParticleGroupEmitter pge = go.GetComponent<Knife.Effects.ParticleGroupEmitter>();
                pge.Emit(1);
            }
        }
        public override bool Use(bool isContinueInput,WeaponUseInputType weaponUseInputType=WeaponUseInputType.Default)
        {
            if ((isContinueInput&&!isCanContinueShoot))
            {
                return false;
            }
            return Shoot();
        }
        #region Animation Event Callback
        //Animation callback
        public void OnFireResult()
        {
            
        }
        public void OnUnEquipedResult()
        {

        }
        #endregion
        //弹药消耗
        public void AmmoConsume()
        {
            AmmoCount--;
            if (AmmoCount==0)
            {
                JustEmptyAmmo = true;
            }
            else
            {
                //还有剩余子弹
                // IsNeedPullBolt
                if (GunData.SometimePullBolt==PullBoltSometime.SingleBullet)
                {
                    PullBolt();
                }
            }
            UpdateRemainInClip();
            //TODO Update ui
            DebugTool.DebugPrint(string.Format("剩余弹药：{0}",AmmoCount));
            DebugTool.DebugPrint(string.Format("当前剩余弹夹：{0}  弹夹中剩余量：{1}",remainClipCount,clipRemainAmmoCount));
        }
        public void UpdateRemainInClip()
        {
            int remainCount = AmmoCount % bulletCountInEachClip;
            //remainCount=0 =>full or empty
            if (remainCount == 0)
            {
                int clipFullCount = (remainClipCount * bulletCountInEachClip);
                if (clipFullCount == AmmoCount)
                {
                    //full
                    remainCount = bulletCountInEachClip;
                }
            }
            clipRemainAmmoCount = remainCount;
        }
        public void ToRechargeClip()
        {
            if (HaveAmmo==false)
            {
                return;
            }
            OnRechargeEventHandler?.Invoke();
            Debug.Log("Recharg Start");
            remainClipCount--;
            //换了个新弹夹
            AmmoCount=(remainClipCount * bulletCountInEachClip);
            AudioManager.Instance.PlayAudioByClipClue(transform,Const_SoundClipCue.WeaponAction_RechargeSoundIdentity,true);
            UpdateRemainInClip();
            StartCoroutine(RechargeCoroutine());
            //TODO SOUND and other 
        }
        public IEnumerator RechargeCoroutine()
        {
            //Game Logic--Animation Update 要在这帧结束后更新 故暂停一帧等待动画状态信息刷新
            yield return 0;
            //MyTimer rechargeTimer = TimeSystem.Instance.CreateTimer();
            //rechargeTimer.Go(rechargeTime);
            while (IsRecharging)
            {
                yield return 0;
            }
            OnRechargeFinishEventHandler?.Invoke();
            Debug.Log("Recharge Finish");
        }

        public override void RenewWeapon()
        {
            base.RenewWeapon();
            OnFireEventHandler = null;
            OnRechargeEventHandler = null;
            OnRechargeFinishEventHandler = null;
            OnJustEmptyAmmoHandler = null;
            OnPullBoltEventHandler = null;
            FireCDFinishEventhandler = null;
        }
        public override void FillAllAmmo()
        {
            AmmoCount = clipCount * bulletCountInEachClip;
            remainClipCount = clipCount;
            clipRemainAmmoCount = bulletCountInEachClip;
        }

        public override WeaponDataConfig GetItemDataConfig()
        {
            return GunData;
        }
        public void ImmediateUpdateAmmoData(int newAmmoCount)
        {
            AmmoCount = newAmmoCount;
            remainClipCount = Mathf.CeilToInt(AmmoCount / bulletCountInEachClip);
            UpdateRemainInClip();
        }
        public void PullBolt()
        {
            OnPullBoltEventHandler?.Invoke();
        }
    }
}
