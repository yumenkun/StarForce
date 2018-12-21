using System.Collections.Generic;
using KSFramework;
using UnityGameFramework.Runtime;
namespace StarForce
{
public class SettingFormWidget : UIWidget
{
  public UnityEngine.GameObject AAA
  {
    get;
    protected set;
  }

    public override void SetPropValue(List<UILuaOutlet.OutletInfo> infos){
        foreach (UILuaOutlet.OutletInfo element in infos){
            if (element.Name == "AAA"){
                AAA = element.Object as UnityEngine.GameObject;
            }
        }
    }
}
}
