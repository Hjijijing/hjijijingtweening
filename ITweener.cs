using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace hjijijing.Tweening
{

    public interface ITweener
    {

        void Stop();

        void forceFinish();

        void revert();

        Func<float, float> easing { get; set; }

        IEnumerator execute(Action<ITweener> onDone);

        ITweeningAction getReverse(Action<ITweener> onDone);
    }


}

