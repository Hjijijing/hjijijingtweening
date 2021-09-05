using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace hjijijing.Tweening {

    public abstract class AnimationTweeningAction<T> : ITweeningAction, ITweener
    {
        public Action<ITweener> onDone;
        public MonoBehaviour mono;

        protected Coroutine coroutine;

        public Func<float, float> easing { get; set; } = Easing.linear;


        public float duration;
        public T startValue;
        public T endValue;

        public GameObject gameObject;

        protected float timeSinceStart = 0f;

        public AnimationTweeningAction(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, float duration, T endValue)
        {
            this.onDone = onDone;
            this.mono = mono;
            this.duration = duration;
            this.endValue = endValue;
            this.gameObject = gameObject;
        }

        public void Stop()
        {
            if (coroutine == null) return;
            mono.StopCoroutine(coroutine);
        }

        public void forceFinish()
        {
            Stop();
            modifyGameObject(1f);
        }

        public void revert()
        {
            Stop();
            modifyGameObject(0f);
        }

        public void doAction()
        {
            setStartValue();
            coroutine = mono.StartCoroutine(execute(onDone));
        }

        public abstract void setStartValue();

        public abstract void modifyGameObject(float time);

        public IEnumerator execute(Action<ITweener> onDone)
        {
            modifyGameObject(0f);
            timeSinceStart = 0f;
            yield return null;

            while((timeSinceStart += Time.deltaTime) < duration)
            {
                modifyGameObject(easing(timeSinceStart / duration));
                yield return null;
            }

            modifyGameObject(1f);
            onDone?.Invoke(this);
        }
    }


}

