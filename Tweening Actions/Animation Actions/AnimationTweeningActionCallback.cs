using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace hjijijing.Tweening {


    public abstract class AnimationTweeningActionCallback<T> : AnimationTweeningAction<T>
    {

        protected Action<T> callback;

        public AnimationTweeningActionCallback(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, float duration, T startValue, T endValue, Action<T> callback) : base(onDone, mono, gameObject, duration, endValue) {
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

        public Vector3TweeningActionCallback(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, float duration, Vector3 startValue, Vector3 endValue, Action<Vector3> callback) : base(onDone, mono, gameObject, duration, startValue, endValue, callback) { }

        public override void modifyGameObject(float time)
        {
            callback?.Invoke(Vector3.LerpUnclamped(startValue, endValue, time));
        }
    }

    public class Vector2TweeningActionCallback : AnimationTweeningActionCallback<Vector2>
    {

        public Vector2TweeningActionCallback(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, float duration, Vector2 startValue, Vector2 endValue, Action<Vector2> callback) : base(onDone, mono, gameObject, duration, startValue, endValue, callback) { }

        public override void modifyGameObject(float time)
        {
            callback?.Invoke(Vector2.LerpUnclamped(startValue, endValue, time));
        }
    }


    public class FloatTweeningActionCallback : AnimationTweeningActionCallback<float>
    {

        public FloatTweeningActionCallback(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, float duration, float startValue, float endValue, Action<float> callback) : base(onDone, mono, gameObject, duration, startValue, endValue, callback) { }

        public override void modifyGameObject(float time)
        {
            callback?.Invoke(Mathf.LerpUnclamped(startValue, endValue, time));

        }
    }

    public class ColorTweeningActionCallback : AnimationTweeningActionCallback<Color>
    {

        public ColorTweeningActionCallback(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, float duration, Color startValue, Color endValue, Action<Color> callback) : base(onDone, mono, gameObject, duration, startValue, endValue, callback) { }

        public override void modifyGameObject(float time)
        {
            callback?.Invoke(Color.LerpUnclamped(startValue, endValue, time));
        }
    }


    public class IntTweeningActionCallback : AnimationTweeningActionCallback<int>
    {

        public IntTweeningActionCallback(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, float duration, int startValue, int endValue, Action<int> callback) : base(onDone, mono, gameObject, duration, startValue, endValue, callback) { }

        public override void modifyGameObject(float time)
        {
            callback?.Invoke((int)Mathf.LerpUnclamped(startValue, endValue, time));

        }
    }

    public class QuaternionTweeningActionCallback : AnimationTweeningActionCallback<Quaternion>
    {

        public QuaternionTweeningActionCallback(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, float duration, Quaternion startValue, Quaternion endValue, Action<Quaternion> callback) : base(onDone, mono, gameObject, duration, startValue, endValue, callback) { }

        public override void modifyGameObject(float time)
        {
            callback?.Invoke(Quaternion.LerpUnclamped(startValue, endValue, time));

        }
    }

    public class StringTweeningActionCallback : AnimationTweeningActionCallback<string>
    {

        public StringTweeningActionCallback(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, float duration, string startValue, string endValue, Action<string> callback) : base(onDone, mono, gameObject, duration, startValue, endValue, callback) { }

        public override void modifyGameObject(float time)
        {
            int charAmount = (int)Mathf.LerpUnclamped(0, endValue.Length, time);

            string result = startValue + endValue.Substring(0, charAmount);

            callback?.Invoke(result);

        }
    }




}




