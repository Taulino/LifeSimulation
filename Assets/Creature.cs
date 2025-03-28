using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;
namespace EPQ
{
    public class Creature
    {
        private float Distance { get; set; }
        public float TimeAlive { get; set; }
        public float TimeLeft { get; set; } = 20;
        public int FoodEaten { get; set; }
        public int FoodCounter { get; set; }
        public bool Alive { get; set; } = true;
        public Brain Brain { get;}
        public GameObject creatureObject { get; }
        public float Velocity { get; set; } = 4f;
        public float Vision { get; set; } = 6;

        public SpriteRenderer renderer;
        public Rigidbody2D rb;

        public Creature(Brain Brain, GameObject creatureObject)
        {
            this.Brain = Brain;
            this.creatureObject = creatureObject;
            renderer = creatureObject.GetComponent<SpriteRenderer>();
            rb = creatureObject.GetComponent<Rigidbody2D>();
        }
        public Creature(GameObject creatureObject)
        {
            this.Brain = Brain.Random();
            this.creatureObject = creatureObject;
            renderer = creatureObject.GetComponent<SpriteRenderer>();
            rb = creatureObject.GetComponent<Rigidbody2D>();
        }
        public virtual void Move()
        {
            float horizontalExtend = Camera.main.orthographicSize * Screen.width / Screen.height;
            float verticalExtend = Camera.main.orthographicSize;
            if (creatureObject.transform.position.x > -renderer.bounds.size.x / 2 + horizontalExtend)
                creatureObject.transform.position = new Vector3(-renderer.bounds.size.x / 2 + horizontalExtend, creatureObject.transform.position.y, creatureObject.transform.position.z);
            if (creatureObject.transform.position.x < renderer.bounds.size.x / 2 - horizontalExtend)
                creatureObject.transform.position = new Vector3(renderer.bounds.size.x / 2 - horizontalExtend, creatureObject.transform.position.y, creatureObject.transform.position.z);
            if (creatureObject.transform.position.y > -renderer.bounds.size.y / 2 + verticalExtend)
                creatureObject.transform.position = new Vector3(creatureObject.transform.position.x, -renderer.bounds.size.y / 2 + verticalExtend, creatureObject.transform.position.z);
            if (creatureObject.transform.position.y < renderer.bounds.size.y / 2 - verticalExtend)
                creatureObject.transform.position = new Vector3(creatureObject.transform.position.x, renderer.bounds.size.y / 2 - verticalExtend, creatureObject.transform.position.z);


            TimeAlive += Time.deltaTime;

            if (TimeAlive > TimeLeft)
            {
                Die();
            }
            
        }
        public void EatFood()
        {
            FoodEaten++;
            TimeLeft += 1.5f;

            FoodCounter++;
            if(FoodCounter == Generator.generator.FoodCounterVariable)
            {
                Generator.generator.GenerateNewNormal();
                FoodCounter = 0;
            }
        }
        public void Die()
        {
            Alive = false;
            Generator.generator.DestroyObject(creatureObject);
            Generator.generator.checkIfRunning();
        }
        public virtual float currentScore()
        {
            return 5 * FoodEaten * FoodEaten + Distance*10;
        }


        public static Vector2 calculateDirection(IReadOnlyList<float> data) =>
           new Vector2(data[0], data[1]).normalized;
    }
}
