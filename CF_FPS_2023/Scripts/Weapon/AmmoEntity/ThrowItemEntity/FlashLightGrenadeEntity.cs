using Knife.Effects;
using UnityEngine;

namespace Assets.Resolution.Scripts.Weapon
{
    public class FlashLightGrenadeEntity:ThrowItemAmmoEntity
    {
        protected override void Activate()
        {
            InstantiateEffect();
            int hitScansMask=~(1<<2);
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius,hitScansMask);

            foreach (var c in colliders)
            {
                var actor = c.GetComponent<ActorComponent>();

                if (actor != null)
                {
                    var role = actor.GetActorComponent<RoleController>();
                    //Check whether the effect affects  actor
                    if (Physics.Linecast(transform.position,role.head.position,out RaycastHit hitInfo,hitScansMask,QueryTriggerInteraction.Ignore))
                    {
                        var actorInLine = hitInfo.collider.GetComponent<ActorComponent>();
                        if (actorInLine && actorInLine.actorSystem == actor.actorSystem)
                        {
                            actor.GetActorComponent<RoleController>().ReceiveFlashGrenade();
                        }
                    }
                    
                }
            }
            TimeSystem.Instance.TimerUpdateFinish(LifeTimer);
            GameObjectFactory.Instance.PushItem(gameObject);
        }
    }
}
