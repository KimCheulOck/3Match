using UnityEditor;

[CustomEditor(typeof(Tween))]
public class TweenInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawPlayStyle();
        DrawCurve();
        DrawDuration();
        DrawStartTime();
        DrawTweenGroup();
        DrawEnable();
        DrawReverse();
        DrawPause();
        DrawFinishEvent();
    }

    private void DrawPlayStyle()
    {
        var playStyle = serializedObject.FindProperty("playStyle");
        TweenPlayStyle tweenPlayStyle = (TweenPlayStyle)EditorGUILayout.EnumPopup("playStyle", 
            (TweenPlayStyle)playStyle.enumValueIndex);

        playStyle.enumValueIndex = (int)tweenPlayStyle;
    }

    private void DrawCurve()
    {
        var curve = serializedObject.FindProperty("curve");
        curve.animationCurveValue = EditorGUILayout.CurveField("curve", curve.animationCurveValue);
    }

    private void DrawDuration()
    {
        var duration = serializedObject.FindProperty("duration");
        duration.floatValue = EditorGUILayout.FloatField("duration", duration.floatValue);
    }

    private void DrawStartTime()
    {
        var startTime = serializedObject.FindProperty("startTime");
        startTime.floatValue = EditorGUILayout.FloatField("startTime", startTime.floatValue);
    }

    private void DrawTweenGroup()
    {
        var tweenGroup = serializedObject.FindProperty("tweenGroup");
        tweenGroup.intValue = EditorGUILayout.IntField("tweenGroup", tweenGroup.intValue);
    }

    private void DrawEnable()
    {
        var isEnable = serializedObject.FindProperty("isEnable");
        isEnable.boolValue = EditorGUILayout.Toggle("isEnable", isEnable.boolValue);
    }

    private void DrawReverse()
    {
        var isReverse = serializedObject.FindProperty("isReverse");
        isReverse.boolValue = EditorGUILayout.Toggle("isReverse", isReverse.boolValue);
    }

    private void DrawPause()
    {
        var isPause = serializedObject.FindProperty("isPause");
        isPause.boolValue = EditorGUILayout.Toggle("isPause", isPause.boolValue);
    }

    private void DrawFinishEvent()
    {
        SerializedProperty finishEvnet = serializedObject.FindProperty("finishEvnet");
        EditorGUILayout.PropertyField(finishEvnet);
    }
}
