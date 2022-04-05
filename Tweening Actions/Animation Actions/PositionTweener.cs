using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hjijijing.Tweening
{

    public class PositionTweener : AnimationTweeningAction<Vector3>
    {

        public PositionTweener(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, Vector3 endValue, float duration, float startDelay = 0f, float endDelay = 0f) : base(onDone, mono, gameObject, endValue, duration, startDelay, endDelay) { }

        public override ITweeningAction getReverse(Action<ITweener> onDone)
        {
            return new PositionTweener(onDone, mono, gameObject, startValue, duration, startDelay, endDelay);
        }

        public override void modifyGameObject(float time)
        {
            gameObject.transform.position = Vector3.LerpUnclamped(startValue, endValue, time);
        }

        public override void findStartValue()
        {
            startValue = gameObject.transform.position;
        }
    }


    public class AnchoredPositionTweener : AnimationTweeningAction<Vector3>
    {

        public AnchoredPositionTweener(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, Vector3 endValue, float duration, float startDelay = 0f, float endDelay = 0f) : base(onDone, mono, gameObject, endValue, duration, startDelay, endDelay) { }

        public override ITweeningAction getReverse(Action<ITweener> onDone)
        {
            return new AnchoredPositionTweener(onDone, mono, gameObject, startValue, duration, startDelay, endDelay);
        }

        public override void modifyGameObject(float time)
        {
            if (!(gameObject.transform is RectTransform)) return;
            ((RectTransform)gameObject.transform).anchoredPosition = Vector3.LerpUnclamped(startValue, endValue, time);
        }

        public override void findStartValue()
        {
            if (!(gameObject.transform is RectTransform)) return;
            startValue = ((RectTransform)gameObject.transform).anchoredPosition;
        }
    }

}


