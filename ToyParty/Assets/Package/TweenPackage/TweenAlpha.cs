using UnityEngine;
using UnityEngine.UI;

public class TweenAlpha : Tween
{
    [HideInInspector]
    public float from;

    [HideInInspector]
    public float to;

    private MaskableGraphic[] maskableGraphics;

    private void Awake()
    {
        FindGraphics();
    }

    protected override void OnEnable()
    {
        if (!isEnable)
            return;

        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.StopEvent();
    }

    protected override void UpdateTween(bool isReverseFlag, float playTime)
    {
        if (maskableGraphics == null)
            return;

        float calculationFrom = isReverseFlag ? to : from;
        float calculationTo = isReverseFlag ? from : to;
        for (int i = 0; i < maskableGraphics.Length; ++i)
        {
            if (maskableGraphics[i] == null)
                continue;

            float r = maskableGraphics[i].color.r;
            float g = maskableGraphics[i].color.g;
            float b = maskableGraphics[i].color.b;
            float value = curve.Evaluate(calculationFrom + (curve.Evaluate(playTime / duration) * (calculationTo - calculationFrom)));

            if ((calculationTo - calculationFrom) > 0 && value >= calculationTo)
                value = calculationTo;
            else if ((calculationTo - calculationFrom) < 0 && value <= calculationTo)
                value = calculationTo;

            maskableGraphics[i].color = new Color(r, g, b, value);
        }
    }

    private void FindGraphics()
    {
        if (maskableGraphics != null)
            maskableGraphics = null;

        maskableGraphics = gameObject.GetComponents<MaskableGraphic>();
    }
}
