namespace StarForce
{
    public class BodyTrait:TraitBase
    {
        public BodyTrait(Entity entity) : base(entity)
        {
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }
    }
}