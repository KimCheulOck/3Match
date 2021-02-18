using UnityEditor;

[CustomEditor(typeof(TweenPosition))]
public class TweenPositionInspector : TweenInspector
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawFrom();
        DrawTo();

        DrawLocalFlag();

        base.OnInspectorGUI();


        serializedObject.ApplyModifiedProperties();
    }

    private void DrawFrom()
    {
        var from = serializedObject.FindProperty("from");
        from.vector3Value = EditorGUILayout.Vector3Field("from", from.vector3Value);
    }

    private void DrawTo()
    {
        var to = serializedObject.FindProperty("to");
        to.vector3Value = EditorGUILayout.Vector3Field("to", to.vector3Value);
    }

    private void DrawLocalFlag()
    {
       var isLocalPostion = serializedObject.FindProperty("isLocalPostion");
        isLocalPostion.boolValue = EditorGUILayout.Toggle("isLocalPostion", isLocalPostion.boolValue);
    }
}
