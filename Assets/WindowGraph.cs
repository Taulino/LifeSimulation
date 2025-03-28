using EPQ;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WindowGraph : MonoBehaviour
{
    public int numberOfDots = 50;
    [SerializeField] 
    private Sprite circleSprite;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private List<GameObject> gameObjectList;

    private List<int> virusNumbers = new List<int>();
    private List<int> normalNumbers = new List<int>();


    public static WindowGraph graphManager;
    private void Awake()
    {
        graphManager = this;
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("dashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("dashTemplateY").GetComponent<RectTransform>();
        gameObjectList = new List<GameObject>();
        List<int> valueList = new List<int>() { 5, 98, 56, 45, 30, 22, 17, 15, 13, 17, 25, 37, 40, 36,1,1,1,1,1,1,1 };
    }
    public void AddPointsGraph(int viruses, int normals)
    {
        foreach(GameObject gameObject in gameObjectList)
        {
            Destroy(gameObject);
        }
        gameObjectList.Clear();

        virusNumbers.Add(viruses);
        if (virusNumbers.Count > numberOfDots) virusNumbers = virusNumbers.GetRange(virusNumbers.Count - numberOfDots, numberOfDots);
        normalNumbers.Add(normals);
        if (normalNumbers.Count > numberOfDots) normalNumbers = normalNumbers.GetRange(normalNumbers.Count - numberOfDots, numberOfDots);
        int highest = virusNumbers.Max() > normalNumbers.Max() ? virusNumbers.Max() : normalNumbers.Max();

        ShowGraph(virusNumbers, Color.red, highest, numberOfDots);
        ShowGraph(normalNumbers, Color.white, highest, numberOfDots);
    }
    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.localScale *= 0.1f;
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }
    private void ShowGraph(List<int> valueList, Color color, int highestValue, int numberOfXValues)
    {
        float graphWidth = graphContainer.sizeDelta.x;
        float xSize = graphWidth / numberOfXValues;
        

        float graphHeight = graphContainer.sizeDelta.y;
        float yMaximum = highestValue * 1.3f;
        GameObject lastCircleGameObject = null;
        for(int i = 0; i< valueList.Count; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = (valueList[i] / yMaximum) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            gameObjectList.Add(circleGameObject);
            if (lastCircleGameObject != null) 
                gameObjectList.Add(CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition, color));
            lastCircleGameObject = circleGameObject;

            /*RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer, false);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -2f);
            labelX.GetComponent<Text>().text = i.ToString();
            gameObjectList.Add(labelX.gameObject);

            RectTransform dashX = Instantiate(dashTemplateY);
            dashX.SetParent(graphContainer, false);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(xPosition, -2f);
            gameObjectList.Add(dashX.gameObject);*/
        }

        int separatorCount = 10;
        /*for(int i = 0; i <= separatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount;
            labelY.anchoredPosition = new Vector2(-7f, normalizedValue* graphHeight);
            labelY.GetComponent<Text>().text = Mathf.RoundToInt(normalizedValue * yMaximum).ToString();
            gameObjectList.Add(labelY.gameObject);

            RectTransform dashY = Instantiate(dashTemplateX);
            dashY.SetParent(graphContainer, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(-4f, normalizedValue * graphHeight);
            gameObjectList.Add(dashY.gameObject);
        }*/
    }
    private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB, Color color)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = color;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 0.5f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance* 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, CollectionsExtensions.GetAngle(dir) * 180 / Mathf.PI);
        return gameObject;
    }
}
