using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EPQ {
    public class Virus : Creature
    {
        public Virus(Brain brain, GameObject creatureObject) : base(brain, creatureObject) { /*Velocity = 6; Vision = 12;*/ }
        public Virus(GameObject creatureObject) : base(creatureObject) { /*Velocity = 6; Vision = 12; */ }

        public override void Move()
        {
            if (!Alive) return;

            float[] data = collectData();
            var applied = CollectionsExtensions.ApplyDirection(data[0], data[1]);
            var results = Brain.Think(new float[] { applied.Item1, applied.Item2, data[2], data[3],data[4],data[5], 0,0});
            var normalizedResults = CollectionsExtensions.ApplyDirection(results[0], results[1]);
            creatureObject.transform.position += new Vector3(normalizedResults.Item1 * Time.deltaTime * Velocity, normalizedResults.Item2 * Time.deltaTime * Velocity, 0);

            Hunt();
            base.Move();
        }
        public override float currentScore()
        {
            return TimeAlive * Mathf.Pow(2, Mathf.Min(FoodEaten, 10)) * Mathf.Max(1, FoodEaten - 9);
            //return Mathf.Pow(3, Mathf.Min(FoodEaten, 6)) * Mathf.Max(1, FoodEaten - 5);
        }
        private float[] collectData()
        {
            var entitiesDetected = Physics2D.OverlapCircleAll(creatureObject.transform.position, Vision)
                .ToList();
            float[] data = new float[6];

            var normalDetected = entitiesDetected.Where(x => x.tag == "Normal").ToList();
            
            if (normalDetected.Count > 0)
            {
                var closest = normalDetected.OrderBy(x => (x.transform.position - creatureObject.transform.position).magnitude).First().transform.position - creatureObject.transform.position;
                Vector2 resultantVector = Vector2.zero;
                normalDetected.ForEach(x =>
                {
                    Vector2 delta = x.transform.position - creatureObject.transform.position;
                    resultantVector += Mathf.Pow(delta.magnitude, -2) * delta *100;
                });
                //resultantVector /= normalDetected.Count;
                data[0] = closest.x;
                data[1] = closest.y;
                data[2] = resultantVector.x;
                if (float.IsNaN(resultantVector.x)) data[2] = 0;
                data[3] = resultantVector.y;
                if (float.IsNaN(resultantVector.y)) data[3] = 0;
            }
            var normals = Generator.generator.creatures.Where(x => x.GetType() == typeof(Normal) && x.Alive).ToList();
            Vector2 normalVector = Vector2.zero;
            normals.ForEach(x =>
            {
                normalVector += (Vector2)x.creatureObject.transform.position;
            });
            normalVector /= normals.Count;
            var relativeNormalPos = normalVector - (Vector2)creatureObject.transform.position;
            data[4] = 1/relativeNormalPos.x;
            data[5] = 1/relativeNormalPos.y;
            return data;
        }
        private void Hunt()
        {
            var matches = Physics2D.OverlapCircleAll(new Vector2(creatureObject.transform.position.x, creatureObject.transform.position.y), renderer.bounds.size.x / 2)
                .Where(x => x.tag == "Normal");
            if (matches.Count() > 0)
            {
                GameObject match = matches.OrderBy(x => (x.transform.position - creatureObject.transform.position).magnitude).First().gameObject;
                Eat(match);
            }
        }
        private void Eat(GameObject match)
        {
            TimeLeft += 1.75f;
            FoodEaten++;

            Generator.generator.VirusEatNormal(creatureObject, match);
        }
    }
}
