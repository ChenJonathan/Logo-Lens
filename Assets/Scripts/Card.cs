using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Card : MonoBehaviour
{
    public GameObject Center;
    public GameObject Top;
    public GameObject Bottom;
    public GameObject Left;
    public GameObject Right;

    public GameObject Loading;
    public GameObject Graph;
    public GameObject Labels;
    public GameObject Points;

    public Text Label;
    public Collider Point;

    public enum TimeRange { Day = 0, ThreeDay = 1, Week = 2, TwoWeek = 3, Month = 4}
    private Dictionary<TimeRange, List<GraphPoint>> nasdaqData = new Dictionary<TimeRange, List<GraphPoint>>();

    [HideInInspector]
    public string Ticker;
    [HideInInspector]
    public TimeRange Range;
    [HideInInspector]
    public int Busy; // 0 if not busy
    [HideInInspector]
    public GameObject prevPoint;
    
    private Vector3 offset;

    private readonly float graphMinX = -3.2f;
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

        Center.transform.localScale = Top.transform.localScale = Bottom.transform.localScale = Left.transform.localScale = Right.transform.localScale =  Vector3.zero;

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

            Top.transform.localScale = Bottom.transform.localScale = tempScale;
            Top.GetComponent<Image>().color = Bottom.GetComponent<Image>().color = tempColor;

            tempPosition.y = 3.73f + 1.47f / 2 * (i - 1);
            Top.transform.localPosition = tempPosition;
            Bottom.transform.localPosition = -tempPosition;

            yield return new WaitForSeconds(0.005f);
        }

        Busy--;
    }

    public IEnumerator Disappear()
    {
        yield return new WaitUntil(() => Busy == 0);
        Busy++;

        if (prevPoint)
        {
            prevPoint.GetComponent<Image>().enabled = false;
        }

        Vector3 tempPosition = Vector3.zero;
        Vector3 tempScale = Vector3.one;
        Color tempColor = Color.white;
        
        for(float i = 1f; i >= 0f; i = Mathf.Lerp(i, -0.01f, 0.15f))
        {
            tempScale.y = i;
            tempColor.a = i;

            Top.transform.localScale = Bottom.transform.localScale = tempScale;
            Top.GetComponent<Image>().color = Bottom.GetComponent<Image>().color = tempColor;

            tempPosition.y = 3.73f + 1.47f / 2 * (i - 1);
            Top.transform.localPosition = tempPosition;
            Bottom.transform.localPosition = -tempPosition;

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

    public void SetTopElementText(string element, string value)
    {
        Top.transform.FindChild(element).GetComponent<Text>().text = value;
    }

    public void SetTopElementColor(string element, Color value)
    {
        Top.transform.FindChild(element).GetComponent<Text>().material.color = value;
    }

    public void SetBottomElementText(string element, string value)
    {
        Bottom.transform.FindChild(element).GetComponent<Text>().text = value;
    }

    public void SetBottomElementColor(string element, Color value)
    {
        if (value == Color.red)
        {
            Bottom.transform.FindChild(element).GetComponent<Text>().material = Resources.Load("Red Text.mat") as Material;
            Bottom.transform.FindChild(element).GetComponent<Text>().material.color = Color.red;
        }
        else if (value == Color.green)
        {
            Bottom.transform.FindChild(element).GetComponent<Text>().material = Resources.Load("Green Text.mat") as Material;
            Bottom.transform.FindChild(element).GetComponent<Text>().material.color = Color.green;
        }
        else if (value == Color.white)
        {
            Bottom.transform.FindChild(element).GetComponent<Text>().material = Resources.Load("White Text.mat") as Material;
            Bottom.transform.FindChild(element).GetComponent<Text>().material.color = Color.white;
        }
    }

    public void SetChange(float change)
    {
        // Update the card with the change
        string changeStr = change.ToString("0.00");
        if(change > 0)
        {
            SetTopElementText("Ticker", Ticker + ": + $" + changeStr);
            SetTopElementColor("Ticker", Color.green);
        }
        else
        {
            changeStr = changeStr.Substring(1);
            SetTopElementText("Ticker", Ticker + ": - $" + changeStr);
            SetTopElementColor("Ticker", Color.red);
        }
    }

    public void SetLoading(bool active)
    {
        Loading.gameObject.SetActive(active);
    }

    public void ViewTimeRange(TimeRange range)
    {
        Debug.Log("Viewing timerange " + range);
        Range = range;

        if (!nasdaqData.ContainsKey(range))
        {
            SetLoading(true);
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
        SetLoading(false);
        List<GraphPoint> points = nasdaqData[range];

        // Set up all the text   
        DateTime endDate = DateTime.Now;
        DateTime startDate = endDate;
        switch (range)
        {
            case TimeRange.Day:
                startDate = endDate.AddDays(-1);
                break;
            case TimeRange.ThreeDay:
                startDate = endDate.AddDays(-3);
                break;
            case TimeRange.Week:
                startDate = endDate.AddDays(-7);
                break;
            case TimeRange.TwoWeek:
                startDate = endDate.AddDays(-14);
                break;
            case TimeRange.Month:
                startDate = endDate.AddDays(-30);
                break;
            default:
                Debug.Log("WARNING: Default TimeRange");
                startDate = endDate;
                break;
        }

        // Set basic card elements
        SetTopElementColor("Ticker", Color.white);
        SetTopElementText("Date", Util.FormatDate(startDate) + " to " + Util.FormatDate(endDate));

        if (points.Count > 0)
        {
            // Set the change text
            SetChange(points.Last().Value - points.First().Value);
        }
        else
        {
            SetTopElementText("Ticker", Ticker + ": No trades found");
            SetTopElementText("Date", "");
        }

        SetGraphPoints(points);
    }

    public void UpdateNasdaqData(List<GraphPoint> points, TimeRange range)
    {
        nasdaqData.Add(range, points);
    }

    public void SetGraphPoints(List<GraphPoint> points)
    {
        // Destroy previous graph labels and points
        Graph.GetComponent<LineRenderer>().numPositions = 0;
        foreach (Text label in Labels.GetComponentsInChildren<Text>())
            Destroy(label.gameObject);
        foreach (Collider point in Points.GetComponentsInChildren<Collider>())
            Destroy(point.gameObject);

        if (points.Count == 0)
            return;

        // Determine min and max value for Y scale
        float minVal = float.MaxValue;
        float maxVal = 0;
        foreach(GraphPoint point in points)
        {
            if(point.Value > maxVal)
                maxVal = point.Value;
            if(point.Value < minVal)
                minVal = point.Value;
        }

        // Calculate scale multipliers
        float xScale = (graphMaxX - graphMinX) / (points.Count - 1);
        float yScale = (graphMaxY - graphMinY) / (maxVal - minVal);

        StartCoroutine(SpawnXLabels(points, xScale, yScale));
        StartCoroutine(SpawnYLabels(points, minVal, maxVal));
        StartCoroutine(SpawnPoints(points, xScale, yScale, minVal, maxVal));
    }

    private IEnumerator SpawnXLabels(List<GraphPoint> points, float xScale, float yScale)
    {
        string lastLabel = "";
        for(int i = 0; i < points.Count; i++)
        {
            float x = graphMinX + i * xScale;

            string temp = "<b>" + points[i].DateTime.Substring(0, 5) + "</b>";
            if((int)Range < 2)
                temp = "<b>" + points[i].DateTime.Substring(11, 5) + "</b>";
            if(!lastLabel.Equals(temp))
            {
                if((int)Range < 3 || (Range == TimeRange.TwoWeek && i % 2 == 0) || (Range == TimeRange.Month && i % 4 == 0))
                {
                    Text text = Instantiate(Label);
                    text.transform.SetParent(Labels.transform);
                    text.transform.localPosition = new Vector3(x, -2.4f, 0);
                    text.transform.localRotation = Quaternion.identity;
                    text.text = lastLabel = temp;
                }
            }

            yield return new WaitForSeconds(1f / points.Count);
        }
    }

    private IEnumerator SpawnYLabels(List<GraphPoint> points, float minVal, float maxVal)
    {
        for(int i = 0; i < 7; i++)
        {
            float y = graphMinY + (graphMaxY - graphMinY) * (i / 6f);
            float val = minVal + (maxVal - minVal) * (i / 6f);

            // Spawn y-axis labels
            Text text = Instantiate(Label);
            text.transform.SetParent(Labels.transform);
            text.transform.localPosition = new Vector3(-3.9f, y, 0);
            text.transform.localRotation = Quaternion.identity;
            text.transform.localScale = new Vector3(0.001f, 0.001f, 1);
            text.text = "<b>" + val.ToString("F2") + "</b>";

            yield return new WaitForSeconds(1f / 7);
        }
    }

    private IEnumerator SpawnPoints(List<GraphPoint> points, float xScale, float yScale, float minVal, float maxVal)
    {
        LineRenderer graph = Graph.GetComponent<LineRenderer>();

        for(int i = 0; i < points.Count; i++)
        {
            // Opening point at the initial time slice
            float x = graphMinX + i * xScale;
            float y = graphMinY + (points[i].Value - minVal) * yScale;
            graph.numPositions = i + 1;
            graph.SetPosition(i, new Vector3(x, y, 0.1f));

            // Create the sphere for raycasting
            
            GameObject sphere = Instantiate(Point).gameObject;
            sphere.GetComponent<RectTransform>().sizeDelta = new Vector2(0.25f * 1.8f/xScale, 0.25f * 1.8f/xScale);
            sphere.transform.SetParent(Points.transform);
            sphere.transform.localPosition = new Vector3(x, y, 0);
            sphere.transform.localScale = new Vector3(xScale, xScale, xScale);
            GraphPoint gp = sphere.GetComponent<GraphPoint>();
            gp.DateTime = points[i].DateTime;
            gp.Value = points[i].Value;

            yield return new WaitForSeconds(1f / points.Count);
        }
    }
}