using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hjijijing.Tweening
{

    public class RotationTweener : AnimationTweeningAction<Quaternion>
    {

        public RotationTweener(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, Quaternion endValue, float duration, float startDelay = 0f, float endDelay = 0f) : base(onDone, mono, gameObject, endValue, duration, startDelay, endDelay) { }

        public override ITweeningAction getReverse()
        {
            return new RotationTweener(onDone, mono, gameObject, startValue, duration, startDelay, endDelay);
        }

        public override void modifyGameObject(float time)
        {
            gameObject.transform.rotation = Quaternion.LerpUnclamped(startValue, endValue, time);
        }

        public override void setStartValue()
        {
            startValue = gameObject.transform.rotation;
        }
    }

}


