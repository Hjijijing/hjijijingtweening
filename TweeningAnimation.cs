using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hjijijing.Tweening
{

    public class TweeningAnimation
    {
        
        
        public delegate void AnimationDelegate();
        public AnimationDelegate onRevert;
        public AnimationDelegate onStart;
        public AnimationDelegate onStop;
        public AnimationDelegate onForceFinish;
        public AnimationDelegate onEnded;
        

        /// <summary>
        /// The source of the tweening animation. This is mainly used to access the StartCoroutine() function
        /// </summary>
        public MonoBehaviour source;

        /// <summary>
        /// The primary gameObject of the animation. If an action is added and no gameObject is specified, it will default to this.
        /// </summary>
        public GameObject gameObject;


        /// <summary>
        /// The default easing. All actions that are added will use this easing by default.
        /// </summary>
        public Func<float, float> easing = Easing.linear;

        /// <summary>
        /// The list of queues of tweening animations.
        /// </summary>
        public List<TweeningSequence> actionQueues = new List<TweeningSequence>();

        /// <summary>
        /// The current queue number
        /// </summary>
        public int queueNumber = 0;

        /// <summary>
        /// The sequence that is currently playing
        /// </summary>
        public TweeningSequence currentPlayingSequence;
        
        /// <summary>
        /// Lists of tweens that are being build in the latest queue.
        /// </summary>
        public TweeningSequence builder = new TweeningSequence();

       

        /// <summary>
        /// Creates a tweening animation with the given source and primary gameobject
        /// </summary>
        /// <param name="source">The source of the tweening animation</param>
        /// <param name="gameObject">The primary (default) gameobject that will be animated</param>
        public TweeningAnimation(MonoBehaviour source, GameObject gameObject) : this(source)
        {
            this.gameObject = gameObject;
        }

        /// <summary>
        /// Creates a tweening animation with the given source
        /// </summary>
        /// <param name="source">The source of the tweening animation</param>
        public TweeningAnimation(MonoBehaviour source)
        {
            this.source = source;
        }

        /// <summary>
        /// Creates a tweening animation with the given source and default easing function
        /// </summary>
        /// <param name="source">The source of the tweening animation</param>
        /// <param name="easing">The default easing function that will be used on newly added actions</param>
        public TweeningAnimation(MonoBehaviour source, Func<float, float> easing) : this(source)
        {
            this.easing = easing;
        }

        /// <summary>
        /// Creates a tweening animation with the given source, primary gameobject and default easing function
        /// </summary>
        /// <param name="source">The source of the tweening animation</param>
        /// <param name="gameObject">The primary (default) gameobject that will be animated</param>
        /// <param name="easing">The default easing function that will be used on newly added actions</param>
        public TweeningAnimation(MonoBehaviour source, GameObject gameObject, Func<float, float> easing) : this(source)
        {
            this.gameObject = gameObject;
            this.easing = easing;
        }


        /// <summary>
        /// Starts the tweening animation
        /// </summary>
        public void Start()
        {
            onStart?.Invoke();
            if (builder.Count != 0)
            {
                commitBuilder();
            }

            StartNextQueue();
        }

        /// <summary>
        /// Internal function that commits the builder and creates a new queue
        /// </summary>
        protected void commitBuilder()
        {
            if (builder.Count == 0 && !builder.HasAnyMarkers()) return;
            actionQueues.Add(builder);
            builder = new TweeningSequence();
        }

        /// <summary>
        /// Internal function that starts the next queue
        /// </summary>
        protected void StartNextQueue()
        {
            if(currentPlayingSequence != null)
                currentPlayingSequence.onTweenSequenceDone -= StartNextQueue;

            if (queueNumber >= actionQueues.Count) {
                onEnded?.Invoke();
                return; }

            TweeningSequence sequence = actionQueues[queueNumber];
            currentPlayingSequence = sequence;

            sequence.onTweenSequenceDone += StartNextQueue;

           
            

            sequence.StartSequence(()=> { queueNumber++; });

            

        }


        /// <summary>
        /// Stops the tweening animation
        /// </summary>
        public void Stop()
        {
            if (currentPlayingSequence == null) return;
            currentPlayingSequence.StopSequence();
            onStop?.Invoke();
        }

        /// <summary>
        /// Forces the tweening animation to finish immediately.
        /// </summary>
        public void forceFinish()
        {
            for (int i = 0; i < actionQueues.Count; i++)
            {
                TweeningSequence sequence = actionQueues[i];

                sequence.ForceFinish();
            }

            onForceFinish?.Invoke();
        }

        /// <summary>
        /// Reverts the changes done by the tweening animation.
        /// </summary>
        public void revert()
        {
            if (actionQueues.Count == 0) return;

            for (int i = queueNumber; i > -1; i--)
            {
                TweeningSequence sequence = actionQueues[i];

                sequence.Revert();
            }
            
            onRevert?.Invoke();
        }
        
        
        /// <summary>
        /// Reverts the changes done by the tweening animation and then calls cleanup.
        /// <param name="cleanup">The cleanup action to call</param>
        /// </summary>
        public void revert(Action cleanup) {
            revert();
            cleanup?.Invoke();
        }


        /// <summary>
        /// Commits the current builder and starts a new one.
        /// </summary>
        /// <returns>The animation</returns>
        public TweeningAnimation then()
        {
            commitBuilder();
            return this;
        }

        /// <summary>
        /// Commits the current builder then creates a new builder that is executed after the specified amount of time.
        /// It is equivalent to doing then().Wait(waitForSeconds).then();
        /// </summary>
        /// <param name="waitForSeconds">The number of seconds betweens the two builders</param>
        /// <returns></returns>
        public TweeningAnimation then(float waitForSeconds)
        {
            return then().Wait(waitForSeconds).then();
        }

        /// <summary>
        /// Commits the current builder and starts a new one with the given marker.
        /// </summary>
        /// <param name="marker">The marker to add</param>
        /// <returns></returns>
        public TweeningAnimation then(string marker)
        {
            return then().Marker(marker);
        }

        /// <summary>
        /// Sets the default easing to use for all tweens added after this function is called.
        /// </summary>
        /// <param name="easing">The easing function to be used</param>
        /// <returns></returns>
        public TweeningAnimation UseEasing(Func<float, float> easing)
        {
            this.easing = easing;

            return this;
        }

        /// <summary>
        /// Sets the easing of the latest added action, if that action supports easing.
        /// </summary>
        /// <param name="easing">The easing function to be used</param>
        /// <returns></returns>
        public TweeningAnimation SetEasing(Func<float, float> easing)
        {
            if(!builder.IsEmpty())
            {
                builder.SetEasingLatest(easing);
                return this;
            }

            if (actionQueues.Count == 0) return this;

            for(int i = actionQueues.Count-1; i > -1; i++)
            {
                if (actionQueues[i].IsEmpty()) continue;
                actionQueues[i].SetEasingLatest(easing);
                return this;
            }

            return this;
        }


        /// <summary>
        /// Makes the latest action tween to target point and then back by combining the easing with its reverse easing.
        /// </summary>
        /// <param name="transitionPoint">The percentage point at which the action will reach the target point and begin to go back</param>
        /// <returns></returns>
        public TweeningAnimation ReturnBack(float transitionPoint = 0.5f)
        {
            if (!builder.IsEmpty())
            {
                builder.SetReturnBackLatest(transitionPoint);
                return this;
            }

            if (actionQueues.Count == 0) return this;

            for (int i = actionQueues.Count - 1; i > -1; i++)
            {
                if (actionQueues[i].IsEmpty()) continue;
                actionQueues[i].SetReturnBackLatest(transitionPoint);
                return this;
            }

            return this;
        }

        /// <summary>
        /// Makes the latest action tween to target point and then back by combining the easing with the specified easing.
        /// </summary>
        /// <param name="returnEasing">The easing to return with</param>
        /// <param name="transitionPoint">The percentage point at which the action will reach the target point and begin to go back</param>
        /// <returns></returns>
        public TweeningAnimation ReturnBack(Func<float,float> returnEasing, float transitionPoint = 0.5f)
        {
            if (!builder.IsEmpty())
            {
                builder.SetReturnBackLatest(returnEasing,transitionPoint);
                return this;
            }

            if (actionQueues.Count == 0) return this;

            for (int i = actionQueues.Count - 1; i > -1; i++)
            {
                if (actionQueues[i].IsEmpty()) continue;
                actionQueues[i].SetReturnBackLatest(returnEasing, transitionPoint);
                return this;
            }

            return this;
        }


        /// <summary>
        /// Sets the start value of the latest action.
        /// </summary>
        /// <param name="startValue">The start value</param>
        /// <returns></returns>
        public TweeningAnimation from<T>(T startValue)
        {
            if (!builder.IsEmpty())
            {
                builder.SetStartValueLatest(startValue);
                return this;
            }

            if (actionQueues.Count == 0) return this;

            for (int i = actionQueues.Count - 1; i > -1; i++)
            {
                if (actionQueues[i].IsEmpty()) continue;
                actionQueues[i].SetStartValueLatest(startValue);
                return this;
            }

            return this;
        }


        public TweeningAnimation from<T>(Func<T> callback)
        {
            return from(callback, out Action c);
        }

        public TweeningAnimation from<T>(Func<T> callback, out Action cleanup)
        {
            Action<AnimationTweeningAction> function = (action) =>
            {
                if (!TryGetAnimationTweeningAction<T>(action, out var animAction)) return;
                animAction.SetStartValue(callback());
            };

            CallOnActionStart(function, out Action c);

            cleanup = c;
            return this;
        }


        public TweeningAnimation to<T>(T value)
        {
            if (!TryGetLatestAddedActionAsAnimationTweeningAction<T>(out var action)) return this;
            action.endValue = value;
            return this;
        }


        public TweeningAnimation to<T>(Func<T> callback)
        {
            return to(callback, out Action c);
        }

        public TweeningAnimation to<T>(Func<T> callback, out Action cleanup)
        {
            Action<AnimationTweeningAction> function = (action) =>
            {
                if (!TryGetAnimationTweeningAction<T>(action, out var animAction)) return;
                animAction.endValue = callback();
            };

            CallOnActionStart(function, out Action c1);
            CallOnActionStopped(function, out Action c2);

            cleanup = () => { c1(); c2(); };
            return this;
        }







        /// <summary>
        /// Sets the easing function for all actions currently in the builder, that support easing.
        /// </summary>
        /// <param name="easing">The easing function to be used</param>
        /// <returns></returns>
        public TweeningAnimation SetEasingBuilder(Func<float, float> easing)
        {
            builder.SetEasingAll(easing);

            return this;
        }

        /// <summary>
        /// Sets the easing of all action currently added to the animation, that support easing.
        /// </summary>
        /// <param name="easing">The easing function to be used</param>
        /// <returns></returns>
        public TweeningAnimation SetEasingAll(Func<float, float> easing)
        {
            foreach (TweeningSequence seq in actionQueues)
            {
                seq.SetEasingAll(easing);
            }


            builder.SetEasingAll(easing);

            return this;
        }


        protected ITweeningAction ApplyDefaultEasing(ITweeningAction action)
        {
            if (action is ITweener)
            {
                ((ITweener)action).easing = easing;
            }

            return action;
        }

        protected void AddActionToBuilder(ITweeningAction action)
        {
            builder.Add(ApplyDefaultEasing(action));
        }

        protected void InsertActionToBuilder(ITweeningAction action, int index)
        {
            builder.Insert(0, ApplyDefaultEasing(action));
        }

        public ITweeningAction GetLatestAddedAction()
        {
            if (!builder.IsEmpty()) return builder.latestBuildAction;

            for (int i = actionQueues.Count - 1; i > -1; i++)
            {
                if (actionQueues[i].IsEmpty()) continue;
                return actionQueues[i].latestBuildAction;
            }

            return null;
        }

        public bool TryGetLatestAddedAction(out ITweeningAction action)
        {
            ITweeningAction latest = GetLatestAddedAction();
            action = latest;
            return latest == null;
        }

        public AnimationTweeningAction GetLatestAddedActionAsAnimationTweeningAction()
        {
            ITweeningAction latest = GetLatestAddedAction();
            if (latest is AnimationTweeningAction) return (AnimationTweeningAction)latest;

            return null;
        }

        

        public bool TryGetLatestAddedActionAsAnimationTweeningAction(out AnimationTweeningAction action)
        {
            AnimationTweeningAction latest = GetLatestAddedActionAsAnimationTweeningAction();
            action = latest;
            return latest != null;
        }

        public AnimationTweeningAction<T> GetLatestAddedActionAsAnimationTweeningAction<T>()
        {
            ITweeningAction latest = GetLatestAddedAction();
            if (latest is AnimationTweeningAction<T>) return (AnimationTweeningAction<T>)latest;

            return null;
        }

        public bool TryGetLatestAddedActionAsAnimationTweeningAction<T>(out AnimationTweeningAction<T> action)
        {
            AnimationTweeningAction<T> latest = GetLatestAddedActionAsAnimationTweeningAction<T>();
            action = latest;
            return latest != null;
        }

        public TweeningAnimation ForceOne(bool force = true)
        {
            ITweeningAction a = GetLatestAddedAction();
            if (!(a is ITweener)) return this;

            ((ITweener)a).forceOneAtEnd = force;
            return this;
        }


        public bool TryGetAnimationTweeningAction(ITweeningAction action, out AnimationTweeningAction result)
        {
            result = null;
            if (!(action is AnimationTweeningAction)) return false;

            result = (AnimationTweeningAction)action;
            return true;
        }

        public bool TryGetAnimationTweeningAction<T>(ITweeningAction action, out AnimationTweeningAction<T> result)
        {
            result = null;
            if (!(action is AnimationTweeningAction<T>)) return false;

            result = (AnimationTweeningAction<T>)action;
            return true;
        }

        #region AnimationTweeningAction callbacks
        public TweeningAnimation CallOnActionStart(Action<AnimationTweeningAction> callback, out Action cleanup)
        {
            cleanup = null;
            AnimationTweeningAction latest = GetLatestAddedActionAsAnimationTweeningAction();
            if (latest == null) return this;

            void cb(AnimationTweeningAction a)
            {
                callback(a);
            }

            //Might not be the best implementation
            latest.onTweenStarted += cb;

            cleanup = () => { latest.onTweenStarted += cb; };
            return this;
        }

        public TweeningAnimation CallOnActionEnd(Action<AnimationTweeningAction> callback, out Action cleanup)
        {
            cleanup = null;
            AnimationTweeningAction latest = GetLatestAddedActionAsAnimationTweeningAction();
            if (latest == null) return this;

            void cb(AnimationTweeningAction a)
            {
                callback(a);
            }

            //Might not be the best implementation
            latest.onTweenEnded += cb;

            cleanup = () => { latest.onTweenEnded += cb; };
            return this;
        }

        public TweeningAnimation CallOnActionStopped(Action<AnimationTweeningAction> callback, out Action cleanup)
        {
            cleanup = null;
            AnimationTweeningAction latest = GetLatestAddedActionAsAnimationTweeningAction();
            if (latest == null) return this;

            void cb(AnimationTweeningAction a)
            {
                callback(a);
            }

            //Might not be the best implementation
            latest.onTweenStopped += cb;

            cleanup = () => { latest.onTweenStopped += cb; };
            return this;
        }

        public TweeningAnimation CallOnActionReverted(Action<AnimationTweeningAction> callback, out Action cleanup)
        {
            cleanup = null;
            AnimationTweeningAction latest = GetLatestAddedActionAsAnimationTweeningAction();
            if (latest == null) return this;

            void cb(AnimationTweeningAction a)
            {
                callback(a);
            }

            //Might not be the best implementation
            latest.onTweenReverted += cb;

            cleanup = () => { latest.onTweenReverted += cb; };
            return this;
        }

        public TweeningAnimation CallOnActionForceFinished(Action<AnimationTweeningAction> callback, out Action cleanup)
        {
            cleanup = null;
            AnimationTweeningAction latest = GetLatestAddedActionAsAnimationTweeningAction();
            if (latest == null) return this;

            void cb(AnimationTweeningAction a)
            {
                callback(a);
            }

            //Might not be the best implementation
            latest.onTweenForceFinished += cb;

            cleanup = () => { latest.onTweenForceFinished += cb; };
            return this;
        }

        #endregion





        #region Tweeners


        #region Position
        public TweeningAnimation move(float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return move(gameObject, duration, startDelay, endDelay);
        }

        public TweeningAnimation move(GameObject gameObject, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return move(gameObject, Vector3.zero, duration, startDelay, endDelay);
        }




        /// <summary>
        /// Adds a movement action that moves the animation's gameobject to the specified position.
        /// </summary>
        /// <param name="targetPosition">The position to move to</param>
        /// <param name="duration">Duration for the movement</param>
        /// <param name="startDelay">Delay before the movement starts. Default is 0</param>
        /// <param name="endDelay">Delay after the movement is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation move(Vector3 targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return move(gameObject, targetPosition, duration, startDelay, endDelay);
        }

        /// <summary>
        /// Adds a movement action that moves the specified gameobject to the specified position.
        /// </summary>
        /// <param name="gameObject">The GameObject to move</param>
        /// <param name="targetPosition">The position to move to</param>
        /// <param name="duration">Duration for the movement</param>
        /// <param name="startDelay">Delay before the movement starts. Default is 0</param>
        /// <param name="endDelay">Delay after the movement is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation move(GameObject gameObject, Vector3 targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            if (gameObject == null) return this;
            PositionTweener action = new PositionTweener(builder.tweenDone, source, gameObject, targetPosition, duration, startDelay, endDelay);

            AddActionToBuilder(action);
            return this;
        }

        /// <summary>
        /// Adds a movement action that moves the animation's gameobject to the specified end position from the speicifed start poistion.
        /// </summary>
        /// <param name="startPosition">The start position</param>
        /// <param name="targetPosition">The position to move to</param>
        /// <param name="duration">Duration for the movement</param>
        /// <param name="startDelay">Delay before the movement starts. Default is 0</param>
        /// <param name="endDelay">Delay after the movement is done, before it is marked as finished. Default is 0</param>
        public TweeningAnimation move(Vector3 startPosition, Vector3 targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return move(gameObject, startPosition, targetPosition, duration, startDelay, endDelay);
        }


        /// <summary>
        /// Adds a movement action that moves the specified gameobject from the specified start poisition to the specified end position.
        /// </summary>
        /// <param name="gameObject">The GameObject to move</param>
        /// <param name="startPosition">The start position</param>
        /// <param name="targetPosition">The position to move to</param>
        /// <param name="duration">Duration for the movement</param>
        /// <param name="startDelay">Delay before the movement starts. Default is 0</param>
        /// <param name="endDelay">Delay after the movement is done, before it is marked as finished. Default is 0</param>
        public TweeningAnimation move(GameObject gameObject, Vector3 startPosition, Vector3 targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            move(gameObject, targetPosition, duration, startDelay, endDelay);
            return from(startPosition);
        }


        #region Endvalue callback
        public TweeningAnimation move(GameObject gameObject, Func<Vector3> targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            move(gameObject, Vector3.zero, duration, startDelay, endDelay);
            return to(targetPosition);
        }

        public TweeningAnimation move(Func<Vector3> targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
           return move(gameObject, targetPosition, duration, startDelay, endDelay);
        }


        public TweeningAnimation move(GameObject gameObject, Vector3 startPosition, Func<Vector3> targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            move(gameObject, targetPosition, duration, startDelay, endDelay);
            return from(startPosition);
        }

        public TweeningAnimation move(Vector3 startPosition, Func<Vector3> targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return move(gameObject, startPosition, targetPosition, duration, startDelay, endDelay);
        }
        #endregion

        #region Start value callbacks
        public TweeningAnimation move(GameObject gameObject, Func<Vector3> startPosition, Vector3 targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            move(gameObject, targetPosition, duration, startDelay, endDelay);
            return from(startPosition);
        }

        public TweeningAnimation move(Func<Vector3> startPosition, Vector3 targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return move(gameObject, startPosition, targetPosition, duration, startDelay, endDelay);
        }
        #endregion

        #region Start and end callback
        public TweeningAnimation move(GameObject gameObject, Func<Vector3> startPosition, Func<Vector3> targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            move(gameObject, targetPosition, duration, startDelay, endDelay);
            return from(startPosition);
        }

        public TweeningAnimation move(Func<Vector3> startPosition, Func<Vector3> targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return move(gameObject, startPosition, targetPosition, duration, startDelay, endDelay);
        }

        #endregion


        #endregion


        #region Anchor position

        public TweeningAnimation anchorMove(float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return anchorMove(gameObject, duration, startDelay, endDelay);
        }

        public TweeningAnimation anchorMove(GameObject gameObject, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return anchorMove(gameObject, Vector3.zero, duration, startDelay, endDelay);
        }




        /// <summary>
        /// Adds a anchorMovement action that anchorMoves the animation's gameobject to the specified position.
        /// </summary>
        /// <param name="targetPosition">The position to anchorMove to</param>
        /// <param name="duration">Duration for the anchorMovement</param>
        /// <param name="startDelay">Delay before the anchorMovement starts. Default is 0</param>
        /// <param name="endDelay">Delay after the anchorMovement is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation anchorMove(Vector3 targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return anchorMove(gameObject, targetPosition, duration, startDelay, endDelay);
        }

        /// <summary>
        /// Adds a anchorMovement action that anchorMoves the specified gameobject to the specified position.
        /// </summary>
        /// <param name="gameObject">The GameObject to anchorMove</param>
        /// <param name="targetPosition">The position to anchorMove to</param>
        /// <param name="duration">Duration for the anchorMovement</param>
        /// <param name="startDelay">Delay before the anchorMovement starts. Default is 0</param>
        /// <param name="endDelay">Delay after the anchorMovement is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation anchorMove(GameObject gameObject, Vector3 targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            if (gameObject == null) return this;
            AnchoredPositionTweener action = new AnchoredPositionTweener(builder.tweenDone, source, gameObject, targetPosition, duration, startDelay, endDelay);

            AddActionToBuilder(action);
            return this;
        }

        /// <summary>
        /// Adds a anchorMovement action that anchorMoves the animation's gameobject to the specified end position from the speicifed start poistion.
        /// </summary>
        /// <param name="startPosition">The start position</param>
        /// <param name="targetPosition">The position to anchorMove to</param>
        /// <param name="duration">Duration for the anchorMovement</param>
        /// <param name="startDelay">Delay before the anchorMovement starts. Default is 0</param>
        /// <param name="endDelay">Delay after the anchorMovement is done, before it is marked as finished. Default is 0</param>
        public TweeningAnimation anchorMove(Vector3 startPosition, Vector3 targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return anchorMove(gameObject, startPosition, targetPosition, duration, startDelay, endDelay);
        }


        /// <summary>
        /// Adds a anchorMovement action that anchorMoves the specified gameobject from the specified start poisition to the specified end position.
        /// </summary>
        /// <param name="gameObject">The GameObject to anchorMove</param>
        /// <param name="startPosition">The start position</param>
        /// <param name="targetPosition">The position to anchorMove to</param>
        /// <param name="duration">Duration for the anchorMovement</param>
        /// <param name="startDelay">Delay before the anchorMovement starts. Default is 0</param>
        /// <param name="endDelay">Delay after the anchorMovement is done, before it is marked as finished. Default is 0</param>
        public TweeningAnimation anchorMove(GameObject gameObject, Vector3 startPosition, Vector3 targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            anchorMove(gameObject, targetPosition, duration, startDelay, endDelay);
            return from(startPosition);
        }


        #region Endvalue callback
        public TweeningAnimation anchorMove(GameObject gameObject, Func<Vector3> targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            anchorMove(gameObject, Vector3.zero, duration, startDelay, endDelay);
            return to(targetPosition);
        }

        public TweeningAnimation anchorMove(Func<Vector3> targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return anchorMove(gameObject, targetPosition, duration, startDelay, endDelay);
        }


        public TweeningAnimation anchorMove(GameObject gameObject, Vector3 startPosition, Func<Vector3> targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            anchorMove(gameObject, targetPosition, duration, startDelay, endDelay);
            return from(startPosition);
        }

        public TweeningAnimation anchorMove(Vector3 startPosition, Func<Vector3> targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return anchorMove(gameObject, startPosition, targetPosition, duration, startDelay, endDelay);
        }
        #endregion

        #region Start value callbacks
        public TweeningAnimation anchorMove(GameObject gameObject, Func<Vector3> startPosition, Vector3 targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            anchorMove(gameObject, targetPosition, duration, startDelay, endDelay);
            return from(startPosition);
        }

        public TweeningAnimation anchorMove(Func<Vector3> startPosition, Vector3 targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return anchorMove(gameObject, startPosition, targetPosition, duration, startDelay, endDelay);
        }
        #endregion

        #region Start and end callback
        public TweeningAnimation anchorMove(GameObject gameObject, Func<Vector3> startPosition, Func<Vector3> targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            anchorMove(gameObject, targetPosition, duration, startDelay, endDelay);
            return from(startPosition);
        }

        public TweeningAnimation anchorMove(Func<Vector3> startPosition, Func<Vector3> targetPosition, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return anchorMove(gameObject, startPosition, targetPosition, duration, startDelay, endDelay);
        }




        #endregion
        #endregion


        #region Mesh Color

        public TweeningAnimation colorMesh(float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return colorMesh(gameObject, duration, startDelay, endDelay);
        }

        public TweeningAnimation colorMesh(GameObject gameObject, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return colorMesh(gameObject, Color.black, duration, startDelay, endDelay);
        }



        /// <summary>
        /// Adds a color change action that changes the animation's gameobject to the specified color.
        /// </summary>
        /// <param name="targetColor">The color to change to</param>
        /// <param name="duration">Duration for the change</param>
        /// <param name="startDelay">Delay before the change starts. Default is 0</param>
        /// <param name="endDelay">Delay after the change is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation colorMesh(Color targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return colorMesh(gameObject, targetColor, duration, startDelay, endDelay);
        }

        /// <summary>
        /// Adds a color change action that changes the specified gameobject to the specified color.
        /// </summary>
        /// <param name="gameObject">The GameObject whose color will be changed</param>
        /// <param name="targetColor">The color to change to</param>
        /// <param name="duration">Duration for the change</param>
        /// <param name="startDelay">Delay before the change starts. Default is 0</param>
        /// <param name="endDelay">Delay after the change is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation colorMesh(GameObject gameObject, Color targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            if (gameObject == null) return this;
            MeshColorTweener action = new MeshColorTweener(builder.tweenDone, source, gameObject, targetColor, duration, startDelay, endDelay);

            AddActionToBuilder(action);
            return this;
        }

        /// <summary>
        /// Adds a color change action that changes the animation's gameobject to the specified color.
        /// </summary>
        /// <param name="startColor">The color to change from</param>
        /// <param name="targetColor">The color to change to</param>
        /// <param name="duration">Duration for the change</param>
        /// <param name="startDelay">Delay before the change starts. Default is 0</param>
        /// <param name="endDelay">Delay after the change is done, before it is marked as finished. Default is 0</param>
        public TweeningAnimation colorMesh(Color startColor, Color targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return (colorMesh(gameObject, startColor, targetColor, duration, startDelay, endDelay));
        }

        /// <summary>
        /// Adds a color change action that changes the specified gameobject to the specified color from the specified start color.
        /// </summary>
        /// <param name="gameObject">The GameObject whose color will be changed</param>
        /// <param name="startColor">The color to change from</param>
        /// <param name="targetColor">The color to change to</param>
        /// <param name="duration">Duration for the change</param>
        /// <param name="startDelay">Delay before the change starts. Default is 0</param>
        /// <param name="endDelay">Delay after the change is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation colorMesh(GameObject gameObject, Color startColor, Color targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            colorMesh(gameObject, targetColor, duration, startDelay, endDelay);
            return from(startColor);
        }


        #region Endvalue callback
        public TweeningAnimation colorMesh(GameObject gameObject, Func<Color> targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            colorMesh(gameObject, Color.black, duration, startDelay, endDelay);
            return to(targetColor);
        }

        public TweeningAnimation colorMesh(Func<Color> targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return colorMesh(gameObject, targetColor, duration, startDelay, endDelay);
        }


        public TweeningAnimation colorMesh(GameObject gameObject, Color startColor, Func<Color> targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            colorMesh(gameObject, targetColor, duration, startDelay, endDelay);
            return from(startColor);
        }

        public TweeningAnimation colorMesh(Color startColor, Func<Color> targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return colorMesh(gameObject, startColor, targetColor, duration, startDelay, endDelay);
        }
        #endregion

        #region Start value callbacks
        public TweeningAnimation colorMesh(GameObject gameObject, Func<Color> startColor, Color targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            colorMesh(gameObject, targetColor, duration, startDelay, endDelay);
            return from(startColor);
        }

        public TweeningAnimation colorMesh(Func<Color> startColor, Color targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return colorMesh(gameObject, startColor, targetColor, duration, startDelay, endDelay);
        }
        #endregion

        #region Start and end callback
        public TweeningAnimation colorMesh(GameObject gameObject, Func<Color> startColor, Func<Color> targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            colorMesh(gameObject, targetColor, duration, startDelay, endDelay);
            return from(startColor);
        }

        public TweeningAnimation colorMesh(Func<Color> startColor, Func<Color> targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return colorMesh(gameObject, startColor, targetColor, duration, startDelay, endDelay);
        }

        #endregion

        #endregion

        #region Sprite color

        public TweeningAnimation colorSprite(float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return colorSprite(gameObject, duration, startDelay, endDelay);
        }

        public TweeningAnimation colorSprite(GameObject gameObject, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return colorSprite(gameObject, Color.black, duration, startDelay, endDelay);
        }


        /// <summary>
        /// Adds a color change action that changes the animation's gameobject to the specified color.
        /// </summary>
        /// <param name="targetColor">The color to change to</param>
        /// <param name="duration">Duration for the change</param>
        /// <param name="startDelay">Delay before the change starts. Default is 0</param>
        /// <param name="endDelay">Delay after the change is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation colorSprite(Color targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return colorSprite(gameObject, targetColor, duration, startDelay, endDelay);
        }

        /// <summary>
        /// Adds a color change action that changes the specified gameobject to the specified color.
        /// </summary>
        /// <param name="gameObject">The GameObject whose color will be changed</param>
        /// <param name="targetColor">The color to change to</param>
        /// <param name="duration">Duration for the change</param>
        /// <param name="startDelay">Delay before the change starts. Default is 0</param>
        /// <param name="endDelay">Delay after the change is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation colorSprite(GameObject gameObject, Color targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            if (gameObject == null) return this;
            SpriteColorTweener action = new SpriteColorTweener(builder.tweenDone, source, gameObject, targetColor, duration, startDelay, endDelay);

            AddActionToBuilder(action);
            return this;
        }

        /// <summary>
        /// Adds a color change action that changes the animation's gameobject to the specified color.
        /// </summary>
        /// <param name="startColor">The color to change from</param>
        /// <param name="targetColor">The color to change to</param>
        /// <param name="duration">Duration for the change</param>
        /// <param name="startDelay">Delay before the change starts. Default is 0</param>
        /// <param name="endDelay">Delay after the change is done, before it is marked as finished. Default is 0</param>
        public TweeningAnimation colorSprite(Color startColor, Color targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return (colorSprite(gameObject, startColor, targetColor, duration, startDelay, endDelay));
        }

        /// <summary>
        /// Adds a color change action that changes the specified gameobject to the specified color from the specified start color.
        /// </summary>
        /// <param name="gameObject">The GameObject whose color will be changed</param>
        /// <param name="startColor">The color to change from</param>
        /// <param name="targetColor">The color to change to</param>
        /// <param name="duration">Duration for the change</param>
        /// <param name="startDelay">Delay before the change starts. Default is 0</param>
        /// <param name="endDelay">Delay after the change is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation colorSprite(GameObject gameObject, Color startColor, Color targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            colorSprite(gameObject, targetColor, duration, startDelay, endDelay);
            return from(startColor);
        }



        #region Endvalue callback
        public TweeningAnimation colorSprite(GameObject gameObject, Func<Color> targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            colorSprite(gameObject, Color.black, duration, startDelay, endDelay);
            return to(targetColor);
        }

        public TweeningAnimation colorSprite(Func<Color> targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return colorSprite(gameObject, targetColor, duration, startDelay, endDelay);
        }


        public TweeningAnimation colorSprite(GameObject gameObject, Color startColor, Func<Color> targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            colorSprite(gameObject, targetColor, duration, startDelay, endDelay);
            return from(startColor);
        }

        public TweeningAnimation colorSprite(Color startColor, Func<Color> targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return colorSprite(gameObject, startColor, targetColor, duration, startDelay, endDelay);
        }
        #endregion

        #region Start value callbacks
        public TweeningAnimation colorSprite(GameObject gameObject, Func<Color> startColor, Color targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            colorSprite(gameObject, targetColor, duration, startDelay, endDelay);
            return from(startColor);
        }

        public TweeningAnimation colorSprite(Func<Color> startColor, Color targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return colorSprite(gameObject, startColor, targetColor, duration, startDelay, endDelay);
        }
        #endregion

        #region Start and end callback
        public TweeningAnimation colorSprite(GameObject gameObject, Func<Color> startColor, Func<Color> targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            colorSprite(gameObject, targetColor, duration, startDelay, endDelay);
            return from(startColor);
        }

        public TweeningAnimation colorSprite(Func<Color> startColor, Func<Color> targetColor, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return colorSprite(gameObject, startColor, targetColor, duration, startDelay, endDelay);
        }

        #endregion




        #endregion


        #region Rotation


        public TweeningAnimation rotate(float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return rotate(gameObject, duration, startDelay, endDelay);
        }

        public TweeningAnimation rotate(GameObject gameObject, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return rotate(gameObject, Quaternion.identity, duration, startDelay, endDelay);
        }



        /// <summary>
        /// Adds a rotation action that rotates the animation's gameobject to the specified rotation.
        /// </summary>
        /// <param name="targetRotation">The rotation to rotate to</param>
        /// <param name="duration">Duration for the rotation</param>
        /// <param name="startDelay">Delay before the rotation starts. Default is 0</param>
        /// <param name="endDelay">Delay after the rotation is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation rotate(Quaternion targetRotation, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return rotate(gameObject, targetRotation, duration, startDelay, endDelay);
        }

        /// <summary>
        /// Adds a rotation action that rotates the specified gameobject to the specified rotation.
        /// </summary>
        /// <param name="gameObject">The gameobject to rotate.</param>
        /// <param name="targetRotation">The rotation to rotate to</param>
        /// <param name="duration">Duration for the rotation</param>
        /// <param name="startDelay">Delay before the rotation starts. Default is 0</param>
        /// <param name="endDelay">Delay after the rotation is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation rotate(GameObject gameObject, Quaternion targetRotation, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            if (gameObject == null) return this;
            RotationTweener action = new RotationTweener(builder.tweenDone, source, gameObject, targetRotation, duration, startDelay, endDelay);

            AddActionToBuilder(action);
            return this;
        }

        /// <summary>
        /// Adds a rotation action that rotates the animation's gameobject to the specified rotation from the specified start rotation.
        /// </summary>
        /// <param name="startRotation">The rotation to rotate from</param>
        /// <param name="targetRotation">The rotation to rotate to</param>
        /// <param name="duration">Duration for the rotation</param>
        /// <param name="startDelay">Delay before the rotation starts. Default is 0</param>
        /// <param name="endDelay">Delay after the rotation is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation rotate(Quaternion startRotation, Quaternion targetRotation, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return rotate(gameObject, startRotation, targetRotation, duration, startDelay, endDelay);
        }

        /// <summary>
        /// Adds a rotation action that rotates the specified gameobject to the specified rotation from the specified start rotation.
        /// </summary>
        /// <param name="gameObject">The gameobject to rotate.</param>
        /// <param name="startRotation">The rotation to rotate from</param>
        /// <param name="targetRotation">The rotation to rotate to</param>
        /// <param name="duration">Duration for the rotation</param>
        /// <param name="startDelay">Delay before the rotation starts. Default is 0</param>
        /// <param name="endDelay">Delay after the rotation is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation rotate(GameObject gameObject, Quaternion startRotation, Quaternion targetRotation, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            rotate(gameObject, targetRotation, duration, startDelay, endDelay);
            return from(startRotation);
        }

        #region Endvalue callback
        public TweeningAnimation rotate(GameObject gameObject, Func<Quaternion> targetRotation, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            rotate(gameObject, Quaternion.identity, duration, startDelay, endDelay);
            return to(targetRotation);
        }

        public TweeningAnimation rotate(Func<Quaternion> targetRotation, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return rotate(gameObject, targetRotation, duration, startDelay, endDelay);
        }


        public TweeningAnimation rotate(GameObject gameObject, Quaternion startRotation, Func<Quaternion> targetRotation, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            rotate(gameObject, targetRotation, duration, startDelay, endDelay);
            return from(startRotation);
        }

        public TweeningAnimation rotate(Quaternion startRotation, Func<Quaternion> targetRotation, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return rotate(gameObject, startRotation, targetRotation, duration, startDelay, endDelay);
        }
        #endregion

        #region Start value callbacks
        public TweeningAnimation rotate(GameObject gameObject, Func<Quaternion> startRotation, Quaternion targetRotation, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            rotate(gameObject, targetRotation, duration, startDelay, endDelay);
            return from(startRotation);
        }

        public TweeningAnimation rotate(Func<Quaternion> startRotation, Quaternion targetRotation, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return rotate(gameObject, startRotation, targetRotation, duration, startDelay, endDelay);
        }
        #endregion

        #region Start and end callback
        public TweeningAnimation rotate(GameObject gameObject, Func<Quaternion> startRotation, Func<Quaternion> targetRotation, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            rotate(gameObject, targetRotation, duration, startDelay, endDelay);
            return from(startRotation);
        }

        public TweeningAnimation rotate(Func<Quaternion> startRotation, Func<Quaternion> targetRotation, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return rotate(gameObject, startRotation, targetRotation, duration, startDelay, endDelay);
        }

        #endregion







        #endregion

        #region Scaling


        public TweeningAnimation scale(float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return scale(gameObject, duration, startDelay, endDelay);
        }

        public TweeningAnimation scale(GameObject gameObject, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return scale(gameObject, Vector3.zero, duration, startDelay, endDelay);
        }


        /// <summary>
        /// Adds a scale action that scales the animation's gameobject to the specified scale.
        /// </summary>
        /// <param name="targetScale">The scale to scale to</param>
        /// <param name="duration">Duration for the scaling</param>
        /// <param name="startDelay">Delay before the scaling starts. Default is 0</param>
        /// <param name="endDelay">Delay after the scaling is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation scale(Vector3 targetScale, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return scale(gameObject, targetScale, duration, startDelay, endDelay);
        }

        /// <summary>
        /// Adds a scale action that scales the specified gameobject to the specified scale.
        /// </summary>
        /// <param name="gameObject">The game object to scale.</param>
        /// <param name="targetScale">The scale to scale to</param>
        /// <param name="duration">Duration for the scaling</param>
        /// <param name="startDelay">Delay before the scaling starts. Default is 0</param>
        /// <param name="endDelay">Delay after the scaling is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation scale(GameObject gameObject, Vector3 targetScale, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            if (gameObject == null) return this;
            ScaleTweener action = new ScaleTweener(builder.tweenDone, source, gameObject, targetScale, duration, startDelay, endDelay);


            
            AddActionToBuilder(action);
            return this;
        }



        /// <summary>
        /// Adds a scale action that scales the animation's gameobject to the specified scale from the specified start scale.
        /// </summary>
        /// <param name="startScale">The scale to scale from</param>
        /// <param name="targetScale">The scale to scale to</param>
        /// <param name="duration">Duration for the scaling</param>
        /// <param name="startDelay">Delay before the scaling starts. Default is 0</param>
        /// <param name="endDelay">Delay after the scaling is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation scale(Vector3 startScale, Vector3 targetScale, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return scale(gameObject, startScale, targetScale, duration, startDelay, endDelay);
        }



        /// <summary>
        /// Adds a scale action that scales the specified gameobject to the specified scale from the specified start scale.
        /// </summary>
        /// <param name="gameObject">The game object to scale.</param>
        /// <param name="startScale">The scale to scale from</param>
        /// <param name="targetScale">The scale to scale to</param>
        /// <param name="duration">Duration for the scaling</param>
        /// <param name="startDelay">Delay before the scaling starts. Default is 0</param>
        /// <param name="endDelay">Delay after the scaling is done, before it is marked as finished. Default is 0</param>
        public TweeningAnimation scale(GameObject gameObject, Vector3 startScale, Vector3 targetScale, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            scale(gameObject, targetScale, duration, startDelay, endDelay);
            return from(startScale);
        }




        #region Endvalue callback
        public TweeningAnimation scale(GameObject gameObject, Func<Vector3> targetScale, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            scale(gameObject, Vector3.zero, duration, startDelay, endDelay);
            return to(targetScale);
        }

        public TweeningAnimation scale(Func<Vector3> targetScale, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return scale(gameObject, targetScale, duration, startDelay, endDelay);
        }


        public TweeningAnimation scale(GameObject gameObject, Vector3 startScale, Func<Vector3> targetScale, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            scale(gameObject, targetScale, duration, startDelay, endDelay);
            return from(startScale);
        }

        public TweeningAnimation scale(Vector3 startScale, Func<Vector3> targetScale, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return scale(gameObject, startScale, targetScale, duration, startDelay, endDelay);
        }
        #endregion

        #region Start value callbacks
        public TweeningAnimation scale(GameObject gameObject, Func<Vector3> startScale, Vector3 targetScale, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            scale(gameObject, targetScale, duration, startDelay, endDelay);
            return from(startScale);
        }

        public TweeningAnimation scale(Func<Vector3> startScale, Vector3 targetScale, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return scale(gameObject, startScale, targetScale, duration, startDelay, endDelay);
        }
        #endregion

        #region Start and end callback
        public TweeningAnimation scale(GameObject gameObject, Func<Vector3> startScale, Func<Vector3> targetScale, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            scale(gameObject, targetScale, duration, startDelay, endDelay);
            return from(startScale);
        }

        public TweeningAnimation scale(Func<Vector3> startScale, Func<Vector3> targetScale, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            return scale(gameObject, startScale, targetScale, duration, startDelay, endDelay);
        }

        #endregion




        #endregion



        #region Callbacks
        /// <summary>
        /// Adds a vector3 callback animation that calls the given callback function every frame with the current value.
        /// </summary>
        /// <param name="start">The start value</param>
        /// <param name="end">The end value</param>
        /// <param name="callback">The callback function. The input of this function will be the current value of the animation</param>
        /// <param name="duration">Duration for the scaling</param>
        /// <param name="startDelay">Delay before the scaling starts. Default is 0</param>
        /// <param name="endDelay">Delay after the scaling is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation vector3Callback(Vector3 start, Vector3 end, Action<Vector3> callback, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            Vector3TweeningActionCallback action = new Vector3TweeningActionCallback(builder.tweenDone, source, gameObject, start, end, callback, duration, startDelay, endDelay);

            AddActionToBuilder(action);
            return this;
        }

        /// <summary>
        /// Adds a vector2 callback animation that calls the given callback function every frame with the current value.
        /// </summary>
        /// <param name="start">The start value</param>
        /// <param name="end">The end value</param>
        /// <param name="callback">The callback function. The input of this function will be the current value of the animation</param>
        /// <param name="duration">Duration for the scaling</param>
        /// <param name="startDelay">Delay before the scaling starts. Default is 0</param>
        /// <param name="endDelay">Delay after the scaling is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation vector2Callback(Vector2 start, Vector2 end, Action<Vector2> callback, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            Vector2TweeningActionCallback action = new Vector2TweeningActionCallback(builder.tweenDone, source, gameObject, start, end, callback, duration, startDelay, endDelay);

            AddActionToBuilder(action);
            return this;
        }

        /// <summary>
        /// Adds a float callback animation that calls the given callback function every frame with the current value.
        /// </summary>
        /// <param name="start">The start value</param>
        /// <param name="end">The end value</param>
        /// <param name="callback">The callback function. The input of this function will be the current value of the animation</param>
        /// <param name="duration">Duration for the scaling</param>
        /// <param name="startDelay">Delay before the scaling starts. Default is 0</param>
        /// <param name="endDelay">Delay after the scaling is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation floatCallback(float start, float end, Action<float> callback, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            FloatTweeningActionCallback action = new FloatTweeningActionCallback(builder.tweenDone, source, gameObject, start, end, callback, duration, startDelay, endDelay);

            AddActionToBuilder(action);
            return this;
        }

        /// <summary>
        /// Adds a int callback animation that calls the given callback function every frame with the current value.
        /// </summary>
        /// <param name="start">The start value</param>
        /// <param name="end">The end value</param>
        /// <param name="callback">The callback function. The input of this function will be the current value of the animation</param>
        /// <param name="duration">Duration for the scaling</param>
        /// <param name="startDelay">Delay before the scaling starts. Default is 0</param>
        /// <param name="endDelay">Delay after the scaling is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation intCallback(int start, int end, Action<int> callback, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            IntTweeningActionCallback action = new IntTweeningActionCallback(builder.tweenDone, source, gameObject, start, end, callback, duration, startDelay, endDelay);

            AddActionToBuilder(action);
            return this;
        }

        /// <summary>
        /// Adds a quaternion callback animation that calls the given callback function every frame with the current value.
        /// </summary>
        /// <param name="start">The start value</param>
        /// <param name="end">The end value</param>
        /// <param name="callback">The callback function. The input of this function will be the current value of the animation</param>
        /// <param name="duration">Duration for the scaling</param>
        /// <param name="startDelay">Delay before the scaling starts. Default is 0</param>
        /// <param name="endDelay">Delay after the scaling is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation QuaternionCallback(Quaternion start, Quaternion end, Action<Quaternion> callback, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            QuaternionTweeningActionCallback action = new QuaternionTweeningActionCallback(builder.tweenDone, source, gameObject, start, end, callback, duration, startDelay, endDelay);

            AddActionToBuilder(action);
            return this;
        }

        /// <summary>
        /// Adds a string callback animation that calls the given callback function every frame with the current value.
        /// </summary>
        /// <param name="start">The start value</param>
        /// <param name="end">The end value</param>
        /// <param name="callback">The callback function. The input of this function will be the current value of the animation</param>
        /// <param name="duration">Duration for the scaling</param>
        /// <param name="startDelay">Delay before the scaling starts. Default is 0</param>
        /// <param name="endDelay">Delay after the scaling is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation stringCallback(string start, string end, Action<string> callback, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            StringTweeningActionCallback action = new StringTweeningActionCallback(builder.tweenDone, source, gameObject, start, end, callback, duration, startDelay, endDelay);


            AddActionToBuilder(action);
            return this;
        }

        /// <summary>
        /// Adds a color callback animation that calls the given callback function every frame with the current value.
        /// </summary>
        /// <param name="start">The start value</param>
        /// <param name="end">The end value</param>
        /// <param name="callback">The callback function. The input of this function will be the current value of the animation</param>
        /// <param name="duration">Duration for the scaling</param>
        /// <param name="startDelay">Delay before the scaling starts. Default is 0</param>
        /// <param name="endDelay">Delay after the scaling is done, before it is marked as finished. Default is 0</param>
        /// <returns></returns>
        public TweeningAnimation colorCallback(Color start, Color end, Action<Color> callback, float duration, float startDelay = 0f, float endDelay = 0f)
        {
            ColorTweeningActionCallback action = new ColorTweeningActionCallback(builder.tweenDone, source, gameObject, start, end, callback, duration, startDelay, endDelay);

            AddActionToBuilder(action);
            return this;
        }
        #endregion

        /// <summary>
        /// Adds an tweening action the calls the given callback.
        /// </summary>
        /// <param name="action">The callback to be executed</param>
        /// <returns></returns>
        public TweeningAnimation call(Action action)
        {
            CallBackIntermediaryAction tweeningaction = new CallBackIntermediaryAction(action);

            AddActionToBuilder(tweeningaction);
            return this;
        }

        /// <summary>
        /// Adds a tweening action that waits for the specified amount of time.
        /// </summary>
        /// <param name="duration">The duration to wait</param>
        /// <returns></returns>
        public TweeningAnimation Wait(float duration)
        {
            WaitForTime action = new WaitForTime(builder.tweenDone, source, gameObject, duration);

            AddActionToBuilder(action);

            return this;
        }

        #endregion


        #region Looping and reversing and such

        #region Markers
        /// <summary>
        /// Adds a marker to the current builder
        /// </summary>
        /// <param name="marker">The marker to add</param>
        /// <returns></returns>
        public TweeningAnimation Marker(string marker)
        {
            builder.AddMarker(marker);

            return this;
        }


        /// <summary>
        /// Checks if the action queue with the specified queue number has the specified marker 
        /// </summary>
        /// <param name="marker">The marker to check for</param>
        /// <param name="queueNumber">The queue to check in</param>
        /// <returns>True if the queue contains the marker, otherwise false</returns>
        public bool HasMarker(string marker, int queueNumber)
        {
            if (queueNumber >= actionQueues.Count) return false;
            return actionQueues[queueNumber].HasMarker(marker);
        }

        /// <summary>
        /// Checks if builder has the specified marker 
        /// </summary>
        /// <param name="marker">The marker to check for</param>
        /// <returns>True if the builder contains the marker, otherwise false</returns>
        public bool BuilderHasMarker(string marker)
        {
            return builder.HasMarker(marker);
        }
        #endregion

        /// <summary>
        /// Adds the reverse of all currently added actions to the queue placed where this is called
        /// </summary>
        public TweeningAnimation Reverse()
        {
            then();
            Marker("_reverse");
            call(AddReverse());
            return then();
        }

        protected Action AddReverse()
        {
            int reverseQueueNumber = actionQueues.Count;

            return () => {
                if (!HasMarker("_reverse", queueNumber)) return;
                RemoveSequence(reverseQueueNumber);

                for(int i = 0; i < reverseQueueNumber; i++)
                {
                    TweeningSequence sequence = actionQueues[i];
                    TweeningSequence reverseSequence = sequence.GetReverse();


                    InsertSequence(reverseQueueNumber, reverseSequence);
                }

                //SetQueueNumber(queueNumber - 1);
                SetQueueNumber(reverseQueueNumber-1);
            };
        }

        /// <summary>
        /// Starts the animation and sets it to loop forever, meaning it will go back to the start of the animation whenever it reaches the end
        /// </summary>
        public void Loop()
        {
            then();
            call(SetQueueNumberAction(-1));
            Start();
        }

        /// <summary>
        /// Repeats the sequence a number of times after it has been played. Beware: Passing 2 will play the animation a total of 3 times, one for the original playback and then two repititions.
        /// </summary>
        /// <param name="times">The amount of times to repeat the sequence</param>
        public void Repeat(uint times)
        {
            uint timesLeft = times;
            then();
            call(()=> 
            {
                if (timesLeft == 0)
                    return;
                timesLeft--;
                SetQueueNumber(-1);
            });
            Start();
        }

        protected Action SetQueueNumberAction(int number)
        {
            return () => { SetQueueNumber(number); };
        }

        protected void SetQueueNumber(int number)
        {
            queueNumber = number;
        }


        #endregion

        #region Branching

        public TweeningAnimation Branch(Func<TweeningAnimation, bool> condition, Action<TweeningAnimation> actionIfTrue, Action<TweeningAnimation> actionIfFalse)
        {
            then();

            

            call(()=> {
                int queueNumber = this.queueNumber;
                Action<TweeningAnimation> actionToCall = condition(this) ? actionIfTrue : actionIfFalse;

                int markerID = GetUniqueNumber();
                string marker = "#" + markerID;
                string markerStart = marker + "start";
                string markerEnd = marker + "end";



                then(markerStart);
                then();
                actionToCall(this);
                then(markerEnd);
                call(() => {RemoveSequences(markerStart, markerEnd); SetQueueNumber(queueNumber); });
                then();

                int markerQueueNumber = GetMarkerQueueNumber(markerStart);


                SetQueueNumber(markerQueueNumber - 1);
            });

            return then();

            
        }



        #endregion



        int uniqueNumber = 0;

        int GetUniqueNumber()
        {
            return uniqueNumber++;
        }

        #region FindindSequences

        public int GetMarkerQueueNumber(string marker)
        {
            for(int i = 0; i < actionQueues.Count; i++)
            {
                if (actionQueues[i].HasMarker(marker)) return i;
            }

            return -1;
        }

        public TweeningSequence GetSequenceInQueue(int queueNumber)
        {
            if (queueNumber < 0 || queueNumber >= actionQueues.Count) return null;
            return actionQueues[queueNumber];
        }

        public TweeningSequence GetSequenceInQueue(string marker)
        {
            return GetSequenceInQueue(GetMarkerQueueNumber(marker));
        }

        public List<TweeningSequence> GetTweeningSequences(int queueNumberStartInclusive, int queueNumberEndInclusive)
        {
            queueNumberStartInclusive = Mathf.Clamp(queueNumberStartInclusive, 0, actionQueues.Count - 1);
            queueNumberEndInclusive = Mathf.Clamp(queueNumberEndInclusive, 0, actionQueues.Count - 1);

            List<TweeningSequence> result = new List<TweeningSequence>();

            int sign = (int)Mathf.Sign(queueNumberEndInclusive - queueNumberStartInclusive);
            for(int i = queueNumberStartInclusive; sign > 0 ? (i <= queueNumberEndInclusive) : (i >= queueNumberEndInclusive); i += sign)
            {
                result.Add(GetSequenceInQueue(i));
            }

            return result;
        }

        public List<TweeningSequence> GetTweeningSequences(string markerFrom, string markerTo)
        {
            return GetTweeningSequences(GetMarkerQueueNumber(markerFrom), GetMarkerQueueNumber(markerTo));
        }

        #endregion

        #region Removing and Inserting Sequences

        public void RemoveSequence(TweeningSequence sequence)
        {
            actionQueues.Remove(sequence);
        }

        public void RemoveSequence(string marker)
        {
            TweeningSequence sequence = GetSequenceInQueue(marker);
            RemoveSequence(sequence);
        }

        public void RemoveSequence(int queueNumber)
        {
            TweeningSequence sequence = GetSequenceInQueue(queueNumber);
            RemoveSequence(sequence);
        }

        public void RemoveSequences(List<TweeningSequence> sequences)
        {
            foreach(TweeningSequence sequence in sequences)
            {
                RemoveSequence(sequence);
            }
        }

        public void RemoveSequences(int queueNumberStartInclusive, int queueNumberEndInclusive)
        {
            RemoveSequences(GetTweeningSequences(queueNumberStartInclusive, queueNumberEndInclusive));
        }

        public void RemoveSequences(string markerFrom, string markerTo)
        {
            RemoveSequences(GetTweeningSequences(markerFrom, markerTo));
        }


        public void InsertSequence(int queueNumber, TweeningSequence sequence)
        {
            actionQueues.Insert(queueNumber, sequence);
        }

        public void InsertSequence(string marker, TweeningSequence sequence)
        {
            actionQueues.Insert(GetMarkerQueueNumber(marker), sequence);
        }


        #endregion





        /// <summary>
        /// Returns an animation that calls the callback with angle values from 0 to 2 PI over the given period
        /// </summary>
        /// <param name="mono">A monobehaviour, to start the animation</param>
        /// <param name="period">The period for the oscillation</param>
        /// <param name="callback">The callback to pass the angle to.</param>
        /// <returns>The animation</returns>
        public static TweeningAnimation Oscillate(MonoBehaviour mono, float period, Action<float> callback)
        {
            TweeningAnimation animation = new TweeningAnimation(mono);
            animation.
                floatCallback(0, Mathf.PI * 2, callback, period);
            return animation;
        }
    }

}


