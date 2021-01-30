//=====================================================
// - FileName:      SkillTriggerScriptEditor、.cs
// - CreateTime:    #CreateTime#
// - Description:   脚本描述 
//======================================================
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class SkillTriggerScriptEditor : EditorWindow
{
    private static SkillTriggerScriptEditor instance = null;
    public enum TriigerType
    {
        None,
        PlayAnimation,
        SingleDamage
    }
    private static List<string>[] paramTitles = new List<string>[]
    {
        new List<string>(),
        new List<string>() {"StartTime:", "Animation ID:"},
        new List<string>()
        {
            "StartTime:", "ConstPhyDmg:", "PercentPhyDmg:", "ConstSprDmg:", "PercentSprDmg:",
        "ConstRealDmg:", "PercentRealDmg:", "CertainHit", "CertainCrit"
        }
    };

    private TriigerType _triigerType = TriigerType.None;
    private float[] _params;

    public static void ShowSkillTriggerScriptWindow(TriigerType triigerType, Rect pos)
    {
        if (instance == null)
        {
            instance = new SkillTriggerScriptEditor();
        }

        instance._triigerType = triigerType;
        instance._params = new float[paramTitles[(int)triigerType].Count];

        float width = 300;
        float height = 20 * (paramTitles[(int)triigerType].Count + 1);
        float left = pos.center.x - width / 2;
        float top = pos.center.y - height / 2;
        instance.position = new Rect(left, top, width, height);
        instance.ShowPopup();
    }

    void OnDestroy()
    {
        instance = null;
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();

        for (int i = 0; i < paramTitles[(int)_triigerType].Count; i++)
        {
            _params[i] = EditorGUILayout.FloatField(paramTitles[(int)_triigerType][i], _params[i]);
        }

        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        GUI.color = Color.green;
        if (GUILayout.Button("Add"))
        {
            SkillScriptEditor.AddContent(ConnectParams());
            this.Close();
        }

        GUI.color = Color.red;

        if (GUILayout.Button("Cancel"))
        {
            this.Close();
        }

        GUI.color = Color.white;
        GUILayout.EndHorizontal();
    }

    string ConnectParams()
    {
        string ret = String.Empty;
        switch (_triigerType)
        {
            case TriigerType.PlayAnimation:
                ret += "PlayAnimation";
                break;
            case TriigerType.SingleDamage:
                ret += "SingleDamage";
                break;
            default:
                return String.Empty;
        }
        ret += "(";
        for (int i = 0; i < _params.Length; i++)
        {
            string str = _params[i].ToString();
            ret += str;
            if (i != _params.Length - 1)
                ret += ", ";
        }
        ret += ");";
        return ret;
    }
}
