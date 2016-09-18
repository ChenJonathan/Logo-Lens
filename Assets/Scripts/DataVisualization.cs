using UnityEngine;

public class DataVisualization : MonoBehaviour
{
    [SerializeField]
    TextMesh signPrefab;

    public void DisplayCard(string ticker, double start, double end)
    {
        TextMesh temp = (TextMesh) Instantiate(signPrefab, transform.position + transform.forward, Quaternion.identity);
        temp.text = ticker + ": $" + start.ToString("F2") + ", $" + end.ToString("F2");

        // Make the text red if the price went down
        if (start > end)
        {
            temp.color = Color.red;
        }
        // Make the text green if the price went up
        else if (start < end)
        {
            temp.color = Color.green;
        }
        // Make the text black if the price stayed the same
        else
        {
            temp.color = Color.black;
        }
    }
}