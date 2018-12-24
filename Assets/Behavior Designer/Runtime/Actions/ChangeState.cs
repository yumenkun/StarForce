using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Returns a TaskStatus of running. Will only stop when interrupted or a conditional abort is triggered.")]
    [TaskIcon("{SkinColor}IdleIcon.png")]
    public class ChangeState : Action
    {
        
        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override void OnStart()
        {
            base.OnStart();
//            Owner.
//            Owner.GetVariable()
        }

        public override TaskStatus OnUpdate()
        {
            return base.OnUpdate();
        }

    }
}