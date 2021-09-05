using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hjijijing.Tweening
{

    public class WaitForTime : AnimationTweeningAction<object>
    {

        public WaitForTime(Action<ITweener> onDone, MonoBehaviour mono, GameObject gameObject, float duration) : base(onDone, mono, gameObject, duration, duration) { }


        public override ITweeningAction getReverse()
        {
            return new WaitForTime(onDone, mono, gameObject, duration);
        }

        public override void modifyGameObject(float time)
        { 
        }

        public override void setStartValue()
        {
        }
    }

}


