using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Line))]
public class LineInspector : Editor {
    private void OnSceneGUI()
    {
        Line line = target as Line;
        Transform handleTransform = line.transform;
        Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;
        Vector3 start = handleTransform.TransformPoint(line.start);
        Vector3 end = handleTransform.TransformPoint(line.end);

        Handles.color = Color.white;
        Handles.DrawLine(start, end);
        EditorGUI.BeginChangeCheck();
        start = Handles.DoPositionHandle(start, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move Point");
            EditorUtility.SetDirty(line);
            line.start = handleTransform.InverseTransformPoint(start);
        }
        EditorGUI.BeginChangeCheck();
        end = Handles.DoPositionHandle(end, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move Point");
            EditorUtility.SetDirty(line);
            line.end = handleTransform.InverseTransformPoint(end);
        }
        

    }
}
