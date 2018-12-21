using System;
using System.Collections.Generic;
using KSFramework;

namespace UnityGameFramework.Runtime
{
    public abstract class UIWidget
    {
        public abstract void SetPropValue(List<UILuaOutlet.OutletInfo> infos);
    }
}
