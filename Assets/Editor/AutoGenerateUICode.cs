//------------------------------------------------------------
// This file write for Game Framework v3.x
// Which Copyright © 2013-2018 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
// The code write by Ron Tang.
//------------------------------------------------------------

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using KSFramework;

namespace UnityGameFramework.Editor
{
    /// <summary>
    /// 生成数据表行代码
    /// </summary>
    internal static class AutoGenerateUICode
    {
        private static string tablePath = "\\GameMain\\DataTables\\";
        private static string codePath = "\\GameMain\\";
        private static string codeSpace = "StarForce";
//        [MenuItem("Game Framework/AutoGenerateCode", false, 100)]
        private static void HandleAllDataTables()
        {

            //设置进度条  
            //EditorUtility.DisplayProgressBar("设置AssetName名称", "正在设置AssetName名称中...", 0.50f);
            //EditorUtility.ClearProgressBar();

            //路径  
            string fullPath = Application.dataPath + tablePath;

            //获取指定路径下面的所有资源文件  
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo direction = new DirectoryInfo(fullPath);
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

                Debug.Log(files.Length);

                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta"))
                    {
                        continue;
                    }
                    //Debug.Log("Name:" + files[i].Name);
                    //Debug.Log( "FullName:" + files[i].FullName );  
                    //Debug.Log( "DirectoryName:" + files[i].DirectoryName );  
                    LoadFile(files[i].FullName);
                }
            }
        }


        static void LoadFile(string filePath)
        {
            using (StreamReader sr = File.OpenText(filePath))
            {
                string line;
                List<string> lines = new List<string>();
                int lineCount = 0;
                while ((line = sr.ReadLine()) != null && lineCount < 3)
                {
                    //Debug.Log(line);
                    lines.Add(line);
                    lineCount++;
                }
                sr.Close();
                sr.Dispose();
//                HandleData(lines);
            }
        }

        public static void ExportCode(string name, List<UILuaOutlet.OutletInfo> outletList)
        {
            HandleData(name ,outletList);
            AssetDatabase.Refresh();
        }

        static void HandleData(string name, List<UILuaOutlet.OutletInfo> infos)
        {
//            string[] textOfTableNames = info[0].Split('\t');
            string textOfTableName = name;

//            string[] textOfPropertyNames = info[1].Split('\t');

//            string[] textOfPropertyTypeNames = info[2].Split('\t');


            StreamWriter sw;
            FileInfo t = new FileInfo(Application.dataPath + codePath + textOfTableName + "Widget.cs");
            sw = t.CreateText();

            WriteHeader(sw);
            WriteNameSpcace(sw, codeSpace);
            sw.WriteLine("{");
            sw.WriteLine(string.Format("public class {0}", textOfTableName + "Widget : UIWidget")); //
            sw.WriteLine("{");
            WriteAllProperty(sw, infos);
            //            WriteParseDataRow(sw, textOfPropertyNames, textOfPropertyTypeNames);
            //            WriteAvoidJIT(sw, textOfTableName);
            WriteSetPropValue(sw, infos);
            sw.WriteLine("}");
            sw.WriteLine("}");
            sw.Flush();
            sw.Close();
            sw.Dispose();

        }

        private static void WriteSetPropValue(StreamWriter sw, List<UILuaOutlet.OutletInfo> infos)
        {
            sw.WriteLine("    public override void SetPropValue(List<UILuaOutlet.OutletInfo> infos){");
            sw.WriteLine("        foreach (UILuaOutlet.OutletInfo element in infos){");
            for (int i = 0; i < infos.Count; i++)
            {
                var element = infos[i];
                if (element != null && !string.IsNullOrEmpty(element.Name))
                {
                    sw.WriteLine(string.Format("            if (element.Name == \"{0}\"){{", element.Name));
                    sw.WriteLine(string.Format("                {0} = element.Object as {1};", element.Name, element.ComponentType));
                    sw.WriteLine("            }");
                }
            }
            sw.WriteLine("        }");
            sw.WriteLine("    }");
        }

        static void WriteNameSpcace(StreamWriter sw, string name)
        {
            sw.WriteLine("namespace " + name);
        }

        static void WriteAllProperty(StreamWriter sw, List<UILuaOutlet.OutletInfo> infos)
        {
            for (int i = 0; i < infos.Count; i++)
            {
                var info = infos[i];
                if (string.IsNullOrEmpty(info.ComponentType))
                    continue;
                if (string.IsNullOrEmpty(info.Name))
                    continue;
                WriteProperty(sw, info.ComponentType, info.Name);

            }
        }

        static void WriteHeader(StreamWriter sw)
        {
            //            sw.WriteLine("using GameFramework.DataTable;");
            sw.WriteLine("using System.Collections.Generic;");
            sw.WriteLine("using KSFramework;");
            sw.WriteLine("using UnityGameFramework.Runtime;");
        }



        static void WriteProperty(StreamWriter sw, string type, string name)
        {
            sw.WriteLine("  public" + " " + type + " " + name);
            sw.WriteLine("  {");
            sw.WriteLine("    get;");
            sw.WriteLine("    protected set;");
            sw.WriteLine("  }");
            sw.WriteLine("");
        }

        static void WriteAvoidJIT(StreamWriter sw, string classTypeName)
        {
            sw.WriteLine("  private void AvoidJIT()");
            sw.WriteLine("  {");
            sw.WriteLine("    " + string.Format("new Dictionary<int, {0} > ();", classTypeName));
            sw.WriteLine("  }");

        }

        static void WriteParseDataRow(StreamWriter sw, string[] names, string[] types)
        {
            sw.WriteLine("  public void ParseDataRow(string dataRowText)");
            sw.WriteLine("  {");
            sw.WriteLine("    string[] text = dataRowText.Split('\\t');");
            sw.WriteLine("    int index = 0;");
            sw.WriteLine("    index++;");
            sw.WriteLine(string.Format("    {0} = {1}.Parse(text[index++]);", names[1], types[1]));
            sw.WriteLine("    index++;");
            for (int i = 2; i < names.Length; i++)
            {
                if (string.IsNullOrEmpty(names[i])) continue;

                if (types[i] != "string")
                    sw.WriteLine(string.Format("    {0} = {1}.Parse(text[index++]);", names[i], types[i]));
                else
                    sw.WriteLine(string.Format("    {0} = text[index++];", names[i]));
            }

            sw.WriteLine("  }");

        }


    }
}
