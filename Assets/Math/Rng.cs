using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace EPQ
{

    public static class Rng
    {
        private static int seed = Environment.TickCount;

        private static ThreadLocal<System.Random> random = new ThreadLocal<System.Random>(
            () => new System.Random(Interlocked.Increment(ref seed)));
        public static int GetInt(int min, int max)
            => random.Value.Next(min, max);
        public static float GetFloat(float min, float max)
            => (float)random.Value.NextDouble() * (max - min) + min;
        public static double GetDouble(double min, double max)
            => random.Value.NextDouble() * (max - min) + min;
        public static double Gaussian(float mean, float stdDev)
        {
            double uniform1 = 1.0 - GetDouble(0, 1);  // uniform(0,1] random doubles
            double uniform2 = 1.0 - GetDouble(0, 1);
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(uniform1)) *
                                   Math.Sin(2.0 * Math.PI * uniform2);  // random normal(0,1)
            return mean + stdDev * randStdNormal;  // random normal(mean,stdDev^2)
        }
    }
}