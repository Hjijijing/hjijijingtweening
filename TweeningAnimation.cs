using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hjijijing.Tweening
{

    public class TweeningAnimation
    {

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
            if (builder.Count != 0)
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

            if (queueNumber >= actionQueues.Count) return;

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

        #region Tweeners

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
            then();
            WaitForTime action = new WaitForTime(builder.tweenDone, source, gameObject, duration);

            AddActionToBuilder(action);
            then();

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
                actionQueues.RemoveAt(reverseQueueNumber);

                for(int i = 0; i < reverseQueueNumber; i++)
                {
                    TweeningSequence sequence = actionQueues[i];
                    TweeningSequence reverseSequence = sequence.GetReverse();



                    actionQueues.Insert(reverseQueueNumber, reverseSequence);
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
                Action<TweeningAnimation> actionToCall = condition(this) ? actionIfTrue : actionIfFalse;

                float stamp = Process.GetCurrentProcess().StartTime;

                then("#" + stamp)
            
            
            });

            then();

            throw new NotImplementedException();
        }



        #endregion



    }

}


