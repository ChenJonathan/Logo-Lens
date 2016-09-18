using UnityEngine;

public class DataVisualization : MonoBehaviour
{
    [SerializeField]
    GameObject signPrefab;

    public void DisplayCard(string ticker, double start, double end)
    {
        GameObject temp = (GameObject)Instantiate(signPrefab, transform.position + transform.forward * 10, Quaternion.identity);
        ((TextMesh)temp.transform.FindChild("Ticker").GetComponent<TextMesh>()).text = ticker;
        ((TextMesh)temp.transform.FindChild("Date").GetComponent<TextMesh>()).text = "9/16/2016";
        ((TextMesh)temp.transform.FindChild("Open Price").GetComponent<TextMesh>()).text = "$" + start.ToString("F2");
        ((TextMesh)temp.transform.FindChild("Close Price").GetComponent<TextMesh>()).text = "$" + end.ToString("F2");
        ((TextMesh)temp.transform.FindChild("Percent Change").GetComponent<TextMesh>()).text = ((end - start) / start).ToString("F3") + "%";

        // Make the text red if the price went down
        if (start > end)
        {
            ((TextMesh)temp.transform.FindChild("Percent Change").GetComponent<TextMesh>()).color = Color.red;
        }
        // Make the text green if the price went up
        else if (start < end)
        {
            ((TextMesh)temp.transform.FindChild("Percent Change").GetComponent<TextMesh>()).color = Color.green;
        }
        // Make the text black if the price stayed the same
        else
        {
            ((TextMesh)temp.transform.FindChild("Percent Change").GetComponent<TextMesh>()).color = Color.black;
        }
    }
}