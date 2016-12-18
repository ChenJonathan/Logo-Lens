using System;
using System.Collections;
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
    [HideInInspector]
    public int Busy; // 0 if not busy
    
    private Vector3 offset;

    private readonly float graphMinX = -4.565f;
    private readonly float graphMaxX = 4.565f;
    private readonly float graphMinY = -2.83f;
    private readonly float graphMaxY = 2.83f;

    public void Start()
    {
        offset = transform.position - Camera.main.transform.position;
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

        Busy++;
        StartCoroutine(Appear());
    }

    public void Update()
    {
        transform.position = Camera.main.transform.position + offset;
    }

    public IEnumerator Appear()
    {
        Vector3 tempPosition = Vector3.zero;
        Vector3 tempScale = Vector3.one;
        Color tempColor = Color.white;

        Center.transform.localScale = Left.transform.localScale = Right.transform.localScale = Bottom.transform.localScale = Vector3.zero;

        for(float i = 0f; i <= 1f; i = Mathf.Lerp(i, 1.01f, 0.15f))
        {
            tempScale.x = tempScale.y = 0.5f + i / 2;
            tempColor.a = i;

            Center.transform.localScale = tempScale;
            Center.GetComponent<Image>().color = tempColor;

            yield return new WaitForSeconds(0.005f);
        }
        tempScale.y = 1;
        for(float i = 0f; i <= 1f; i = Mathf.Lerp(i, 1.01f, 0.15f))
        {
            tempScale.x = i;
            tempColor.a = i;

            Left.transform.localScale = Right.transform.localScale = tempScale;
            Left.GetComponent<Image>().color = Right.GetComponent<Image>().color = tempColor;

            tempPosition.x = -6.1f - 2.78f / 2 * (i - 1);
            Left.transform.localPosition = tempPosition;
            tempPosition.x *= -1;
            Right.transform.localPosition = tempPosition;

            yield return new WaitForSeconds(0.005f);
        }
        tempPosition.x = 0;
        tempScale.x = 1;
        for(float i = 0f; i <= 1f; i = Mathf.Lerp(i, 1.01f, 0.15f))
        {
            tempScale.y = i;
            tempColor.a = i;

            Bottom.transform.localScale = tempScale;
            Bottom.GetComponent<Image>().color = tempColor;

            tempPosition.y = -3.73f - 1.47f / 2 * (i - 1);
            Bottom.transform.localPosition = tempPosition;

            yield return new WaitForSeconds(0.005f);
        }

        Busy--;
    }

    public IEnumerator Disappear()
    {
        yield return new WaitUntil(() => Busy == 0);
        Busy++;

        Vector3 tempPosition = Vector3.zero;
        Vector3 tempScale = Vector3.one;
        Color tempColor = Color.white;
        
        for(float i = 1f; i >= 0f; i = Mathf.Lerp(i, -0.01f, 0.15f))
        {
            tempScale.y = i;
            tempColor.a = i;

            Bottom.transform.localScale = tempScale;
            Bottom.GetComponent<Image>().color = tempColor;

            tempPosition.y = -3.73f - 1.47f / 2 * (i - 1);
            Bottom.transform.localPosition = tempPosition;

            yield return new WaitForSeconds(0.005f);
        }
        tempPosition.y = 0;
        tempScale.y = 1;
        for(float i = 1f; i >= 0f; i = Mathf.Lerp(i, -0.01f, 0.15f))
        {
            tempScale.x = i;
            tempColor.a = i;

            Left.transform.localScale = Right.transform.localScale = tempScale;
            Left.GetComponent<Image>().color = Right.GetComponent<Image>().color = tempColor;

            tempPosition.x = -6.1f - 2.78f / 2 * (i - 1);
            Left.transform.localPosition = tempPosition;
            tempPosition.x *= -1;
            Right.transform.localPosition = tempPosition;

            yield return new WaitForSeconds(0.005f);
        }
        for(float i = 1f; i >= 0f; i = Mathf.Lerp(i, -0.01f, 0.15f))
        {
            tempScale.x = tempScale.y = 0.5f + i / 2;
            tempColor.a = i;

            Center.transform.localScale = tempScale;
            Center.GetComponent<Image>().color = tempColor;

            yield return new WaitForSeconds(0.005f);
        }

        Destroy(gameObject);
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
            graphLine.SetPosition(0, transform.localPosition + (Vector3)points[i]);
            graphLine.SetPosition(1, transform.localPosition + (Vector3)points[i + 1]);
        }
    }
}