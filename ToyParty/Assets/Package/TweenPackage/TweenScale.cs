using UnityEngine;

public class TweenScale : Tween
{
    [HideInInspector]
    public Vector3 from;

    [HideInInspector]
    public Vector3 to;

    private Transform[] transforms;

    private void Awake()
    {
        FindTransform();
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
        if (transforms == null)
            return;

        Vector3 calculationFrom = isReverseFlag ? to : from;
        Vector3 calculationTo = isReverseFlag ? from : to;
        for (int i = 0; i < transforms.Length; ++i)
        {
            if (transforms[i] == null)
                continue;

            float valueX = (calculationFrom.x + (curve.Evaluate(playTime / duration) * (calculationTo.x - calculationFrom.x)));
            float valueY = (calculationFrom.y + (curve.Evaluate(playTime / duration) * (calculationTo.y - calculationFrom.y)));
            float valueZ = (calculationFrom.z + (curve.Evaluate(playTime / duration) * (calculationTo.z - calculationFrom.z)));

            transforms[i].transform.localScale = new Vector3(valueX, valueY, valueZ);
        }
    }

    private void FindTransform()
    {
        if (transforms != null)
            transforms = null;

        transforms = gameObject.GetComponents<Transform>();
    }
}
