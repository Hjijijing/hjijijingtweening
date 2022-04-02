using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace hjijijing.Tweening
{
    public abstract class AnimationTweeningAction : ITweeningAction, ITweener
    {
        public float duration;
        public float startDelay = 0f;
        public float endDelay = 0f;


        public bool forceOneAtEnd { get; set; }
        public Func<float, float> easing { get; set; } = Easing.linear;

        public Action<ITweener> onDone;
        public MonoBehaviour mono;

        protected Coroutine coroutine;

        public delegate void AnimationTweeningEvent(AnimationTweeningAction action);
        public AnimationTweeningEvent onTweenStarted;
        public AnimationTweeningEvent onTweenEnded;
        public AnimationTweeningEvent onTweenStopped;
        public AnimationTweeningEvent onTweenForceFinished;
        public AnimationTweeningEvent onTweenReverted;

        public GameObject gameObject;

        protected float timeSinceStart = 0f;



        public AnimationTweeningAction(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            this.onDone = onDone;
            this.mono = mono;
            this.duration = duration;
            this.gameObject = gameObject;
            this.startDelay = startDelay;
            this.endDelay = endDelay;
        }


        public void Stop()
        {
            if (coroutine == null) return;
            mono.StopCoroutine(coroutine);
            onTweenStopped?.Invoke(this);
        }

        public void forceFinish()
        {
            Stop();
            modifyGameObject(1f);
            onTweenForceFinished?.Invoke(this);
        }

        public void revert()
        {
            Stop();
            modifyGameObject(0f);
            onTweenReverted?.Invoke(this);
        }


        public void doAction()
        {
            coroutine = mono.StartCoroutine(execute(onDone));
        }

        public abstract IEnumerator execute(Action<ITweener> onDone);

        public abstract ITweeningAction getReverse(Action<ITweener> onDone);

        public abstract void modifyGameObject(float time);
    }



    public abstract class AnimationTweeningAction<T> : AnimationTweeningAction
    {     
        public T startValue;
        public T endValue;
        public bool startDetermined = false;

        public AnimationTweeningAction(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, T endValue, float duration, float startDelay = 0f, float endDelay = 0f)
            : base(onDone, mono, gameObject, duration, startDelay, endDelay)
        {
            this.endValue = endValue;
        }

       

        public abstract void findStartValue();


        public void SetStartValue(T start)
        {
            startValue = start;
            startDetermined = true;
        }

        public override IEnumerator execute(Action<ITweener> onDone)
        {
            timeSinceStart = 0f;
            if (startDelay > 0f)
                yield return new WaitForSeconds(startDelay);

            if(!startDetermined)
            findStartValue();

            onTweenStarted?.Invoke(this);
            modifyGameObject(0f);

            yield return null;

            while ((timeSinceStart += Time.deltaTime) < duration)
            {
                modifyGameObject(easing((timeSinceStart) / duration));
                yield return null;
            }


            modifyGameObject(forceOneAtEnd ? 1f : easing(1f));
            onTweenEnded?.Invoke(this);

            if (endDelay > 0f)
                yield return new WaitForSeconds(endDelay);
            onDone?.Invoke(this);
        }




        public override string ToString()
        {
            return "Start Value: " + startValue + " End Value: " + endValue + " Duration: " + duration + " Start Delay: " + startDelay + " End Delay: " + endDelay;
        }
    }


    


}

