using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace hjijijing.Tweening
{

    public interface ITweener
    {

        bool forceOneAtEnd
        {
            get;
            set;
        }

        void Stop();

        void forceFinish();

        void revert();

        Func<float, float> easing { get; set; }

        IEnumerator execute(Action<ITweener> onDone);

        ITweeningAction getReverse(Action<ITweener> onDone);
    }


}

