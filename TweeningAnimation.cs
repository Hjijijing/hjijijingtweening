using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hjijijing.Tweening
{

    public class TweeningAnimation
    {
        public MonoBehaviour source;
        public GameObject gameObject;


        public List<List<ITweeningAction>> actionQueues = new List<List<ITweeningAction>>();

        public int queueNumber = 0;
        public List<ITweener> ongoingTweens = new List<ITweener>();

        public List<ITweeningAction> builder = new List<ITweeningAction>();

        public ITweeningAction latestBuildAction = null;

        public TweeningAnimation(MonoBehaviour source, GameObject gameObject) : this(source)
        {
            this.gameObject = gameObject;
        }

        public TweeningAnimation(MonoBehaviour source)
        {
            this.source = source;
        }


        public void Start()
        {
            if (builder.Count != 0)
            {
                commitBuilder();
            }

            StartNextQueue();
        }

        public void commitBuilder()
        {
            actionQueues.Add(builder);
            builder = new List<ITweeningAction>();
        }

        protected void StartNextQueue()
        {
            if (queueNumber >= actionQueues.Count) return;

            List<ITweeningAction> actionQueue = actionQueues[queueNumber];

            foreach (ITweeningAction action in actionQueue)
            {
                action.doAction();
                if (action is ITweener)
                {
                    ongoingTweens.Add((ITweener)action);
                }
            }
            queueNumber++;

            if (ongoingTweens.Count == 0) StartNextQueue();
        }


        protected void tweenDone(ITweener tweener)
        {
            ongoingTweens.Remove(tweener);
            if (ongoingTweens.Count == 0) StartNextQueue();
        }

        public void Stop()
        {
            foreach (ITweener tweener in ongoingTweens)
            {
                tweener.Stop();
            }
        }


        public void forceFinish()
        {
            for (int i = 0; i < actionQueues.Count; i++)
            {
                List<ITweeningAction> actionQueue = actionQueues[i];

                for (int j = 0; j < actionQueue.Count; j++)
                {
                    ITweeningAction action = actionQueue[j];
                    if (!(action is ITweener)) continue;
                    ((ITweener)action).forceFinish();
                }
            }
        }

        public void revert()
        {
            if (actionQueues.Count == 0) return;

            for (int i = queueNumber; i > -1; i--)
            {
                List<ITweeningAction> actionQueue = actionQueues[i];

                for (int j = actionQueue.Count - 1; i > -1; j--)
                {
                    ITweeningAction action = actionQueue[j];
                    if (!(action is ITweener)) continue;
                    ((ITweener)action).revert();
                }
            }
        }


        public TweeningAnimation then()
        {
            commitBuilder();
            return this;
        }

        public TweeningAnimation SetEasing(Func<float, float> easing)
        {
            if (latestBuildAction == null) return this;
            if (!(latestBuildAction is ITweener)) return this;

            ITweener action = (ITweener)latestBuildAction;

            action.easing = easing;

            return this;
        }

        public TweeningAnimation SetEasingAll(Func<float, float> easing)
        {
            foreach (List<ITweeningAction> l in actionQueues)
            {
                foreach (ITweeningAction a in l)
                {
                    if (!(a is ITweener)) continue;
                    ((ITweener)a).easing = easing;
                }
            }


            foreach (ITweeningAction a in builder)
            {
                if (!(a is ITweener)) continue;
                ((ITweener)a).easing = easing;
            }

            return this;
        }
        #region Tweeners

        public TweeningAnimation move(Vector3 targetPosition, float duration)
        {
            return move(gameObject, targetPosition, duration);
        }

        public TweeningAnimation move(GameObject gameObject, Vector3 targetPosition, float duration)
        {
            if (gameObject == null) return this;
            PositionTweener action = new PositionTweener(tweenDone, source, gameObject, duration, targetPosition);
            latestBuildAction = action;



            builder.Add(action);
            return this;
        }



        public TweeningAnimation colorMesh(Color targetColor, float duration)
        {
            return colorMesh(gameObject, targetColor, duration);
        }

        public TweeningAnimation colorMesh(GameObject gameObject, Color targetColor, float duration)
        {
            if (gameObject == null) return this;
            MeshColorTweener action = new MeshColorTweener(tweenDone, source, gameObject, duration, targetColor);
            latestBuildAction = action;

            builder.Add(action);
            return this;
        }

        public TweeningAnimation rotate(Quaternion targetRotation, float duration)
        {
            return rotate(gameObject, targetRotation, duration);
        }

        public TweeningAnimation rotate(GameObject gameObject, Quaternion targetRotation, float duration)
        {
            if (gameObject == null) return this;
            RotationTweener action = new RotationTweener(tweenDone, source, gameObject, duration, targetRotation);
            latestBuildAction = action;

            builder.Add(action);
            return this;
        }

        public TweeningAnimation scale(Vector3 targetScale, float duration)
        {
            return scale(gameObject, targetScale, duration);
        }

        public TweeningAnimation scale(GameObject gameObject, Vector3 targetScale, float duration)
        {
            if (gameObject == null) return this;
            ScaleTweener action = new ScaleTweener(tweenDone, source, gameObject, duration, targetScale);
            latestBuildAction = action;



            builder.Add(action);
            return this;
        }


        public TweeningAnimation vector3Callback(Vector3 start, Vector3 end, Action<Vector3> callback, float duration)
        {
            Vector3TweeningActionCallback action = new Vector3TweeningActionCallback(tweenDone, source, gameObject, duration, start, end, callback);
            latestBuildAction = action;

            builder.Add(action);
            return this;
        }

        public TweeningAnimation vector2Callback(Vector2 start, Vector2 end, Action<Vector2> callback, float duration)
        {
            Vector2TweeningActionCallback action = new Vector2TweeningActionCallback(tweenDone, source, gameObject, duration, start, end, callback);
            latestBuildAction = action;

            builder.Add(action);
            return this;
        }

        public TweeningAnimation floatCallback(float start, float end, Action<float> callback, float duration)
        {
            FloatTweeningActionCallback action = new FloatTweeningActionCallback(tweenDone, source, gameObject, duration, start, end, callback);
            latestBuildAction = action;

            builder.Add(action);
            return this;
        }

        public TweeningAnimation intCallback(int start, int end, Action<int> callback, float duration)
        {
            IntTweeningActionCallback action = new IntTweeningActionCallback(tweenDone, source, gameObject, duration, start, end, callback);
            latestBuildAction = action;

            builder.Add(action);
            return this;
        }

        public TweeningAnimation QuaternionCallback(Quaternion start, Quaternion end, Action<Quaternion> callback, float duration)
        {
            QuaternionTweeningActionCallback action = new QuaternionTweeningActionCallback(tweenDone, source, gameObject, duration, start, end, callback);
            latestBuildAction = action;

            builder.Add(action);
            return this;
        }

        public TweeningAnimation stringCallback(string start, string end, Action<string> callback, float duration)
        {
            StringTweeningActionCallback action = new StringTweeningActionCallback(tweenDone, source, gameObject, duration, start, end, callback);
            latestBuildAction = action;


            builder.Add(action);
            return this;
        }



        public TweeningAnimation call(Action action)
        {
            CallBackIntermediaryAction tweeningaction = new CallBackIntermediaryAction(action);
            latestBuildAction = tweeningaction;

            builder.Add(tweeningaction);
            return this;
        }


        public TweeningAnimation Wait(float duration)
        {
            then();
            WaitForTime action = new WaitForTime(tweenDone, source, gameObject, duration);
            latestBuildAction = action;

            builder.Add(action);
            then();

            return this;
        }

        #endregion

    }

}


