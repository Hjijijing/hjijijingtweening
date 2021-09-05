using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hjijijing.Tweening
{

    public class PositionTweener : AnimationTweeningAction<Vector3>
    {

        public PositionTweener(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, Vector3 endValue, float duration, float startDelay = 0f, float endDelay = 0f) : base(onDone, mono, gameObject, endValue, duration, startDelay, endDelay) { }

        public override ITweeningAction getReverse()
        {
            return new PositionTweener(onDone, mono, gameObject, startValue, duration, startDelay, endDelay);
        }

        public override void modifyGameObject(float time)
        {
            gameObject.transform.position = Vector3.LerpUnclamped(startValue, endValue, time);
        }

        public override void setStartValue()
        {
            startValue = gameObject.transform.position;
        }
    }

}


