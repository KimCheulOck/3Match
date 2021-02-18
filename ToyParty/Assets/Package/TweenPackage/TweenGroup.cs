using System.Collections.Generic;
using UnityEngine;

public class TweenGroup : MonoBehaviour
{
    [SerializeField]
    private int tweenGroup = 0;

    [SerializeField]
    private bool onEnable = false;

    private List<Tween> tweens = new List<Tween>();
    private int playTweenGroup = 0;

    private void OnEnable()
    {
        if(onEnable)
            PlayEvent(tweenGroup);
    }

    public void PlayEvent(int playTweenGroup = 0)
    {
        this.playTweenGroup = playTweenGroup;

        StopEvent();

        SetTweens();

        for (int i = 0; i < tweens.Count; ++i)
            tweens[i].PlayEvent();
    }

    public void StopEvent()
    {
        for (int i = 0; i < tweens.Count; ++i)
            tweens[i].StopEvent();
    }

    public void PauseEvent()
    {
        for (int i = 0; i < tweens.Count; ++i)
            tweens[i].PauseEvent();
    }

    private void SetTweens()
    {
        tweens.Clear();

        Tween[] childTweens = GetComponentsInChildren<Tween>();
        for (int i = 0; i < childTweens.Length; ++i)
        {
            if (childTweens[i].tweenGroup == playTweenGroup)
                tweens.Add(childTweens[i]);
        }
    }
}
