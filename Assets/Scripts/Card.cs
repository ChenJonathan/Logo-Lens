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

    public Text Label;

    public enum TimeRange { Today = 0, Day = 1, ThreeDay = 2, Week = 3, TwoWeek = 4, Month = 5}

    [HideInInspector]
    public string Ticker;
    [HideInInspector]
    public TimeRange Range;
    [HideInInspector]
    public int Busy; // 0 if not busy
    
    private Vector3 offset;

    private readonly float graphMinX = -3f;
    private readonly float graphMaxX = 4f;
    private readonly float graphMinY = -1.5f;
    private readonly float graphMaxY = 2.5f;

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

    public void SetChange(float change)
    {
        // Update the card with the change
        string changeStr = change.ToString("0.00");
        if(change > 0)
        {
            SetElementText("Ticker", Ticker + ": + $" + changeStr);
            SetElementColor("Ticker", Color.green);
        }
        else
        {
            changeStr = changeStr.Substring(1);
            SetElementText("Ticker", Ticker + ": - $" + changeStr);
            SetElementColor("Ticker", Color.red);
        }
    }

    public void SetTimeRange(TimeRange range)
    {
        Range = range;
        Center.transform.FindChild("Loading").gameObject.SetActive(true);
        CardController.Instance.UpdateCard(this, Ticker, range);
    }

    public void SetGraphPoints(List<GraphPoint> points)
    {
        Center.transform.FindChild("Loading").gameObject.SetActive(false);
        foreach(Text text in Center.transform.GetComponentsInChildren<Text>())
            Destroy(text.gameObject);
        if(points.Count == 0)
        {
            // TODO error message
            Debug.Log("No points to graph!");
            return;
        }

        // Determine min and max value for Y scale
        float minVal = float.MaxValue;
        float maxVal = 0;
        foreach(GraphPoint point in points)
        {
            if(point.Value > maxVal)
            {
                maxVal = point.Value;
            }
            if(point.Value < minVal)
            {
                minVal = point.Value;
            }
        }

        // Calculate scale multipliers
        float xScale = (graphMaxX - graphMinX) / (points.Count - 1);
        float yScale = (graphMaxY - graphMinY) / (maxVal - minVal);
        Debug.Log("Plotting " + points.Count + " points with xScale " + xScale);

        // Plot the opening -> closing points
        LineRenderer graph = Center.transform.FindChild("Graph").GetComponent<LineRenderer>();
        graph.numPositions = points.Count;
        string lastLabel = "";
        for(int i = 0; i < points.Count; i++)
        {
            // Opening point at the initial time slice
            float x = graphMinX + i * xScale;
            float y = graphMinY + (points[i].Value - minVal) * yScale;
            graph.SetPosition(i, new Vector3(x, y, 0));

            // Spawn x-axis labels
            Text text = Instantiate(Label);
            text.transform.SetParent(Center.transform);
            text.transform.localPosition = new Vector3(x, text.transform.position.y, text.transform.position.z);
            string temp = "<b>" + points[i].DateTime.Substring(0, 5) + "</b>";
            if(!lastLabel.Equals(temp))
                text.text = lastLabel = temp;
        }

        for(int i = 0; i < 5; i++)
        {
            float y = graphMinY + (graphMaxY - graphMinY) * (i / 4f);

            // Spawn y-axis labels
            Text text = Instantiate(Label);
            text.transform.SetParent(Center.transform);
            text.transform.localPosition = new Vector3(text.transform.position.x, y, text.transform.position.z);
            text.text = "<b>$" + points[i].Value.ToString("F2") + "</b>";
            text.transform.localScale = new Vector3(0.001f, 0.001f, 1);
        }
    }
}