using UnityEditor;

[CustomEditor(typeof(TweenAlpha))]
public class TweenAlphaInspector : TweenInspector
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawFrom();
        DrawTo();

        base.OnInspectorGUI();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawFrom()
    {
        var from = serializedObject.FindProperty("from");
        EditorGUILayout.Slider(from, 0, 1.0f);
    }

    private void DrawTo()
    {
        var to = serializedObject.FindProperty("to");
        EditorGUILayout.Slider(to, 0, 1.0f);
    }
}
