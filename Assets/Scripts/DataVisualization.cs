using UnityEngine;

public class DataVisualization : MonoBehaviour
{
    [SerializeField]
    private GameObject signPrefab;

    public void DisplayCard(string ticker, double start, double end)
    {
        GameObject temp = (GameObject)Instantiate(signPrefab, transform.position + transform.forward * 1, Quaternion.identity);
        (temp.transform.FindChild("Ticker").GetComponent<TextMesh>()).text = ticker;
        (temp.transform.FindChild("Date").GetComponent<TextMesh>()).text = "9/16/2016";
        (temp.transform.FindChild("Open Price").GetComponent<TextMesh>()).text = "$" + start.ToString("F2");
        (temp.transform.FindChild("Close Price").GetComponent<TextMesh>()).text = "$" + end.ToString("F2");
        (temp.transform.FindChild("Percent Change").GetComponent<TextMesh>()).text = ((end - start) / start).ToString("F3") + "%";

        // Make the text red if the price went down
        if (start > end)
        {
            (temp.transform.FindChild("Percent Change").GetComponent<TextMesh>()).color = Color.red;
        }
        // Make the text green if the price went up
        else if (start < end)
        {
            (temp.transform.FindChild("Percent Change").GetComponent<TextMesh>()).color = Color.green;
        }
        // Make the text black if the price stayed the same
        else
        {
            (temp.transform.FindChild("Percent Change").GetComponent<TextMesh>()).color = Color.black;
        }
    }
}