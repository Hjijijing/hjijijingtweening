using UnityEngine;
using System.Collections;


namespace hjijijing.Tweening
{
    public class MarkerAction : IntermediaryTweeningAction
    {
        public string marker;

        public MarkerAction(string marker)
        {
            this.marker = marker;
        }

        public override void doAction()
        {
            return;
        }
    }

}
