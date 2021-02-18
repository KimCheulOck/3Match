using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public enum TweenPlayStyle
{
    One = 0,
    PingPongOne = 1,
    PingPong = 2,
    Loop = 3,
}

public abstract class Tween : MonoBehaviour
{
    [HideInInspector]
    public TweenPlayStyle playStyle = TweenPlayStyle.One;

    [HideInInspector]
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 1f));

    [HideInInspector]
    public float duration = 0;

    [HideInInspector]
    public float startTime = 0;

    [HideInInspector]
    public int tweenGroup = 0;

    [HideInInspector]
    public bool isEnable = false;

    [HideInInspector]
    public bool isReverse = false;

    [HideInInspector]
    public bool isPause = false;

    [HideInInspector]
    public UnityEvent finishEvnet;

    private bool checkReverse;
    private bool checkOneLoop;
    private float checkPlayTime;
    private float updateTime = 0.01f;

    protected virtual void OnEnable()
    {
        PlayEvent();
    }

    protected virtual void OnDisable()
    {
        StopEvent();
    }

    protected abstract void UpdateTween(bool isReverseFlag, float playTime);

    [ContextMenu("PlayEvent")]
    public void PlayEvent()
    {
        StopEvent();

        Play();
    }

    [ContextMenu("StopEvent")]
    public void StopEvent()
    {
        checkPlayTime = 0;
        CancelInvoke("InvokePlay");
    }

    [ContextMenu("PauseEvent")]
    public void PauseEvent()
    {
        isPause = !isPause;

        if(isPause)
            CancelInvoke("InvokePlay");
        else
            InvokeRepeating("InvokePlay", 0, updateTime);
    }

    public void ClearFinishedEvent()
    {
        finishEvnet.RemoveAllListeners();
    }

    public void AddFinishedEvent(UnityAction unityAction)
    {
        finishEvnet.AddListener(unityAction);
    }

    private void Play()
    {
        checkReverse = isReverse;
        checkOneLoop = false;

        InvokeRepeating("InvokePlay", startTime, updateTime);
    }

    private void InvokePlay()
    {
        bool end = checkPlayTime >= duration;
        float playTime = end ? duration : checkPlayTime;
        UpdateTween(checkReverse, playTime);

        checkPlayTime += updateTime;

        if (end)
        {
            if (CheckPlayOneEnd())
                CancelInvoke("InvokePlay");
            else if (CheckPlayPingPongOneEnd())
                CancelInvoke("InvokePlay");
            else if (CheckPlayPingPong())
                CancelInvoke("InvokePlay");

            checkPlayTime = 0;

            if ((playStyle == TweenPlayStyle.One) ||
                (playStyle == TweenPlayStyle.PingPongOne && checkOneLoop))
            {
                FinishEvent();
            }

        }
    }

    private bool CheckPlayOneEnd()
    {
        if (playStyle == TweenPlayStyle.One)
            return true;

        return false;
    }

    private bool CheckPlayPingPongOneEnd()
    {
        if (playStyle == TweenPlayStyle.PingPongOne)
        {
            if (checkOneLoop)
                return true;

            checkOneLoop = true;
            checkReverse = !checkReverse;
        }

        return false;
    }

    private bool CheckPlayPingPong()
    {
        if (playStyle == TweenPlayStyle.PingPong)
        {
            checkReverse = !checkReverse;
            return false;
        }

        return false;
    }

    private void FinishEvent()
    {
        if (finishEvnet == null)
            return;

        finishEvnet.Invoke();
    }
}
