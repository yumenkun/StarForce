namespace StarForce
{
    public abstract class TraitBase
    {
        public TraitBase(Entity entity)
        {
            actor = entity;
        }

        protected Entity actor
        {
            get;
            private set;
        }

        public bool isEnable = true;

        protected void SetEnable(bool enable)
        {
            if (isEnable != enable)
            {
                isEnable = enable;
            }
        }

        public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            
        }

        protected void Dispose()
        {
            
        }
    }
}