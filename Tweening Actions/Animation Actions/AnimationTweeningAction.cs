using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace hjijijing.Tweening
{

    public abstract class AnimationTweeningAction<T> : ITweeningAction, ITweener
    {
        public Action<ITweener> onDone;
        public MonoBehaviour mono;

        protected Coroutine coroutine;

        public Func<float, float> easing { get; set; } = Easing.linear;


        public float duration;
        public float startDelay = 0f;
        public float endDelay = 0f;
        public T startValue;
        public bool startDetermined = false;
        public T endValue;

        public GameObject gameObject;

        protected float timeSinceStart = 0f;


        public AnimationTweeningAction(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, T endValue, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            this.onDone = onDone;
            this.mono = mono;
            this.duration = duration;
            this.endValue = endValue;
            this.gameObject = gameObject;
            this.startDelay = startDelay;
            this.endDelay = endDelay;
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

       /* public void reverse()
        {
            T temp = startValue;
            startValue = endValue;
            endValue = temp;
        }
        */

        public void doAction()
        {
            coroutine = mono.StartCoroutine(execute(onDone));
        }

        public abstract void findStartValue();


        public void SetStartValue(T start)
        {
            startValue = start;
            startDetermined = true;
        }


        public abstract void modifyGameObject(float time);

        public IEnumerator execute(Action<ITweener> onDone)
        {
            timeSinceStart = 0f;
            if (startDelay > 0f)
                yield return new WaitForSeconds(startDelay);

            if(!startDetermined)
            findStartValue();
            modifyGameObject(0f);

            yield return null;

            while ((timeSinceStart += Time.deltaTime) < duration)
            {
                modifyGameObject(easing((timeSinceStart) / duration));
                yield return null;
            }


            modifyGameObject(1f);

            if (endDelay > 0f)
                yield return new WaitForSeconds(endDelay);
            onDone?.Invoke(this);
        }

        public abstract ITweeningAction getReverse(Action<ITweener> onDone);


        public override string ToString()
        {
            return "Start Value: " + startValue + " End Value: " + endValue + " Duration: " + duration + " Start Delay: " + startDelay + " End Delay: " + endDelay;
        }
    }


    


}

