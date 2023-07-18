using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Tools 
{
    [MenuItem("GameObject/Chuzzle/Validate")]
    public static void ValidateLevel(MenuCommand menuCommand)
    {
        var go = (GameObject)menuCommand.context;
        if (go == null) return;
        foreach (var iValidator in go.GetComponentsInChildren<IEditorValidator>(true))
        {
            iValidator.OnEditorValidate(go);
        }
        EditorUtility.SetDirty(go);
        AssetDatabase.SaveAssets();
    }
}
