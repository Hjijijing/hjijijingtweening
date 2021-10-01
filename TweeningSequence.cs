using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace hjijijing.Tweening
{
    public class TweeningSequence : List<ITweeningAction>
    {

        HashSet<string> markers = new HashSet<string>();

        /// <summary>
        /// List of tweens that are currently animating.
        /// </summary>
        public List<ITweener> ongoingTweens = new List<ITweener>();

        /// <summary>
        /// The latest action that has been added to the builder.
        /// </summary>
        public ITweeningAction latestBuildAction;

        public delegate void TweenSequenceDone();
        public TweenSequenceDone onTweenSequenceDone;

        public void StartSequence()
        {
            foreach (ITweeningAction action in this)
            {
                action.doAction();
                if (action is ITweener)
                {
                    ongoingTweens.Add((ITweener)action);
                }
            }

            if (ongoingTweens.Count == 0) onTweenSequenceDone?.Invoke();

        }

        public void StopSequence()
        {
            foreach(ITweener tweener in ongoingTweens)
            {
                tweener.Stop();
            }
        }

        /// <summary>
        /// Internal function that is called by tweeners every time a tween has finished. When all tweens in the currently executing builder has finished, this function will begin the next queue
        /// </summary>
        /// <param name="tweener">The tweener that has finished</param>
        public void tweenDone(ITweener tweener)
        {
            ongoingTweens.Remove(tweener);
            if (ongoingTweens.Count == 0) onTweenSequenceDone?.Invoke();
        }

        public void SetEasingAll(Func<float, float> easing)
        {
            foreach(ITweeningAction a in this)
            {
                if (!(a is ITweener)) continue;
                ((ITweener)a).easing = easing;
            }
        }

        public void SetEasingLatest(Func<float, float> easing)
        {
            if (latestBuildAction == null) return;
            if (!(latestBuildAction is ITweener)) return;

            ITweener action = (ITweener)latestBuildAction;

            action.easing = easing;
        }


        public new void Add(ITweeningAction action)
        {
            base.Add(action);
            latestBuildAction = action;
        }


        public void AddMarker(string marker)
        {
            markers.Add(marker);
        }

        public void RemoveMarker(string marker)
        {
            markers.Remove(marker);
        }

        public bool HasMarker(string marker)
        {
            return markers.Contains(marker);
        }

        public HashSet<string> GetMarkers()
        {
            return markers;
        }

        public TweeningSequence GetReverse()
        {
            TweeningSequence reverseSequence = new TweeningSequence();
            foreach(ITweeningAction action in this)
            {
                ITweeningAction actionToAdd;
                if (action is ITweener)
                {
                    actionToAdd = ((ITweener)action).getReverse();
                }
                else
                {
                    actionToAdd = action;
                }

                reverseSequence.Add(actionToAdd);
            }

            return reverseSequence;
        }

        public void ForceFinish()
        {
            ForEachITweener((action) => { action.forceFinish(); });
        }

        public void Revert()
        {
            ForEachITweener((action) => { action.revert(); });
        }

        public void ForEachITweener(Action<ITweener> callback)
        {
            foreach (ITweeningAction action in this)
            {
                if (!(action is ITweener)) continue;
                callback.Invoke((ITweener)action);
            }
        }


        public bool IsEmpty()
        {
            return Count == 0;
        }


    }
}

