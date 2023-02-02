using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Kelecik
{
    public static class Tools 
    {
        public static Coroutine Lerp(this MonoBehaviour monoBehaviour, float initValue, float endValue, float duration, UnityAction<float> currentValue)
        {
            return monoBehaviour.StartCoroutine(ExecuteRoutine(initValue, endValue, duration, currentValue));
        }

        private static IEnumerator ExecuteRoutine(float initValue, float endValue, float duration, UnityAction<float> currentValue)
        {
            float elapsedTime = 0f;
            float valueToLerp = initValue;
            while (Math.Abs(valueToLerp - endValue) > 0f)
            {
                valueToLerp = Mathf.Lerp(initValue, endValue, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                currentValue?.Invoke(valueToLerp);
                yield return null;
            }
        }
    }
}


