using System;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace StarForce
{
    public abstract class Entity : EntityLogic
    {
        [SerializeField]
        private EntityData m_EntityData = null;

        private Dictionary<Type, TraitBase> traitDic = new Dictionary<Type, TraitBase>();
        private List<TraitBase> traitList = new List<TraitBase>();

        public T GetTrait<T>() where T : TraitBase
        {
            TraitBase val;
            if (traitDic.TryGetValue(typeof(T), out val))
            {
                return val as T;
            }
            else
            {
                Log.Error("Entity not has " + typeof(T).Name);
                return null;
            }
        }

        public int Id
        {
            get
            {
                return Entity.Id;
            }
        }

        public Animation CachedAnimation
        {
            get;
            private set;
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnInit(object userData)
#else
        protected internal override void OnInit(object userData)
#endif
        {
            base.OnInit(userData);
            CachedAnimation = GetComponent<Animation>();
            EntityData data = userData as EntityData;
            var traits = data.getTrait();
            foreach (Type type in traits)
            {
                TraitBase trait = Activator.CreateInstance(type, this) as TraitBase;
                traitDic.Add(type,  trait);
                traitList.Add(trait);
            }
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnShow(object userData)
#else
        protected internal override void OnShow(object userData)
#endif
        {
            base.OnShow(userData);

            m_EntityData = userData as EntityData;
            if (m_EntityData == null)
            {
                Log.Error("Entity data is invalid.");
                return;
            }

            Name = string.Format("[Entity {0}]", Id.ToString());
            CachedTransform.localPosition = m_EntityData.Position;
            CachedTransform.localRotation = m_EntityData.Rotation;
            CachedTransform.localScale = Vector3.one;
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnHide(object userData)
#else
        protected internal override void OnHide(object userData)
#endif
        {
            base.OnHide(userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData)
#else
        protected internal override void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData)
#endif
        {
            base.OnAttached(childEntity, parentTransform, userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnDetached(EntityLogic childEntity, object userData)
#else
        protected internal override void OnDetached(EntityLogic childEntity, object userData)
#endif
        {
            base.OnDetached(childEntity, userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
#else
        protected internal override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
#endif
        {
            base.OnAttachTo(parentEntity, parentTransform, userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnDetachFrom(EntityLogic parentEntity, object userData)
#else
        protected internal override void OnDetachFrom(EntityLogic parentEntity, object userData)
#endif
        {
            base.OnDetachFrom(parentEntity, userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
#else
        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
#endif
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (traitList != null && traitList.Count > 0)
            {
                for (int i = 0; i < traitList.Count; i++)
                {
                    var trait = traitList[i];
                    if (trait.isEnable)
                    {
                        traitList[i].OnUpdate(elapseSeconds, realElapseSeconds);
                    }
                }
            }
        }
    }
}
