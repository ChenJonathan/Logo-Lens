using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;

public class nasdaq : MonoBehaviour {

    private string test = "";
    private int layer = 0;

	// Use this for initialization
	void Start () {
        callNasdaqAPI("09/16/2016", "09/16/2016", "AAPL,MSFT,GOOG");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void callNasdaqAPI(string startDate, string endDate, string tickers)
    {
        string url = "http://ws.nasdaqdod.com/v1/NASDAQAnalytics.asmx/GetEndOfDayData";
        WWWForm form = new WWWForm();
        form.AddField("_Token", "247F80E1279F451499B6D68857FA0A93");
        form.AddField("Symbols", tickers);
        form.AddField("StartDate", startDate);
        form.AddField("EndDate", endDate);
        form.AddField("MarketCenters", "");
        WWW www = new WWW(url, form);
        StartCoroutine(WaitForRequest(www));
    }

    public IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        if (www.error == null)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(www.text);

            int isFirstTime = 0;
            XmlNode ArrayOfEndOfDayPriceCollection = doc.LastChild;
            foreach (XmlNode EndOfDayPriceCollection in ArrayOfEndOfDayPriceCollection.ChildNodes)
            {
                string symbol = EndOfDayPriceCollection.ChildNodes[3 - isFirstTime].InnerText;
                XmlNode Prices = EndOfDayPriceCollection.ChildNodes[4 - isFirstTime];
                XmlNode EndOfDayPrice = Prices.ChildNodes[0];
                string open = EndOfDayPrice.ChildNodes[2].InnerText;
                string close = EndOfDayPrice.ChildNodes[5].InnerText;

                Debug.Log(symbol + ": " + open + " to " + close);

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
