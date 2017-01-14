using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Card : MonoBehaviour
{
    public GameObject Center;
    public GameObject Left;
    public GameObject Right;
    public GameObject Bottom;

    public LineRenderer GraphLinePrefab;

    public enum TimeRange { Day = 0, ThreeDay = 1, Week = 2, TwoWeek = 3, Month = 4}
    private Dictionary<TimeRange, List<GraphPoint>> nasdaqData = new Dictionary<TimeRange, List<GraphPoint>>();

    [HideInInspector]
    public string Ticker;
    [HideInInspector]
    public TimeRange Range;
    [HideInInspector]
    public int Busy; // 0 if not busy
    
    private Vector3 offset;

    private readonly float graphMinX = -4.2f;
    private readonly float graphMaxX = 4.2f;
    private readonly float graphMinY = -2.6f;
    private readonly float graphMaxY = 2.6f;

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

    public void ViewTimeRange(TimeRange range)
    {
        Debug.Log("Viewing timerange " + range);
        this.Range = range;

        if (!nasdaqData.ContainsKey(range))
        {
            Center.transform.FindChild("Loading").gameObject.SetActive(true);
            StartCoroutine(WaitForData(range));
        }
        else
        {
            DisplayData(range);
        }
    }

    private IEnumerator WaitForData(TimeRange range)
    {
        Busy++;
        yield return new WaitUntil(() => nasdaqData.ContainsKey(range));
        Busy--;
        DisplayData(range);
    }

    private void DisplayData(TimeRange range)
    {
        Center.transform.FindChild("Loading").gameObject.SetActive(false);

        // Set up all the text   
        DateTime endDate = DateTime.Now;
        DateTime startDate = endDate;

        // Determine values for display variables based on the passed in time range
        switch (range)
        {
            case Card.TimeRange.Day:
                startDate = endDate.AddDays(-1);
                break;
            case Card.TimeRange.ThreeDay:
                startDate = endDate.AddDays(-3);
                break;
            case Card.TimeRange.Week:
                startDate = endDate.AddDays(-7);
                break;
            case Card.TimeRange.TwoWeek:
                startDate = endDate.AddDays(-14);
                break;
            case Card.TimeRange.Month:
                startDate = endDate.AddDays(-30);
                break;
            default:
                Debug.Log("WARNING: Default TimeRange");
                startDate = endDate;
                break;
        }

        // Set basic card elements
        this.SetElementColor("Ticker", Color.white);
        this.SetElementText("Date", Util.FormatDate(startDate) + " to " + Util.FormatDate(endDate));

        List<GraphPoint> points = nasdaqData[range];

        // Set the change text
        this.SetChange(points.Last().Value - points.First().Value);

        this.SetGraphPoints(points);
    }

    public void UpdateNasdaqData(List<GraphPoint> points, TimeRange range)
    {
        nasdaqData.Add(range, points);
    }

    public void SetGraphPoints(List<GraphPoint> points)
    {
        // Destroy previous graph
        foreach(LineRenderer oldGraphLine in Center.GetComponentsInChildren<LineRenderer>())
        {
            Destroy(oldGraphLine.gameObject);
        }

        if (points.Count == 0)
        {
            // TODO error message
            Debug.Log("No points to graph!");
            return;
        }

        // Determine min and max value for Y scale
        float minVal = float.MaxValue;
        float maxVal = 0;
        foreach (GraphPoint point in points)
        {
            if (point.Value > maxVal)
            {
                maxVal = point.Value;
            }
            if (point.Value < minVal)
            {
                minVal = point.Value;
            }
        }

        // Calculate scale multipliers
        float xScale = (graphMaxX - graphMinX) / (points.Count - 1);
        float yScale = (graphMaxY - graphMinY) / (maxVal - minVal);
        //Debug.Log("Plotting " + points.Count + " points with xScale " + xScale);

        // Plot the opening -> closing points by instaniating lines
        int i = 0;
        LineRenderer graphLine = Instantiate(GraphLinePrefab);
        graphLine.transform.parent = Center.transform;
        graphLine.numPositions = points.Count;
        while(i < points.Count)
        {
            // Opening point at the initial time slice
            float x = graphMinX + i * xScale;
            float y = graphMinY + (points[i].Value - minVal) * yScale;
            graphLine.SetPosition(i, transform.localPosition + new Vector3(x, y, 0));

            i++;
        }
    }
}