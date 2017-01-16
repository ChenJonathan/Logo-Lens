using UnityEngine;
using System.Collections;

public class GraphPoint : MonoBehaviour {

    public string DateTime;
    public float Value;

    public GraphPoint(string DateTime, float Value)
    {
        this.DateTime = DateTime;
        this.Value = Value;
    }
}
