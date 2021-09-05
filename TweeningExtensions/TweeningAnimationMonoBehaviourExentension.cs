using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace hjijijing.Tweening
{
    public static class TweeningAnimationMonoBehaviourExentension
    {

        /// <summary>
        /// Creates and returns a tweening animation and sets the source to be the monobehaviour that this method is called on.
        /// Sets the primary (default) gameobject to the gameobject of the monobehaviour.
        /// </summary>
        /// <param name="source">The monobehaviour that this is called on</param>
        /// <returns>The tweening animation</returns>
        public static TweeningAnimation Tween(this MonoBehaviour source)
        {
            return new TweeningAnimation(source, source.gameObject);
        }

        /// <summary>
        /// Creates and returns a tweening animation and sets the source to be the monobehaviour that this method is called on.
        /// Sets the primary (default) gameobject to the gameobject of the monobehaviour.
        /// Sets the default easing function to the specified function.
        /// </summary>
        /// <param name="source">The monobehaviour that this is called on</param>
        /// <param name="easing">The default easing function to use when adding actions</param>
        /// <returns>The tweening animation</returns>
        public static TweeningAnimation Tween(this MonoBehaviour source, Func<float, float> easing)
        {
            return new TweeningAnimation(source, easing);
        }

        /// <summary>
        /// Creates and returns a tweening animation and sets the source to be the monobehaviour that this method is called on.
        /// Sets the primary (default) gameobject to the specified object.
        /// Sets the default easing function to the specified function.
        /// </summary>
        /// <param name="source">The monobehaviour that this is called on</param>
        /// <param name="easing">The default easing function to use when adding actions</param>
        /// <param name="gameObject">The gameobject to set as the primary (default) gameobject for the animation</param>
        /// <returns>The tweening animation</returns>
        public static TweeningAnimation Tween(this MonoBehaviour source, GameObject gameObject, Func<float, float> easing)
        {
            return new TweeningAnimation(source, gameObject, easing);
        }


    }
}