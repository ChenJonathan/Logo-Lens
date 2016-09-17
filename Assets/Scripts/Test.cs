using UnityEngine;
using System.Collections;
using System.Xml;

public class Test : MonoBehaviour
{
    public static string test = "";

	public void Start()
    {
        string url = "http://ws.nasdaqdod.com/v1/NASDAQAnalytics.asmx/GetEndOfDayData";
        WWWForm form = new WWWForm();
        form.AddField("_Token", "247F80E1279F451499B6D68857FA0A93");
        form.AddField("Symbols", "AAPL,MSFT,GOOG");
        form.AddField("StartDate", "9/16/2016");
        form.AddField("EndDate", "9/16/2016");
        form.AddField("MarketCenters", "");
        WWW www = new WWW(url, form);
        StartCoroutine(WaitForRequest(www));
    }

    public IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        if(www.error == null)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(www.text);
            Log(doc.FirstChild);
            Log(doc.LastChild);
            Debug.Log("Test:" + test);
        }
        else
        {
            Debug.Log("Error: " + www.error);
        }
    }

    public void Log(XmlNode node)
    {
        if(node.HasChildNodes)
        {
            for(int i = 0; i < node.ChildNodes.Count; i++)
            {
                Log(node.ChildNodes[i]);
            }
        }
        Debug.Log(node.Value);
        test += node.Value + "\n";
    }
}