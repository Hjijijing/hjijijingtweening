using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hjijijing.Tweening
{

    public class RotationTweener : AnimationTweeningAction<Quaternion>
    {

        public RotationTweener(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, float duration, Quaternion endValue) : base(onDone, mono, gameObject, duration, endValue) { }


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


