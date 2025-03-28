using EPQ;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

public class LineManager : MonoBehaviour
{
    public float lineSize = 1;
    public float cellRadius;
    public float horizontalMargin;
    public float verticalMargin;
    public LineRenderer circleRenderer;
    public float radius;
    public int steps;
    public int fontSize = 7;
    // Start is called before the first frame update
    void Start()
    {
        //DrawCircle(steps, radius);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*void DrawCircle(int steps, float radius)
    {
        circleRenderer.positionCount = steps+1;

        for(int currentStep = 0; currentStep<=steps; currentStep++)
        {
            float circumferenceProgress = (float)currentStep / steps;

            float currentRadian = circumferenceProgress * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);
            float x = xScaled * radius;
            float y = yScaled * radius;
            Vector3 currentPosition = new Vector3(x, y, 1);
            circleRenderer.SetPosition(currentStep, currentPosition);
            
        }
    }*/
    private void OnDrawGizmos()
    {
        Creature creature = Generator.generator.creatures[0];
        Brain brain = creature.Brain;
        int firstLayerSize = Brain.InputSize;
        int secondLayerSize = Brain.Hidden_2Size;
        int thirdLayerSize = Brain.HiddenSize;
        int fourthLayerSize = Brain.OutputSize;

        Vector2[] firstLayer = new Vector2[firstLayerSize];
        Vector2[] secondLayer = new Vector2[secondLayerSize];
        Vector2[] thirdLayer = new Vector2[thirdLayerSize];
        Vector2[] fourthLayer = new Vector2[fourthLayerSize];

        float horizontalFirst = -(horizontalMargin * 3 / 2);
        Vector2 bottomFirst = new Vector2(horizontalFirst, -(verticalMargin * (firstLayerSize-1) / 2f));
        for(int i = 0; i< firstLayerSize; i++)
        {
            Vector2 pos = bottomFirst + (new Vector2(0, verticalMargin) * i);
            firstLayer[i] = pos;
            Handles.DrawSolidDisc(pos, Vector3.back, cellRadius);
        }

        float horizontalSecond = horizontalFirst + horizontalMargin;
        Vector2 bottomSecond = new Vector2(horizontalSecond, -(verticalMargin * (secondLayerSize - 1) / 2f));
        for (int i = 0; i < secondLayerSize; i++)
        {
            Vector2 pos = bottomSecond + (new Vector2(0, verticalMargin) * i);
            secondLayer[i] = pos;
            Handles.DrawSolidDisc(pos, Vector3.back, cellRadius);
        }

        float horizontalThird = horizontalSecond + horizontalMargin;
        Vector2 bottomThird = new Vector2(horizontalThird, -(verticalMargin * (thirdLayerSize - 1) / 2f));
        for (int i = 0; i < thirdLayerSize; i++)
        {
            Vector2 pos = bottomThird + (new Vector2(0, verticalMargin) * i);
            thirdLayer[i] = pos;
            Handles.DrawSolidDisc(pos, Vector3.back, cellRadius);
        }

        float horizontalFourth = horizontalThird + horizontalMargin;
        Vector2 bottomFourth = new Vector2(horizontalFourth, -(verticalMargin * (fourthLayerSize - 1) / 2f));
        for (int i = 0; i < fourthLayerSize; i++)
        {
            Vector2 pos = bottomFourth + (new Vector2(0, verticalMargin) * i);
            fourthLayer[i] = pos;
            Handles.DrawSolidDisc(pos, Vector3.back, cellRadius);
        }
        GUIStyle style = new GUIStyle();
        style.fontSize = fontSize;
        style.stretchWidth = false;
        style.stretchHeight = false;
        for(int i = 0; i < firstLayerSize; i++) 
            for(int j = 0; j<secondLayerSize; j++)
            {
                

                float value = brain.inputToHidden.Cells[j,i]*brain.currentFirstLayer[i];
                float ratio = (value + 1) * Mathf.PI/ 4;
                float sin = Mathf.Sin(ratio);
                Handles.color = new Color(sin,0.5f,0.1f);
                Handles.DrawLine(firstLayer[i], secondLayer[j], lineSize);
                Handles.color = Color.white;

                float weight = (float)Math.Round(brain.inputToHidden.Cells[j, i], 2);
                Vector2 centrePos = ((secondLayer[j] - firstLayer[i]) * 0.6f) + firstLayer[i];
                Handles.Label(centrePos, weight.ToString(), style);
            }
        for (int i = 0; i < secondLayerSize; i++)
            for (int j = 0; j < thirdLayerSize; j++)
            {
                

                float value = brain.hiddenToHidden.Cells[j,i] * brain.currentSecondLayer[i];
                float ratio = (value + 1) * Mathf.PI / 4;
                float sin = Mathf.Sin(ratio);
                Handles.color = new Color(sin, 0.3f, 0.6f);
                Handles.DrawLine(secondLayer[i], thirdLayer[j], lineSize);
                Handles.color = Color.white;

                float weight = (float)Math.Round(brain.hiddenToHidden.Cells[j, i], 2);
                Vector2 centrePos = ((thirdLayer[j] - secondLayer[i]) * 0.6f) + secondLayer[i];
                Handles.Label(centrePos, weight.ToString(), style);
            }
        for (int i = 0; i < thirdLayerSize; i++)
            for (int j = 0; j < fourthLayerSize; j++)
            {
                

                float value = brain.hiddenToOutput.Cells[j,i] * brain.currentThirdLayer[i];
                float ratio = (value + 1) * Mathf.PI / 4;
                float sin = Mathf.Sin(ratio);
                Handles.color = new Color(sin, 0.5f, 1f);
                Handles.DrawLine(thirdLayer[i], fourthLayer[j], lineSize);
                Handles.color = Color.white;

                float weight = (float)Math.Round(brain.hiddenToOutput.Cells[j, i], 2);
                Vector2 centrePos = ((fourthLayer[j] - thirdLayer[i]) * 0.6f) + thirdLayer[i];
                Handles.Label(centrePos, weight.ToString(), style);
            }
        
        foreach(var pos in firstLayer)
            Handles.DrawSolidDisc(pos, Vector3.back, cellRadius);
        foreach (var pos in secondLayer)
            Handles.DrawSolidDisc(pos, Vector3.back, cellRadius);
        foreach (var pos in thirdLayer)
            Handles.DrawSolidDisc(pos, Vector3.back, cellRadius);
        foreach (var pos in fourthLayer)
            Handles.DrawSolidDisc(pos, Vector3.back, cellRadius);
    }
}
