using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Kelecik
{
    public static class Tools 
    {
        public static Coroutine Lerp(this MonoBehaviour monoBehaviour,float initValue,float endValue ,float duration, UnityAction<float> currentValue,float completeThreshold = .005f)
        {
            return monoBehaviour.StartCoroutine(ExecuteRoutine(initValue, endValue, duration, currentValue,completeThreshold));
        }

        private static IEnumerator ExecuteRoutine(float initValue, float endValue, float duration, UnityAction<float> currentValue,float completeThreshold)
        {
            float elapsedTime = 0f;
            float valueToLerp = 0f;
            while (Math.Abs(valueToLerp - endValue) >completeThreshold)
            { 
                valueToLerp = Mathf.Lerp(initValue, endValue, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                currentValue?.Invoke(valueToLerp);
                yield return null;
            }
        }
    }
}


