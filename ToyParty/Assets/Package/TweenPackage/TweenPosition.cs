using UnityEngine;

public class TweenPosition : Tween
{
    [HideInInspector]
    public Vector3 from;

    [HideInInspector]
    public Vector3 to;

    [HideInInspector]
    public bool isLocalPostion = true;

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
        if (transform == null)
            return;

        Vector3 calculationFrom = isReverseFlag ? to : from;
        Vector3 calculationTo = isReverseFlag ? from : to;
        float valueX = (calculationFrom.x + (curve.Evaluate(playTime / duration) * (calculationTo.x - calculationFrom.x)));
        float valueY = (calculationFrom.y + (curve.Evaluate(playTime / duration) * (calculationTo.y - calculationFrom.y)));
        float valueZ = (calculationFrom.z + (curve.Evaluate(playTime / duration) * (calculationTo.z - calculationFrom.z)));

        if(isLocalPostion)
            transform.transform.localPosition = new Vector3(valueX, valueY, valueZ);
        else
            transform.transform.position = new Vector3(valueX, valueY, valueZ);
    }
}
