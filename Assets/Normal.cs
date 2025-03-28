using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using UnityEngine;
namespace EPQ
{
    public class Normal : Creature
    {
        public Normal(Brain brain, GameObject creatureObject) : base(brain, creatureObject) {  Velocity = 3f; Vision = 9; }
        public Normal(GameObject creatureObject) : base(creatureObject) { Velocity = 3f; Vision = 9; }
        
        public override void Move()
        {
            if (!Alive) return;

            var data = collectData();
            var applied = CollectionsExtensions.ApplyDirection(data[0], data[1]);
            var results = Brain.Think(new float[] { applied.Item1, applied.Item2, data[2], data[3], data[4], data[5], data[6], data[7] });
            var normalizedResults = CollectionsExtensions.ApplyDirection(results[0], results[1]);
            Vector3 newPos = new Vector3(normalizedResults.Item1 * Time.deltaTime * Velocity, normalizedResults.Item2 * Time.deltaTime * Velocity, 0) + creatureObject.transform.position;
            creatureObject.transform.position = newPos;
            
            base.Move();
        }
        public override float currentScore()
        {
            return TimeAlive * Mathf.Pow(2, Mathf.Min(FoodEaten, 10)) * Mathf.Max(1, FoodEaten - 9);
        }
        private float[] collectData()
        {
            var entitiesDetected = Physics2D.OverlapCircleAll(creatureObject.transform.position, Vision)
                .ToList();
            float[] data = new float[8];
            var foodDetected = entitiesDetected.Where(x => x.tag == "food").ToList();
            if (foodDetected.Count > 0)
            {
                var closest = foodDetected.OrderBy(x => (x.transform.position - creatureObject.transform.position).magnitude).First().transform.position - creatureObject.transform.position;
                Vector2 resultantVector = Vector2.zero;
                foodDetected.ForEach(x =>
                {
                    Vector2 delta = x.transform.position - creatureObject.transform.position;
                    resultantVector += Mathf.Pow(delta.magnitude, -2) * delta * 100;
                });
                data[0] = closest.x;
                data[1] = closest.y;
                data[2] = resultantVector.x;
                if (float.IsNaN(resultantVector.x)) data[2] = 0;
                data[3] = resultantVector.y;
                if (float.IsNaN(resultantVector.y)) data[3] = 0;
            }
            var virusDetected = entitiesDetected.Where(x => x.tag == "Virus").ToList();
            if (virusDetected.Count > 0)
            {
                Vector2 resultantVector = Vector2.zero;
                virusDetected.ForEach(x =>
                {
                    Vector2 delta = x.transform.position - creatureObject.transform.position;
                    resultantVector += Mathf.Pow(delta.magnitude, -2) * delta * 100;
                });
                data[4] = resultantVector.x;
                if (float.IsNaN(resultantVector.x)) data[4] = 0;
                data[5] = resultantVector.y;
                if (float.IsNaN(resultantVector.y)) data[5] = 0;
            }
            var viruses = Generator.generator.creatures.Where(x => x.GetType() == typeof(Virus) && x.Alive).ToList();
            Vector2 virusVector = Vector2.zero;
            viruses.ForEach(x =>
            {
                virusVector += (Vector2)x.creatureObject.transform.position;
            });
            virusVector /= viruses.Count;
            var relativeVirusPos = virusVector - (Vector2)creatureObject.transform.position;
            data[6] = relativeVirusPos.x;
            data[7] = relativeVirusPos.y;
            return data;
        }

        
    }
}
