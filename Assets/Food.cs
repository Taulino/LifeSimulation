using EPQ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float NutritionValue { get; set; } = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.tag == "Normal")
        {
            Generator.generator.OrganismEatFood(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
