using StarForce;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedActor : SharedVariable<Entity>
    {
        public static implicit operator SharedActor(Entity value) { return new SharedActor { mValue = value }; }
    }
}