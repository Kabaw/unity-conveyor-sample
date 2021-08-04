using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class MathUtil
    {
        public static Vector2 Abs(Vector2 vector)
        {
            vector[0] = Mathf.Abs(vector[0]);
            vector[1] = Mathf.Abs(vector[1]);

            return vector;
        }

        public static Vector3 Abs(Vector3 vector)
        {
            vector[0] = Mathf.Abs(vector[0]);
            vector[1] = Mathf.Abs(vector[1]);
            vector[2] = Mathf.Abs(vector[2]);

            return vector;            
        }

        public static float RuleOfThree(float a, float b, float c)
        {
            return (b * c) / a;
        }
    }
}