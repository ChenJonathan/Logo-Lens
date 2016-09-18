using UnityEngine;

public class DataVisualization : MonoBehaviour
{
    [SerializeField]
    TextMesh signPrefab;

    public void DisplayCard(string ticker, double start, double end)
    {
        TextMesh temp = (TextMesh)Instantiate(signPrefab, transform.position + transform.forward * 10, Quaternion.identity);
        temp.text = ticker + ": $" + start.ToString("F2") + ", $" + end.ToString("F2");
    }
}