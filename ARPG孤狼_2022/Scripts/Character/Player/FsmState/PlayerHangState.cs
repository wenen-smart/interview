using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class PlayerHangState : IState<PlayerStateType, PlayerTransitionType, PlayerController>
{
        public PlayerHangState(PlayerStateType stateType, FsmStateMachine<PlayerStateType, PlayerTransitionType, PlayerController> fsmStateMachine) : base(stateType, fsmStateMachine)
        {

        }

        public override void Enter(PlayerController go, params object[] args)
        {
            base.Enter(go, args);
        } 
        public override void Excute(PlayerController go, params object[] args)
        {
           
        }

        public override void Exit(PlayerController go, params object[] args)
        {
           base.Exit(go, args);
        }

        public override void OnUpdate(PlayerController go, params object[] args)
        {
            go.HangOnUpdate();

        }
    }

