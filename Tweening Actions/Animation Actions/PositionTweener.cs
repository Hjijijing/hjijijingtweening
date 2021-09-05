using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hjijijing.Tweening
{

    public class PositionTweener : AnimationTweeningAction<Vector3>
    {

        public PositionTweener(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, float duration, Vector3 endValue) : base(onDone, mono, gameObject, duration, endValue) { }


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


