using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;

public class GetStockData : MonoBehaviour
{
    public void CallNasdaqAPI(string startDate, string endDate, string ticker)
    {
        string url = "http://ws.nasdaqdod.com/v1/NASDAQAnalytics.asmx/GetEndOfDayData";
        WWWForm form = new WWWForm();
        form.AddField("_Token", "247F80E1279F451499B6D68857FA0A93");
        form.AddField("Symbols", ticker);
        form.AddField("StartDate", startDate);
        form.AddField("EndDate", endDate);
        form.AddField("MarketCenters", "");
        WWW www = new WWW(url, form);
        GetComponent<DataVisualization>().DisplayCard("MSFT", 1, 2);
        // StartCoroutine(WaitForRequest(www));
    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        if (www.error == null)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(www.text);
            Debug.Log(www.text);
            int isFirstTime = 0;
            XmlNode ArrayOfEndOfDayPriceCollection = doc.LastChild;
            foreach (XmlNode EndOfDayPriceCollection in ArrayOfEndOfDayPriceCollection.ChildNodes)
            {
                string symbol = EndOfDayPriceCollection.ChildNodes[3 - isFirstTime].InnerText;
                XmlNode Prices = EndOfDayPriceCollection.ChildNodes[4 - isFirstTime];
                XmlNode EndOfDayPrice = Prices.ChildNodes[0];
                string open = EndOfDayPrice.ChildNodes[2].InnerText;
                string close = EndOfDayPrice.ChildNodes[5].InnerText;

                Debug.Log(symbol + " went from " + open + " to " + close);
                GetComponent<DataVisualization>().DisplayCard(symbol, float.Parse(open), float.Parse(close));

                if (isFirstTime == 0)
                {
                    isFirstTime = 1;
                }
            }
        }
        else
        {
            Debug.Log("Error: " + www.error);
        }
    }
}
