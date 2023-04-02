using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Kelecik
{
    public static class KeTween
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

        private static IEnumerator OnCoroutineComplete(this IEnumerator coroutine, Action onComplete)
        {
            yield return coroutine;
            onComplete?.Invoke();
        }
    }

    public static class Tools
    {
        public static byte[] ObjectToByte(this object obj)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, obj);
            return memoryStream.ToArray();
        }

        public static object ByteToObject(this byte[] bytesData)
        {
            MemoryStream memoryStream = new MemoryStream(bytesData);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            object objectData = binaryFormatter.Deserialize(memoryStream);
            return objectData;
        }

        /// <summary>
        /// "If the material data in Unity's render component is not singular, you need to first store it in a variable and then modify it before reassigning it again. This method will save you from the hassle."
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="index"></param>
        /// <param name="material"></param>
        public static void ChangeMaterial(this Renderer renderer, int index, Material material)
        {
            var materialDataContr = renderer.materials;
            materialDataContr[index] = material;
            renderer.materials = materialDataContr;
        }

        public static int[] RandomIntArray(int count)
        {
            System.Random random = new System.Random();
            int[] result = new int[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = random.Next();
            }

            return result;
        }

        public static bool IsPossible(int possibility)
        {
            int random = UnityEngine.Random.Range(0, 100);
            return random <= possibility;
        }

        /// <summary>
        /// you can get percentage of your values 
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static float Percentage(float amount, float percentage) => amount * 0.01f * percentage;

        /// <summary>
        /// It calculates the value between the origin and target distance based on the maximum distance and returns the farthest distance as 1
        /// </summary>
        public static float InvertDistanceNormal(Vector3 from, Vector3 to, float maxDistance)
        {
            float distance = Vector3.Distance(from, to);
            float normalizedDistance = Mathf.Clamp01(distance / maxDistance);
            return normalizedDistance;
        }

        /// <summary>
        /// It calculates distance based on the maximum distance and returns the farthest distance as 1
        /// </summary>
        public static float InvertDistanceNormal(float distance, float maxDistance)
        {
            float normalizedDistance = Mathf.Clamp01(distance / maxDistance);
            return normalizedDistance;
        }
    }

    public static class AI
    {
        /// <summary>
        /// It defines the distance from the starting point to the target point according to the defined navmesh path.
        /// </summary>
        public static float GetDistance(Transform origin, Transform target, NavMeshPath path, int areaMask)
        {
            float distance = 0;
            NavMesh.CalculatePath(origin.position, target.position, areaMask, path);
            if (path.status == NavMeshPathStatus.PathInvalid || path.corners.Length <= 1) return distance;
            for (int i = 1; i < path.corners.Length; i++)
            {
                Vector3 previous = path.corners[i - 1];
                Vector3 next = path.corners[i];
                distance += Vector3.Distance(previous, next);
            }

            return distance;
        }

        /// <summary>
        /// If the target is outside the defined navmesh path, it returns the closest position to the target.
        /// </summary>
        public static void CalcSamplePosition(Vector3 point, out Vector3 result, int areaMask, float maxDistance = 100f)
        {
            NavMesh.SamplePosition(point, out var sampleHit, maxDistance, areaMask);
            result = sampleHit.position;
        }

        /// <summary>
        /// The navmesh checks if it can create a path to the target.
        /// </summary>
        public static bool PathExist(Vector3 fromPos, Vector3 toPos, NavMeshPath path, int areMask)
        {
            path.ClearCorners();
            return NavMesh.CalculatePath(fromPos, toPos, areMask, path);
        }

        /// <summary>
        /// The navmesh checks if the object can reach this target.
        /// </summary>
        public static bool IsReachableArea(Vector3 from, Vector3 to, int areaMask, NavMeshPath path)
        {
            if (PathExist(from, to, path, areaMask))
            {
                return path.status != NavMeshPathStatus.PathInvalid && path.status != NavMeshPathStatus.PathPartial;
            }

            return false;
        }
        
        /// <summary>
        ///  It checks whether the target object is behind the origin object or not.
        /// </summary>
        public static bool IsTargetBehind(Transform origin,Transform target,float degreeOfBehindAmount)
        {
            Vector3 dir = origin.position - target.position;
            float angle = Vector3.Angle(origin.forward, dir);
            return !(angle > degreeOfBehindAmount);
        }
    }
}