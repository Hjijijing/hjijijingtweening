using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hjijijing.Tweening
{

    public class MeshColorTweener : AnimationTweeningAction<Color>
    {

        public MeshColorTweener(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, Color endValue, float duration, float startDelay = 0f, float endDelay = 0f) : base(onDone, mono, gameObject, endValue, duration, startDelay, endDelay) { }

        public override ITweeningAction getReverse(Action<ITweener> onDone)
        {
            return new MeshColorTweener(onDone, mono, gameObject, startValue, duration, startDelay, endDelay);
        }

        public override void modifyGameObject(float time)
        {
            MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
            if (mr == null) return;
            mr.material.color = Color.LerpUnclamped(startValue, endValue, time);

        }

        public override void findStartValue()
        {
            MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
            if (mr == null) return;
            startValue = mr.material.color;
        }
    }

}
