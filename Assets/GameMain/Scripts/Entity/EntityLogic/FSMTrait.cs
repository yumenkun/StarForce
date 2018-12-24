using GameFramework.Fsm;

namespace StarForce
{
    public class FSMTrait : TraitBase
    {
        public FSMTrait(Entity entity) : base(entity)
        {
            GameEntry.Fsm.CreateFsm<Entity>(entity, GetStates());
        }

        public FsmState<Entity>[] GetStates()
        {
//            if(actor.)
            return new FsmState<Entity>[] {new MoveState() };
        }
    }
}