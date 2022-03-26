using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hjijijing.Tweening
{

    public class ScaleTweener : AnimationTweeningAction<Vector3>
    {

        public ScaleTweener(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, Vector3 endValue, float duration, float startDelay = 0f, float endDelay = 0f) : base(onDone, mono, gameObject, endValue, duration, startDelay, endDelay) { }

        public override ITweeningAction getReverse(Action<ITweener> onDone)
        {
            return new ScaleTweener(onDone, mono, gameObject, startValue, duration, startDelay, endDelay);
        }

        public override void modifyGameObject(float time)
        {
            gameObject.transform.localScale = Vector3.LerpUnclamped(startValue, endValue, time);
        }

        public override void findStartValue()
        {
            startValue = gameObject.transform.localScale;
        }
    }

}


