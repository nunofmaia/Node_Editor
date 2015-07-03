using UnityEngine;
using UnityEditor;
using System;

public class CreateNode
{
    [MenuItem ("Assets/Create/Flow Node")]
    static void Create()
    {
        Create("NodeTemplate.txt", "NewNode.cs");
    }

    static void Create(string template, string filename)
    {
        string currentPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        string path = currentPath + "/" + filename;

        var DoCreateScriptAsset = Type.GetType("UnityEditor.ProjectWindowCallback.DoCreateScriptAsset, UnityEditor");
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                                                                ScriptableObject.CreateInstance(DoCreateScriptAsset) as UnityEditor.ProjectWindowCallback.EndNameEditAction,
                                                                path,
                                                                null,
                                                                "Assets/Node_Editor/Framework/" + template);
    }
}
