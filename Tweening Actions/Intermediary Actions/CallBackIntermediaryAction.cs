using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hjijijing.Tweening {
    public class CallBackIntermediaryAction : IntermediaryTweeningAction
    {
        public Action action;

        public CallBackIntermediaryAction(Action action)
        {
            this.action = action;
        }

        public override void doAction()
        {
            action();
        }
    }
}


