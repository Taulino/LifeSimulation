using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EPQ
{
    public class Breed
    {
        System.Random r = new System.Random();

        public int threshHold = 0;

        private readonly IReadOnlyList<(Brain brain, float score)> scores;
        public readonly float totalScore;
        public Breed(List<Creature> creatures)
        {
            scores = creatures
                .Select(x => (x.Brain, x.currentScore()))
                .Pair_OrderBy((brain, score) => -score)
                .ToList();
            
            totalScore = scores.Pair_Select((brain, score) => score).Sum();
        }
        public IReadOnlyList<Brain> KeepTopN(int count) =>
            scores
                .Pair_Select((brain, score) => brain)
                .Take(count)
                .ToList();

        public Brain Spawn()
        {
            Brain mom = ChooseParent();
            Brain dad = ChooseParent();
            return Brain.Cross(mom, dad);
        }
        public Brain SpawnRandom()
        {
            Brain mom = ChooseRandomParent();
            Brain dad = ChooseRandomParent();
            return Brain.Cross(mom, dad);
        }
        private Brain ChooseRandomParent() =>
            scores[Rng.GetInt(0, scores.Count)].brain;
        private Brain ChooseParent()
        {
            double seed = Rng.GetDouble(0, totalScore);
            foreach ((Brain brain, double score) pair in scores)
            {
                seed -= pair.score;
                if (seed < 0)
                {
                    return pair.brain;
                }
            }
            throw new InvalidOperationException("No matching snake found");
        }
        private Brain TournamentFunction(int k)
        {
            
            if (k > scores.Count) k = scores.Count / 2;
            var randomSample = Enumerable.Range(0, k).Select(x => scores[r.Next(0, scores.Count)]).ToList();
            
            if(scores.Max(x => x.score) >= threshHold)
            {
                while(randomSample.Max(x => x.score) < threshHold) randomSample = Enumerable.Range(0, k).Select(x => scores[r.Next(0, scores.Count)]).ToList();
            }
            var best = randomSample.Where(x => x.score == randomSample.Max(x => x.score)).First();
            return best.brain;
        }
        public int scoresCount() => scores.Count();
        public float largestScore => scores.Max(x => x.score);
    }
}
