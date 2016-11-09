using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public LineRenderer GraphLinePrefab;

    private Camera mainCamera;
    private Vector3 offset;

    private readonly float graphMinX = -8f;
    private readonly float graphMaxX = 8f;
    private readonly float graphMinY = -4f;
    private readonly float graphMaxY = 4f;

    public void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        offset = transform.position - mainCamera.transform.position;
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
    }

    public void Update()
    {
        transform.position = mainCamera.transform.position + offset;
    }

    public void SetElementText(string element, string value)
    {
        transform.FindChild(element).GetComponent<TextMesh>().text = value;
    }

    public void SetElementColor(string element, Color value)
    {
        transform.FindChild(element).GetComponent<TextMesh>().color = value;
    }

    public void SetGraphPoints(List<Vector2> points)
    {
        // Destroy previous graph
        GameObject graph = transform.FindChild("Graph").gameObject;
        foreach(LineRenderer graphLine in graph.GetComponentsInChildren<LineRenderer>())
        {
            Destroy(graphLine.gameObject);
        }

        // TODO Calculate scale
        for(int i = 0; i < points.Count; i++)
        {
            points[i] = new Vector2(points[i].x, points[i].y - 775);
        }
        
        // Instantiate lines
        for(int i = 0; i < points.Count - 1; i++)
        {
            LineRenderer graphLine = GameObject.Instantiate(GraphLinePrefab);
            graphLine.transform.parent = graph.transform;
            graphLine.SetPosition(0, transform.position + (Vector3)points[i]);
            graphLine.SetPosition(1, transform.position + (Vector3)points[i + 1]);
        }
    }
}