using UnityEngine;
using System.Collections;

public class UIWidgetRef : MonoBehaviour {

    public bool isPublic = false;
    public bool isLoadAuto = false;
    public bool shouldCache = false;

    [HideInInspector]
    public string refPath;

    [HideInInspector]
    public string refName;
}
