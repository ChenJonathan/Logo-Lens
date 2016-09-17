using UnityEngine;
using System.Collections;
using System.Xml;

public class Test : MonoBehaviour
{
    public static string test = "";

	public void Start()
    {
        string url = "http://ws.nasdaqdod.com/v1/NASDAQAnalytics.asmx/GetSummarizedTrades";
        WWWForm form = new WWWForm();
        form.AddField("_Token", "247F80E1279F451499B6D68857FA0A93");
        form.AddField("Symbols", "AAPL,MSFT");
        form.AddField("StartDateTime", "2/1/2015 00:00:00.000");
        form.AddField("EndDateTime", "2/18/2015 23:59:59.999");
        form.AddField("MarketCenters", "");
        form.AddField("TradePrecision", "Hour");
        form.AddField("TradePeriod", "1");
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