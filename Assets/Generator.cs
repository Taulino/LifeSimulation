using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UI;

namespace EPQ {
    public enum CreatureType
    {
        Normal,
        Virus
    }
    public class Generator : MonoBehaviour
    {
        public int FoodCounterVariable = 7;

        public Text populationText;
        public Text scoreText;
        public Text foodText;
        public Text scoreVirusText;
        public Text foodVirusText;

        public float MovesPerFrame = 1;
        private int FrameCount = 0;

        public static Generator generator;

        public GameObject testTarget;
        public GameObject normalPrefab;
        public GameObject virusPrefab;
        public int generationNumber;
        public int normalNumber;
        public int virusNumber;

        public float GenerationLifeTime;
        public int KeepTopN = 0;

        public List<Creature> creatures;

        private int CurrentPopulation = 0;

        void Awake()
        {
            generator = this;
            creatures = Enumerable.Range(0, normalNumber)
                .Select(i => (Creature)new Normal(spawnCreatureRandom(CreatureType.Normal)))
                .ToList()
                .Concat(Enumerable.Range(0, virusNumber)
                    .Select(i => (Creature)new Virus(spawnCreatureRandom(CreatureType.Virus)))
                    .ToList())
                .ToList();
            GetComponent<FoodGenerator>().SpawnFood();

            InvokeRepeating("GraphDrawNewPoint", 0.1f, 0.6f);
        }
        public void GraphDrawNewPoint()
        {
            int viruses = creatures.Where(x => x.GetType() == typeof(Virus)).Count();
            int normals = creatures.Where(x => x.GetType() == typeof(Normal)).Count();
            WindowGraph.graphManager.AddPointsGraph(viruses, normals);
        }
        private void Update()
        {
            if(MovesPerFrame >= 1)
                for (int i = 0; i < MovesPerFrame; i++) creatures.Where(x => x.Alive).ToList().ForEach(x => x.Move());
            else
            {
                FrameCount++;
                float k = 1 / MovesPerFrame;
                if (FrameCount > k)
                {
                    FrameCount = 0;
                    creatures.Where(x => x.Alive).ToList().ForEach(x => x.Move());
                }
            }
        }
        public void checkIfRunning()
        {
            var deadViruses = creatures.Where(x => x.GetType() == typeof(Virus) && !x.Alive).ToList().OrderBy(x => x.currentScore()).ToList();
            var deadNormals = creatures.Where(x => x.GetType() == typeof(Normal) && !x.Alive).ToList().OrderBy(x => x.currentScore()).ToList();
            if (deadViruses.Count() > 500) for(int i = 0; i< 100;i ++) creatures.Remove(deadViruses[i]);
            if (deadNormals.Count() > 500) for (int i = 0; i < 100; i++) creatures.Remove(deadNormals[i]);
            if (creatures.Where(x => x.GetType() == typeof(Virus) && x.Alive == true).Count() == 0 || creatures.Where(x => x.GetType() == typeof(Normal) && x.Alive == true).Count() == 0) 
                SpawnNewGeneration();
        }   
        private GameObject spawnCreatureRandom(CreatureType type)
        {
            float verticalExtend = Camera.main.orthographicSize;
            float horizontalExtend = Camera.main.orthographicSize * Screen.width / Screen.height;

            float x = UnityEngine.Random.Range(-horizontalExtend, horizontalExtend);
            float y = UnityEngine.Random.Range(-verticalExtend, verticalExtend);
            GameObject creaturePrefab = type == CreatureType.Normal ? normalPrefab : virusPrefab;
            return Instantiate(creaturePrefab, new Vector3(x, y, creaturePrefab.transform.position.z), Quaternion.identity);
        }
        private GameObject spawnCreature(Vector2 coordinates, CreatureType type)
        {
            GameObject creaturePrefab = type == CreatureType.Normal ? normalPrefab : virusPrefab;
            return Instantiate(creaturePrefab, new Vector3(coordinates.x, coordinates.y, creaturePrefab.transform.position.z), Quaternion.identity);
        }
        private void SpawnNewGeneration()
        {
            if (creatures.Count == 0)
                throw new InvalidOperationException("The generation is empty!");
            creatures.ForEach(x =>
            {
                if (x.creatureObject != null) Destroy(x.creatureObject);
            });

            CurrentPopulation++;
            int largestVirusScore = (int)creatures.Where(x => x.GetType() == typeof(Virus)).ToList().Max(x => x.currentScore());
            int mostVirusFoodEaten = creatures.Where(x => x.GetType() == typeof(Virus)).ToList().Max(x => x.FoodEaten);
            int largestNormalScore = (int)creatures.Where(x => x.GetType() == typeof(Normal)).ToList().Max(x => x.currentScore());
            int mostNormalFoodEaten = creatures.Where(x => x.GetType() == typeof(Normal)).ToList().Max(x => x.FoodEaten);
            UpdateText(CurrentPopulation, largestNormalScore, mostNormalFoodEaten, largestVirusScore, mostVirusFoodEaten);

            Breed normalBreed = new Breed(creatures.Where(x => x.GetType() == typeof(Normal)).ToList());
            Breed virusBreed = new Breed(creatures.Where(x => x.GetType() == typeof(Virus)).ToList());
            creatures = new List<Creature>();
            if (normalBreed.scoresCount() == 0)
                creatures = Enumerable.Range(0, normalNumber)
                    .Select(i => (Creature)new Normal(spawnCreatureRandom(CreatureType.Normal)))
                    .ToList();
            else if (normalBreed.totalScore == 0)
                creatures = Enumerable.Range(0, normalNumber)
                    .Select(x => (Creature)new Normal(normalBreed.SpawnRandom(), spawnCreatureRandom(CreatureType.Normal)))
                    .ToList();
            else
            {
                creatures = normalBreed.KeepTopN(KeepTopN)
                    .Concat(Enumerable.Range(0, normalNumber)
                        .AsParallel()
                        .Select(x => normalBreed.Spawn())
                        .ToList())
                    .Select(x => (Creature)new Normal(x, spawnCreatureRandom(CreatureType.Normal)))
                    .ToList();
            }

            if (virusBreed.scoresCount() == 0)
                creatures.AddRange(Enumerable.Range(0, virusNumber)
                    .Select(i => (Creature)new Virus(spawnCreatureRandom(CreatureType.Virus)))
                    .ToList());
            if (virusBreed.totalScore == 0)
                creatures.AddRange(Enumerable.Range(0, virusNumber)
                    .Select(i => (Creature)new Virus(virusBreed.SpawnRandom(), spawnCreatureRandom(CreatureType.Virus)))
                    .ToList());
            else
                creatures.AddRange(
                    virusBreed.KeepTopN(KeepTopN)
                    .Concat(Enumerable.Range(0, virusNumber)
                        .AsParallel()
                        .Select(x => virusBreed.Spawn())
                        .ToList())
                    .Select(x => (Creature)new Virus(x, spawnCreatureRandom(CreatureType.Virus)))
                    .ToList()
                    );

           
            GetComponent<FoodGenerator>().SpawnFood();

            
        }
        public void OrganismEatFood(GameObject organism)
        {
            Creature creature = creatures.First(x => x.creatureObject == organism);
            if (creature == null)
                throw new InvalidOperationException("The organism couldn't eat the food");
            else
            {
                creature.EatFood();
            }
        }
        public void VirusEatNormal(GameObject virus, GameObject normal)
        {
            Creature virusCreature = creatures.First(x => x.creatureObject == virus);
            if (virusCreature == null)
                throw new InvalidOperationException("The organism couldn't eat the food");
            else
            {
                Debug.Log("Spawned!");
                Creature found = creatures.Find(x => x.creatureObject == normal);
                found.Alive = false;
                Breed virusBreed = new Breed(creatures.Where(x => x.GetType() == typeof(Virus)).ToList());
                creatures.Add(new Virus(virusBreed.Spawn(), spawnCreature(normal.transform.position, CreatureType.Virus)));
                Destroy(normal);
                

            }
        }
        public void GenerateNewNormal()
        {
            Debug.Log("Generated!");
            Breed normalBreed = new Breed(creatures.Where(x => x.GetType() == typeof(Normal)).ToList());
            creatures.Add(new Normal(normalBreed.Spawn(), spawnCreatureRandom(CreatureType.Normal)));
        }
        public void DestroyObject(GameObject gameObject)
        {
            Destroy(gameObject);
        }

        public void BecomeVirus(GameObject creatureObject, Brain primalBrain)
        {

            

            Destroy(creatureObject);
            Creature found = creatures.Find(x => x.creatureObject == creatureObject);
            
            if (found == null) return;
            creatures.Remove(found);
        }

        public void UpdateText(int currentPopulation, float largestNormalScore, int mostNormalFoodEaten, float largestVirusScore, int mostVirusFoodEaten)
        {
            populationText.text = "Current population: " + currentPopulation;
            scoreText.text = "Largest normal score: " + largestNormalScore;
            foodText.text = "Most normal food eaten: " + mostNormalFoodEaten;
            scoreVirusText.text = "Largest virus score: " + largestVirusScore;
            foodVirusText.text = "Most virus food eaten: " + mostVirusFoodEaten;
        }
        
    }
}
