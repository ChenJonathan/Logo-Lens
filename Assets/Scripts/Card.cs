using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public GameObject Center;
    public GameObject Left;
    public GameObject Right;
    public GameObject Bottom;

    public LineRenderer GraphLinePrefab;

    public enum TimeRange { Day = 0, Week = 1, Month = 2, Year = 3 }

    [HideInInspector]
    public string Ticker;
    [HideInInspector]
    public TimeRange Range;
    
    private Vector3 offset;

    private readonly float graphMinX = -4.565f;
    private readonly float graphMaxX = 4.565f;
    private readonly float graphMinY = -2.83f;
    private readonly float graphMaxY = 2.83f;

    public void Start()
    {
        offset = transform.position - Camera.main.transform.position;
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }

    public void Update()
    {
        transform.position = Camera.main.transform.position + offset;
    }

    public void SetElementText(string element, string value)
    {
        Bottom.transform.FindChild(element).GetComponent<Text>().text = value;
    }

    public void SetElementColor(string element, Color value)
    {
        Bottom.transform.FindChild(element).GetComponent<Text>().color = value;
    }

    public void SetTimeRange(TimeRange range)
    {
        Range = range;
        CardController.Instance.UpdateCard(this, Ticker, range);
    }

    public void SetGraphPoints(List<Vector2> points)
    {
        // Destroy previous graph
        foreach(LineRenderer graphLine in Center.GetComponentsInChildren<LineRenderer>())
        {
            Destroy(graphLine.gameObject);
        }

        // TODO Calculate scale and add some constant y-value so that the graph is on top of the card
        for(int i = 0; i < points.Count; i++)
        {
            points[i] = new Vector2(points[i].x * 3 / 2 + graphMinX, points[i].y % graphMaxY);
        }
        
        // Instantiate lines
        for(int i = 0; i < points.Count - 1; i++)
        {
            LineRenderer graphLine = Instantiate(GraphLinePrefab);
            graphLine.transform.parent = Center.transform;
            graphLine.SetPosition(0, transform.position + (Vector3)points[i]);
            graphLine.SetPosition(1, transform.position + (Vector3)points[i + 1]);
        }
    }
}