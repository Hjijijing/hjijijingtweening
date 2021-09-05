using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace hjijijing.Tweening
{


    public abstract class AnimationTweeningActionCallback<T> : AnimationTweeningAction<T>
    {

        protected Action<T> callback;

        public AnimationTweeningActionCallback(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, T startValue, T endValue, Action<T> callback, float duration, float startDelay = 0f, float endDelay = 0f) : base(onDone, mono, gameObject, endValue, duration, startDelay, endDelay)
        {
            this.startValue = startValue;
            this.callback = callback;
        }





        public override void setStartValue()
        {
            return; //Start value is already set in constructor
        }
    }



    public class Vector3TweeningActionCallback : AnimationTweeningActionCallback<Vector3>
    {

        public Vector3TweeningActionCallback(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, Vector3 startValue, Vector3 endValue, Action<Vector3> callback, float duration, float startDelay = 0f, float endDelay = 0f) : base(onDone, mono, gameObject, startValue, endValue, callback, duration, startDelay, endDelay) { }

        public override void modifyGameObject(float time)
        {
            callback?.Invoke(Vector3.LerpUnclamped(startValue, endValue, time));
        }

        public override ITweeningAction getReverse()
        {
            return new Vector3TweeningActionCallback(onDone, mono, gameObject, endValue, startValue, callback, duration, startDelay, endDelay);
        }
    }

    public class Vector2TweeningActionCallback : AnimationTweeningActionCallback<Vector2>
    {

        public Vector2TweeningActionCallback(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, Vector2 startValue, Vector2 endValue, Action<Vector2> callback, float duration, float startDelay = 0f, float endDelay = 0f) : base(onDone, mono, gameObject, startValue, endValue, callback, duration, startDelay, endDelay) { }

        public override void modifyGameObject(float time)
        {
            callback?.Invoke(Vector2.LerpUnclamped(startValue, endValue, time));
        }

        public override ITweeningAction getReverse()
        {
            return new Vector2TweeningActionCallback(onDone, mono, gameObject, endValue, startValue, callback, duration, startDelay, endDelay);
        }
    }


    public class FloatTweeningActionCallback : AnimationTweeningActionCallback<float>
    {

        public FloatTweeningActionCallback(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, float startValue, float endValue, Action<float> callback, float duration, float startDelay = 0f, float endDelay = 0f) : base(onDone, mono, gameObject, startValue, endValue, callback, duration, startDelay, endDelay) { }

        public override void modifyGameObject(float time)
        {
            callback?.Invoke(Mathf.LerpUnclamped(startValue, endValue, time));

        }

        public override ITweeningAction getReverse()
        {
            return new FloatTweeningActionCallback(onDone, mono, gameObject, endValue, startValue, callback, duration, startDelay, endDelay);
        }
    }

    public class ColorTweeningActionCallback : AnimationTweeningActionCallback<Color>
    {

        public ColorTweeningActionCallback(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, Color startValue, Color endValue, Action<Color> callback, float duration, float startDelay = 0f, float endDelay = 0f) : base(onDone, mono, gameObject, startValue, endValue, callback, duration, startDelay, endDelay) { }

        public override void modifyGameObject(float time)
        {
            callback?.Invoke(Color.LerpUnclamped(startValue, endValue, time));
        }

        public override ITweeningAction getReverse()
        {
            return new ColorTweeningActionCallback(onDone, mono, gameObject, endValue, startValue, callback, duration, startDelay, endDelay);
        }
    }


    public class IntTweeningActionCallback : AnimationTweeningActionCallback<int>
    {

        public IntTweeningActionCallback(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, int startValue, int endValue, Action<int> callback, float duration, float startDelay = 0f, float endDelay = 0f) : base(onDone, mono, gameObject, startValue, endValue, callback, duration, startDelay, endDelay) { }

        public override void modifyGameObject(float time)
        {
            callback?.Invoke((int)Mathf.LerpUnclamped(startValue, endValue, time));

        }

        public override ITweeningAction getReverse()
        {
            return new IntTweeningActionCallback(onDone, mono, gameObject, endValue, startValue, callback, duration, startDelay, endDelay);
        }
    }

    public class QuaternionTweeningActionCallback : AnimationTweeningActionCallback<Quaternion>
    {

        public QuaternionTweeningActionCallback(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, Quaternion startValue, Quaternion endValue, Action<Quaternion> callback, float duration, float startDelay = 0f, float endDelay = 0f) : base(onDone, mono, gameObject, startValue, endValue, callback, duration, startDelay, endDelay) { }

        public override void modifyGameObject(float time)
        {
            callback?.Invoke(Quaternion.LerpUnclamped(startValue, endValue, time));

        }

        public override ITweeningAction getReverse()
        {
            return new QuaternionTweeningActionCallback(onDone, mono, gameObject, endValue, startValue, callback, duration, startDelay, endDelay);
        }
    }

    public class StringTweeningActionCallback : AnimationTweeningActionCallback<string>
    {

        public StringTweeningActionCallback(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, string startValue, string endValue, Action<string> callback, float duration, float startDelay = 0f, float endDelay = 0f) : base(onDone, mono, gameObject, startValue, endValue, callback, duration, startDelay, endDelay) { }

        public override void modifyGameObject(float time)
        {
            int charAmount = (int)Mathf.LerpUnclamped(0, endValue.Length, time);

            string result = startValue + endValue.Substring(0, charAmount);

            callback?.Invoke(result);

        }

        public override ITweeningAction getReverse()
        {
            return new StringTweeningActionCallback(onDone, mono, gameObject, endValue, startValue, callback, duration, startDelay, endDelay);
        }
    }




}




