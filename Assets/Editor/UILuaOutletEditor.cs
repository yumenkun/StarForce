using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using KSFramework;
using UnityEditorInternal;
using UnityGameFramework.Editor;

[InitializeOnLoad]
[CustomEditor (typeof(UILuaOutlet))]
public class UILuaOutletEditor : Editor
{
	/// <summary>
	/// mark all Game objects that has Outlet, for GUIhierarchyWindow mark
	/// </summary>
	static Dictionary<GameObject,string> _outletObjects = new Dictionary<GameObject,string> ();

    private ReorderableList subRootList;

    private string findName = "";

    static UILuaOutletEditor ()
	{
		EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
		UIWindowAssetEditor.CustomInspectorGUIAfter += (KEngine.UI.UIWindowAsset target) => {
			if (target.gameObject.GetComponent<UILuaOutlet> () == null) {
				if (GUILayout.Button ("Add UILuaOutlet")) {
					target.gameObject.AddComponent<UILuaOutlet> ();
				}
			}
		};
	}

	private static void HierarchyItemCB (int instanceid, Rect selectionrect)
	{
		var obj = EditorUtility.InstanceIDToObject (instanceid) as GameObject;
		if (obj != null) {
			if (_outletObjects.ContainsKey (obj)) {
				Rect r = new Rect (selectionrect);
				r.x = r.width - 80;
				r.width = 80;
				var style = new GUIStyle ();
				style.normal.textColor = Color.green;
				style.hover.textColor = Color.cyan;
				GUI.Label (r, string.Format ("=>'{0}'", _outletObjects [obj]), style);
			}
		}
	}

	GUIStyle GreenFont;
	GUIStyle RedFont;

	private HashSet<string> _cachedPropertyNames = new HashSet<string> ();

	void OnEnable ()
	{
		GreenFont = new GUIStyle ();
		GreenFont.fontStyle = FontStyle.Bold;
		GreenFont.fontSize = 11;
		GreenFont.normal.textColor = Color.green;
		RedFont = new GUIStyle ();
		RedFont.fontStyle = FontStyle.Bold;
		RedFont.fontSize = 11;
		RedFont.normal.textColor = Color.red;
        subRootList = new ReorderableList(serializedObject,
       serializedObject.FindProperty("subRootGoList"),
       true, true, true, true);
        subRootList.drawElementCallback = DrawRootElement;
	}

