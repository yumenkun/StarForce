using UnityEngine;
using UnityGameFramework.Runtime;

namespace StarForce
{
    public class PlayerEntity:Entity
    {
        private CreatureData model;
        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            model = userData as CreatureData;
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
        }

        protected override void OnHide(object userData)
        {
            base.OnHide(userData);
        }


        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }

    }
}