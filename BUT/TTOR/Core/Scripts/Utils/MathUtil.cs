using System;
using UnityEngine;

namespace BUT.TTOR.Core.Utils
{
    public static class MathUtil
    {
        public const float EPSILON = 0.0001f;
        public const float EPSILON_SQR = 0.0000001f;

        public static bool FuzzyEquals(Vector2 a, Vector2 b)
        {
            return FuzzyEqual(Vector3.SqrMagnitude(a - b), EPSILON_SQR);
        }

        public static bool FuzzyEquals(Vector2 a, Vector2 b, float epsilon)
        {
            return FuzzyEqual(Vector3.SqrMagnitude(a - b), epsilon);
        }

        public static bool FuzzyEquals(Vector3 a, Vector3 b)
        {
            return FuzzyEqual(Vector3.SqrMagnitude(a - b), EPSILON_SQR);
        }

        public static bool FuzzyEquals(Vector3 a, Vector3 b, float epsilon)
        {
            return FuzzyEqual(Vector3.SqrMagnitude(a - b), epsilon);
        }

        private static bool FuzzyEqual(float a, float b, float epsilon)
        {
            return Math.Abs(a - b) < epsilon;
        }

        private static bool FuzzyEqual(float a, float b)
        {
            return FuzzyEqual(a, b, EPSILON);
        }

        public static int Factorial(int i)
        {
            if (i <= 1)
                return 1;
            return i * Factorial(i - 1);
        }
    }
}
   
