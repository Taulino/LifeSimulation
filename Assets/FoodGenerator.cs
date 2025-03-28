using EPQ;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public class FoodGenerator : MonoBehaviour
{
    public float multiplier = 1;
    public float CyclesPerSecond = 5;
    public int FoodPerCycle = 2;
    public int FoodCount = 10;

    public GameObject foodPrefab;
    private float prefabRadius;

    private List<GameObject> currentFood = new List<GameObject>();

    [SerializeField]
    private LayerMask creatureLayerMask;
    private void Start()
    {
        CyclesPerSecond *= Generator.generator.MovesPerFrame;
    }
    public void SpawnFood()
    {
        ClearFood();
        prefabRadius = foodPrefab.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        System.Random r = new System.Random();
        for (int i = 0; i< FoodCount; i++)
        {
            SpawnFoodUnit(r);
        }

        startGeneratorCycle();
    }
    private void SpawnFoodUnit(System.Random r)
    {
        float horizontalExtend = Camera.main.orthographicSize * Screen.width / Screen.height;
        float verticalExtend = Camera.main.orthographicSize;
        
        float x = (horizontalExtend * 2 * (float)r.NextDouble()) - horizontalExtend;
        float y = (verticalExtend * 2 * (float)r.NextDouble()) - verticalExtend;
        while (Physics2D.OverlapCircle(new Vector2(x, y), prefabRadius, creatureLayerMask))
        {
            x = (horizontalExtend * 2 * (float)r.NextDouble()) - horizontalExtend;
            y = (verticalExtend * 2 * (float)r.NextDouble()) - verticalExtend;
        }
        currentFood.Add(Instantiate(foodPrefab, new Vector3(x, y, foodPrefab.transform.position.z), Quaternion.identity));
    }
    private void ClearFood()
    {
        currentFood.ForEach(x =>
        {
            if (x != null) Destroy(x);
        });
        currentFood = new List<GameObject>();
    }
    private void startGeneratorCycle()
    {
        float time = 1f / CyclesPerSecond;
        InvokeRepeating("GenerateFood", time, time);
    }
    private void GenerateFood()
    {
        currentFood.RemoveAll(x => x == null);
        System.Random r = new System.Random();
        for (int i = 0; i < FoodPerCycle; i++) if(currentFood.Count < FoodCount * multiplier)
            {
                SpawnFoodUnit(r);
                Debug.Log("Spawned!");
            }
    }
}
