using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EPQ
{
    static class CollectionsExtensions
    {

        public static IReadOnlyList<float> AttachOne(this IReadOnlyList<float> values) =>
            values.Concat(new[] { 1f }).ToList();

        public static Matrix VectorToColumnMatrix(this IReadOnlyList<float> vector)
        {
            float[,] column = new float[vector.Count, 1];
            for (int i = 0; i < vector.Count; i++)
                column[i, 0] = vector[i];
            return new Matrix(column);
        }

        public static IReadOnlyList<float> ColumnMatrixToVector(this Matrix matrix) =>
            matrix.Cells.Cast<float>().ToList();

        public static IEnumerable<(A, B)> Pair_OrderBy<A, B, K>(this IEnumerable<(A, B)> items, Func<A, B, K> selectKey) =>
            items.OrderBy(pair => selectKey(pair.Item1, pair.Item2));
        public static IEnumerable<R> Pair_Select<A, B, R>(this IEnumerable<(A, B)> items, Func<A, B, R> map) =>
            items.Select(pair => map(pair.Item1, pair.Item2));

        public static T ArgMax<T>(this IReadOnlyList<T> items, Func<T, double> evaluate) where T : class
        {
            if (items.Count == 0)
                throw new InvalidOperationException("sequence may not be empty");

            T best = null;
            double bestScore = 0;

            foreach (T item in items)
            {
                double score = evaluate(item);
                if (best is null || score > bestScore)
                {
                    best = item;
                    bestScore = score;
                }
            }
            return best;
        }
        public static (float, float) ApplyDirection(float x, float y)
        {
            if (x == 0 && y == 0) return (0, 0);
            float k = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
            return (x / k, y / k);
        }
        public static float GetAngle(Vector2 vector)
        {
            float angle = Mathf.Atan(vector.y / vector.x);
            if (vector.x < 0) angle += Mathf.PI;
            return angle;
        }
    }
}