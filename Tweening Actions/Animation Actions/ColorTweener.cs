using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hjijijing.Tweening
{

    public class MeshColorTweener : AnimationTweeningAction<Color>
    {

        public MeshColorTweener(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, float duration, Color endValue) : base(onDone, mono, gameObject, duration, endValue) { }

        public override void modifyGameObject(float time)
        {
            MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
            if (mr == null) return;
            mr.material.color = Color.LerpUnclamped(startValue, endValue, time);

        }

        public override void setStartValue()
        {
            MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
            if (mr == null) return;
            startValue = mr.material.color;
        }
    }

}