    private void DrawRootElement(Rect rect, int index, bool selected, bool focused)
    {
        SerializedProperty itemData = subRootList.serializedProperty.GetArrayElementAtIndex(index);
   
        rect.y += 2;
        rect.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, itemData, GUIContent.none);
    }

    private void operateOutlet(int type)
    {
        bool isFinish = false;
        if (!string.IsNullOrEmpty(findName))
        {
            var outlet = target as UILuaOutlet;
            for (var j = outlet.OutletInfos.Count - 1; j >= 0; j--)
            {
                var outletInfo = outlet.OutletInfos[j];
                if (outletInfo.Name == findName)
                {
                    if (type == 1)
                    {
                        Selection.activeObject = outletInfo.Object;
//                        EditorGUI.FocusTextInControl("Find");//FocusControl("Find");
                    }
                    else if (type == 2)
                    {
                        Undo.RecordObject(target, "Remove OutletInfo");
                        outlet.OutletInfos.RemoveAt(j);
                    }
                    isFinish = true;
                    Debug.Log(string.Format("====处理成功 名字:{0}, 类型:{1} ", findName, type));
                    break;
                }
            }
        }
        if (!isFinish)
        {
            Debug.LogError(string.Format("====处理失败 名字:{0}, 类型:{1} ", findName, type));    
        }
    }


    public override void OnInspectorGUI ()
	{
		_cachedPropertyNames.Clear ();

		EditorGUI.BeginChangeCheck ();

        UILuaOutlet outlet = target as UILuaOutlet;
        outlet.FillByObjectName = EditorGUILayout.Toggle("根据Object的名字自动填充Name", outlet.FillByObjectName);
        EditorGUILayout.LabelField("总数量="+outlet.OutletInfos.Count);
        findName = EditorGUILayout.TextField("控件名字:",findName);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("查找"))
        {
            operateOutlet(1);
        }
        if (GUILayout.Button("删除"))
        {
            operateOutlet(2);
        }
        EditorGUILayout.EndHorizontal();
        serializedObject.Update();
        subRootList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
	    if (GUILayout.Button("测试自动"))
	    {
            AutoGenerateUICode.ExportCode(outlet.gameObject.name, outlet.OutletInfos);
	    }
        if (GUILayout.Button("根据父节点列表自动绑定"))
	    {
	        Dictionary<GameObject, List<UILuaOutlet.OutletInfo>> Dic =
	            new Dictionary<GameObject, List<UILuaOutlet.OutletInfo>>();

            for (var j = outlet.OutletInfos.Count - 1; j >= 0; j--)
	        {
	            UILuaOutlet.OutletInfo outletInfo = outlet.OutletInfos[j];
	            GameObject skin = null;
	  
	            skin = outletInfo.Object as GameObject;
	            if (skin == null)
	            {
	                var tr = outletInfo.Object as RectTransform;
	                if (tr)
	                {
	                    skin = tr.gameObject;
	                }
	            }
                if (skin == null)
                {
                    var com = outletInfo.Object as Component;
                    if (com)
                    {
                        skin = com.gameObject;
                    }
                }
	            if (skin == null)
	                continue;
                GameObject temp = skin;
	            while (temp.transform.parent)
	            {
	                temp = temp.transform.parent.gameObject;
	                bool hasFind = false;
	                for (int i = 0; i < outlet.subRootGoList.Count; i++)
	                {
	                    GameObject root = outlet.subRootGoList[i];
	                    if (root)
	                    {
	                        if (temp == root)
	                        {
	                            if (Dic.ContainsKey(root) == false)
	                            {
	                                Dic[root] = new List<UILuaOutlet.OutletInfo>();
	                            }
	                            Dic[root].Add(outletInfo);
//	                            outlet.OutletInfos[j] = null;
	                            hasFind = true;
	                            break;
	                        }
	                    }
	                }
	                if (hasFind)
	                {
	                    break;
	                }
	            }
	        }

            for (int i = 0; i < outlet.subRootGoList.Count; i++)
	        {
                GameObject root = outlet.subRootGoList[i];
	            UILuaOutlet subOutlet = root.GetComponent<UILuaOutlet>();
	            if (subOutlet == null)
	            {
	                subOutlet = root.AddComponent<UILuaOutlet>();
	            }
 
	            if (Dic.ContainsKey(root))
	            {
	                var list = Dic[root];          
                    subOutlet.OutletInfos = list;
	                for (int j = 0; j < list.Count; j++)
	                {
	                    var item = list[j];
	                    for (var k = outlet.OutletInfos.Count - 1; k >= 0; k--)
	                    {
	                        if (outlet.OutletInfos[k].Object == item.Object)
	                        {
                                outlet.OutletInfos.RemoveAt(k);
                                break;
                            }
                        }
                    }
	            }        
            }
	    }

	    if (outlet.OutletInfos == null || outlet.OutletInfos.Count == 0) {
			if (GUILayout.Button ("Add New Outlet")) {
				if (outlet.OutletInfos == null)
					outlet.OutletInfos = new List<UILuaOutlet.OutletInfo> ();

				Undo.RecordObject (target, "Add OutletInfo");
				outlet.OutletInfos.Add (new UILuaOutlet.OutletInfo ());
			}
		} else {


			// outlet ui edit

			for (var j = outlet.OutletInfos.Count - 1; j >= 0; j--) {
				var currentTypeIndex = -1;
				var outletInfo = outlet.OutletInfos [j];
				string[] typesOptions = new string[0];

				var isValid = outletInfo.Object != null && !_cachedPropertyNames.Contains (outletInfo.Name);
				// check duplicate property name
				_cachedPropertyNames.Add (outletInfo.Name);

				if (outletInfo.Object != null) {
					if (outletInfo.Object is GameObject) {


						currentTypeIndex = 0;// give it default
						var gameObj = outletInfo.Object as GameObject;
                        Component[] components = gameObj.GetComponents<Component> ();
//                        //TODO 把Transform组件增加进去
//                        var count = components.Length + 1;
//                        components[count - 1] = gameObj.transform;


                        _outletObjects [gameObj] = outletInfo.Name;
                        
						typesOptions = new string[components.Length + 1];
					    typesOptions[components.Length] = "UnityEngine.GameObject";        
                        for (var i = 0; i < components.Length; i++) {
							var com = components [i];
                            if (com)
                            {
                                var typeName = typesOptions[i] = com.GetType().FullName;
                                if (typeName == outletInfo.ComponentType)
                                {
                                    currentTypeIndex = i;
                                }
                                else if (outletInfo.ComponentType == "UnityEngine.GameObject")
                                {
                                    currentTypeIndex = components.Length;
                                }
                            }
                        }
                    }
				}


				EditorGUILayout.Separator ();
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (string.Format ("Lua Property: '{0}'", outletInfo.Name), isValid ? GreenFont : RedFont);
				EditorGUILayout.Space ();
				if (GUILayout.Button ("+")) {
					Undo.RecordObject (target, "Insert OutletInfo");
					outlet.OutletInfos.Insert (j, new UILuaOutlet.OutletInfo ());
				}
				if (GUILayout.Button ("-")) {

					Undo.RecordObject (target, "Remove OutletInfo");
					outlet.OutletInfos.RemoveAt (j);
				}
				EditorGUILayout.EndHorizontal ();

//                if (outletInfo.Name == findName)
//                {
//                    GUI.SetNextControlName("Find");
//                }

                outletInfo.Name = EditorGUILayout.TextField ("Name:", outletInfo.Name);
				outletInfo.Object = EditorGUILayout.ObjectField ("Object:", outletInfo.Object, typeof(UnityEngine.Object), true);
                //在开发期可以考虑自动填充Name,不会修改原已赋值的name
			    if (outlet.FillByObjectName && outletInfo.Object != null && string.IsNullOrEmpty(outletInfo.Name))
			    {
			        outletInfo.Name = outletInfo.Object.name;
			    }
			    if (currentTypeIndex >= 0) {
					var typeIndex = EditorGUILayout.Popup ("Component:", currentTypeIndex, typesOptions);
					outletInfo.ComponentType = typesOptions [typeIndex].ToString ();

				}
			}
		}
		//base.OnInspectorGUI ();
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (target, "GUI Change Check");
		}
	}


}
