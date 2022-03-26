using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace hjijijing.Tweening
{

    public static class Easing
    {


        //SINE

        public static Func<float, float> easeInSine = (t) =>
        {
            return 1f - Mathf.Cos((Mathf.PI * t) / 2f);
        };

        public static Func<float, float> easeOutSine = (t) =>
        {
            return Mathf.Sin((t * Mathf.PI) / 2f);
        };

        public static Func<float, float> easeInOutSine = (t) =>
        {
            return -(Mathf.Cos(Mathf.PI * t) - 1f) / 2f;
        };

        //QUAD

        public static Func<float, float> easeInQuad = (t) =>
        {
            return t * t;
        };

        public static Func<float, float> easeOutQuad = (t) =>
        {
            return 1f - ((1f - t) * (1f - t));
        };

        public static Func<float, float> easeInOutQuad = (t) =>
        {
            return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
        };


        //CUBIC

        public static Func<float, float> easeInCubic = (t) =>
        {
            return t * t * t;
        };

        public static Func<float, float> easeOutCubic = (t) =>
        {
            return 1f - Mathf.Pow(1f - t, 3f);
        };

        public static Func<float, float> easeInOutCubic = (t) =>
        {
            return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
        };

        //QUART

        public static Func<float, float> easeInQuart = (t) =>
        {
            return t * t * t * t;
        };

        public static Func<float, float> easeOutQuart = (t) =>
        {
            return 1f - Mathf.Pow(1f - t, 4f);
        };

        public static Func<float, float> easeInOutQuart = (t) =>
        {
            return t < 0.5f ? 8f * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 4f) / 2f;
        };

        //Quint

        public static Func<float, float> easeInQuint = (t) =>
        {
            return t * t * t * t * t;
        };

        public static Func<float, float> easeOutQuint = (t) =>
        {
            return 1f - Mathf.Pow(1f - t, 5f);
        };

        public static Func<float, float> easeInOutQuint = (t) =>
        {
            return t < 0.5f ? 16f * t * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 5f) / 2f;
        };

        //Expo

        public static Func<float, float> easeInExpo = (t) =>
        {
            return t == 0f ? 0f : Mathf.Pow(2f, 10f * t - 10f);
        };

        public static Func<float, float> easeOutExpo = (t) =>
        {
            return t == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
        };

        public static Func<float, float> easeInOutExpo = (t) =>
        {
            return t == 0f ? 0f : t == 1f ? 1f : t < 0.5f ? Mathf.Pow(2f, 20f * t - 10f) / 2f : (2f - Mathf.Pow(2f, -20f * t + 10f)) / 2f;
        };

        //Circ

        public static Func<float, float> easeInCirc = (t) =>
        {
            return 1 - Mathf.Sqrt(1f - Mathf.Pow(t, 2f));
        };

        public static Func<float, float> easeOutCirc = (t) =>
        {
            return Mathf.Sqrt(1f - Mathf.Pow(t - 1f, 2f));
        };

        public static Func<float, float> easeInOutCirc = (t) =>
        {
            return t < 0.5f ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * t, 2f))) / 2f : (Mathf.Sqrt(1f - Mathf.Pow(-2f * t + 2f, 2f)) + 1f) / 2f;
        };


        //Back

        public static Func<float, float> easeInBack = (t) =>
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1f;

            return c3 * t * t * t - c1 * t * t;
        };

        public static Func<float, float> easeOutBack = (t) =>
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1f;

            return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
        };

        public static Func<float, float> easeInOutBack = (t) =>
        {
            float c1 = 1.70158f;
            float c2 = c1 * 1.525f;

            return t < 0.5f
              ? (Mathf.Pow(2f * t, 2f) * ((c2 + 1f) * 2f * t - c2)) / 2f
              : (Mathf.Pow(2f * t - 2f, 2f) * ((c2 + 1f) * (t * 2f - 2f) + c2) + 2f) / 2f;
        };


        //Elastic

        public static Func<float, float> easeInElastic = (t) =>
        {
            float c4 = (2f * Mathf.PI) / 3f;

            return t == 0f
              ? 0f
              : t == 1f
              ? 1f
              : -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sin((t * 10f - 10.75f) * c4);
        };

        public static Func<float, float> easeOutElastic = (t) =>
        {
            float c4 = (2f * Mathf.PI) / 3f;

            return t == 0f
              ? 0f
              : t == 1f
              ? 1f
              : Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
        };

        public static Func<float, float> easeInOutElastic = (t) =>
        {
            float c5 = (2f * Mathf.PI) / 4.5f;

            return t == 0f
              ? 0f
              : t == 1f
              ? 1f
              : t < 0.5f
              ? -(Mathf.Pow(2f, 20f * t - 10f) * Mathf.Sin((20f * t - 11.125f) * c5)) / 2f
              : (Mathf.Pow(2f, -20f * t + 10f) * Mathf.Sin((20f * t - 11.125f) * c5)) / 2f + 1f;
        };


        //Bounce

        public static Func<float, float> easeInBounce = (t) =>
        {
            return 1f - easeOutBounce(1f - t);
        };

        public static Func<float, float> easeOutBounce = (t) =>
        {
            float n1 = 7.5625f;
            float d1 = 2.75f;

            if (t < 1f / d1)
            {
                return n1 * t * t;
            }
            else if (t < 2f / d1)
            {
                return n1 * (t -= 1.5f / d1) * t + 0.75f;
            }
            else if (t < 2.5f / d1)
            {
                return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            }
            else
            {
                return n1 * (t -= 2.625f / d1) * t + 0.984375f;
            }
        };

        public static Func<float, float> easeInOutBounce = (t) =>
        {
            return t < 0.5f
                ? (1f - easeOutBounce(1f - 2f * t)) / 2f
                : (1f + easeOutBounce(2f * t - 1f)) / 2f;
        };



        public static Func<float, float> linear = (t) => t;


        public static Func<float, float> randomEasing(float min, float max)
        {
            return (t) => { return t == 0f ? 0f : t == 1f ? 1f : UnityEngine.Random.Range(min, max); };
        }

        public static Func<float, float> reverseEasing(Func<float, float> easing)
        {
            return (t) => { return -easing(t); };
        }

        public static Func<float, float> combineEasings(Func<float, float> easing1, Func<float, float> easing2, float transitionPoint = 0.5f)
        {
            return (t) =>
            {
                if (t <= transitionPoint)
                    return easing1(t / transitionPoint);
                else
                    return easing2((t - transitionPoint)/(1 - transitionPoint));
            };
        }
    }

}


