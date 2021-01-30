//=====================================================
// - FileName:      DkSkillWindow.cs
// - CreateTime:    #CreateTime#
// - Description:   脚本描述 
//======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SkillScriptEditor: EditorWindow
{
    [MenuItem("SkillScriptEditor/Open Script Editor")]
    static void OpenSkillScriptEditor()
    {
        SkillScriptEditor window = GetWindow<SkillScriptEditor>(true, "SkillScriptEditor");
        window.Init();
    }

    private int skillId = 1000;
    private int selectContentIndex = -1;
    private Vector2 scrollPos = Vector2.zero;

    private static List<string> scriptContent = new List<string>();

    void Init()
    {
        selectContentIndex = -1;
        scriptContent.Clear();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical("box", GUILayout.Width(400), GUILayout.Height(400));
        #region ScrollView - Script Content

        EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(400), GUILayout.Height(400));
        //GUIStyle style = new GUIStyle(GUI.skin.button);
        for (int i = 0; i < scriptContent.Count; i++)
        {
            int index = i;
            if (selectContentIndex == index)
                GUI.color = Color.green;

            if (GUILayout.Button(scriptContent[index]))
            {
                selectContentIndex = index;
            }
            GUI.color = Color.white;
        }

        EditorGUILayout.EndScrollView();
        #endregion
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("box", GUILayout.Width(200), GUILayout.Height(400));
        #region UI
        skillId = EditorGUILayout.IntField("script ID:", skillId);
        if (GUILayout.Button("Load Script by Script ID"))
            LoadScript();
        if (GUILayout.Button("Save Script"))
            SaveScript();
        if (GUILayout.Button("New Or Clear Script"))
            ClearContent();
        if (GUILayout.Button("Delete Line"))
            DeleteContent();
        if (GUILayout.Button("Move Up"))
            MoveUpContent();
        if (GUILayout.Button("Move Down"))
            MoveDownContent();
        #region Add Trigger
        EditorGUILayout.BeginVertical("box");
        GUI.color = Color.yellow;
        if (GUILayout.Button("PlayAnimationTrigger"))
        {
            SkillTriggerScriptEditor.ShowSkillTriggerScriptWindow(SkillTriggerScriptEditor.TriigerType.PlayAnimation, this.position);
        }

        if (GUILayout.Button("SingleDamageTrigger"))
        {
            SkillTriggerScriptEditor.ShowSkillTriggerScriptWindow(SkillTriggerScriptEditor.TriigerType.SingleDamage, this.position);
        }
        GUI.color = Color.white;
        EditorGUILayout.EndVertical();
        #endregion
        #endregion

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }



    void DeleteContent()
    {
        if (selectContentIndex != -1)
            scriptContent.RemoveAt(selectContentIndex);
        selectContentIndex = -1;
    }

    void ClearContent()
    {
        scriptContent.Clear();
    }

    public static void AddContent(string str)
    {
        scriptContent.Add(str);
    }

    void MoveUpContent()
    {
        if (selectContentIndex != -1 && selectContentIndex > 0)
        {
            string temp = scriptContent[selectContentIndex];
            scriptContent[selectContentIndex] = scriptContent[selectContentIndex - 1];
            scriptContent[selectContentIndex - 1] = temp;
            selectContentIndex -= 1;
        }
    }

    void MoveDownContent()
    {
        if (selectContentIndex != -1 && selectContentIndex < scriptContent.Count - 1)
        {
            string temp = scriptContent[selectContentIndex];
            scriptContent[selectContentIndex] = scriptContent[selectContentIndex + 1];
            scriptContent[selectContentIndex + 1] = temp;
            selectContentIndex += 1;
        }
    }

    void SaveScript()
    {
        string content = String.Empty;
        content += string.Format("skill({0})", skillId);
        content += "\r\n{\r\n";
        foreach (var str in scriptContent)
        {
            content += "\t";
            content += str;
            content += "\r\n";
        }
        content += "}";
        //bool ret = FileWriterProxy.Write(Application.streamingAssetsPath + "/" + "SkillScript" + skillId.ToString() + ".txt", content);
        //Debug.Log(ret);
    }

    void LoadScript()
    {
        ClearContent();
        bool ret = ParseScript(Application.streamingAssetsPath + "/" + "SkillScript" + skillId.ToString() + ".txt");
    }

    private bool ParseScript(string filename)
    {
        bool ret = false;
        StreamReader sr = null;
        try
        {
            //sr = FileReaderProxy.ReadFile(filename);
            if (sr != null)
                ret = LoadScriptFromStream(sr);

        }
        catch (Exception e)
        {
            string err = "Exception:" + e.Message + "\n" + e.StackTrace + "\n";
            //LogSystem.ErrorLog(err);
        }
        if (sr != null)
            sr.Close();
        return ret;
    }

    private bool LoadScriptFromStream(StreamReader sr)
    {
        bool bracket = false;
        do
        {
            string line = sr.ReadLine();

            if (line == null)
                break;

            line = line.Trim();

            if (line.StartsWith("//") || line == "")
                continue;

            if (line.StartsWith("skill"))
            {
                int start = line.IndexOf("(");
                int end = line.IndexOf(")");
                if (start == -1 || end == -1)
                    Debug.LogError(string.Format("ParseScript Error, start == -1 || end == -1  {0}", line));

                int length = end - start - 1;
                if (length <= 0)
                {
                    Debug.LogError(string.Format("ParseScript Error, length <= 1, {0}", line));
                    return false;
                }

                string args = line.Substring(start + 1, length);
                int skillId = (int)Convert.ChangeType(args, typeof(int));
            }
            else if (line.StartsWith("{"))
            {
                bracket = true;
            }
            else if (line.StartsWith("}"))
            {
                bracket = false;
            }
            else
            {
                // 解析trigger
                if (bracket == true)
                {
                    int start = line.IndexOf("(");
                    int end = line.IndexOf(")");
                    if (start == -1 || end == -1)
                        Debug.LogError(string.Format("ParseScript Error, {0}", line));

                    int length = end - start - 1;
                    if (length <= 0)
                    {
                        Debug.LogError(string.Format("ParseScript Error, length <= 1,  {0}", line));
                        return false;
                    }
                    line = line.Replace("\t", "");
                    scriptContent.Add(line);
                }
            }
        } while (true);


        return true;
    }
}
