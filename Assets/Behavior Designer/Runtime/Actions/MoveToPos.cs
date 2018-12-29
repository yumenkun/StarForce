using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Move to Pos or Target")]
    [TaskIcon("{SkinColor}WaitIcon.png")]
    public class MoveToPos : Action
    {
        [Tooltip("Target Pos")]
        public SharedVector3 targetPos;
        [Tooltip("Force Move Speed")]
        public SharedFloat forceMoveSpeed;

        public override void OnStart()
        {

        }

        public override TaskStatus OnUpdate()
        {
//            // The task is done waiting if the time waitDuration has elapsed since the task was started.
//            if (startTime + waitDuration < Time.time)
//            {
//                return TaskStatus.Success;
//            }

            return TaskStatus.Running;
        }


        public override void OnReset()
        {
        }
    }
}