using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [SerializeField]
    private GameObject signPrefab;

    public struct StockDataPoint
    {
        public string ticker;
        public float start;
        public float end;

        public StockDataPoint(string ticker, float start, float end)
        {
            this.ticker = ticker;
            this.start = start;
            this.end = end;
        }
    }

    public void AddCard(string image)
    {
        Card card = ((GameObject)Instantiate(signPrefab, transform.position + transform.forward * 20, Quaternion.identity)).GetComponent<Card>();

        // Google Vision API request
        string url = "https://vision.googleapis.com/v1/images:annotate?key=AIzaSyDNiOUrvRHj0anRBsC1NrrU7v8wwA90v8E";
        string json = "{\"requests\": [ { \"image\" : {\"content\":\"" + image + "\"},\"features\":[{\"type\":\"LOGO_DETECTION\",\"maxResults\":20}]}]}";
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        WWW www = new WWW(url, Encoding.ASCII.GetBytes(json.ToCharArray()), headers);

        StartCoroutine(WaitForRequest(www, card, HandleGoogleVisionResponse));
    }

    public void UpdateCard(Card card, string ticker, string startDate, string endDate)
    {
        // Update card part I
        card.transform.FindChild("Ticker").GetComponent<TextMesh>().text = ticker;

        // NASDAQ API request
        string url = "http://ws.nasdaqdod.com/v1/NASDAQAnalytics.asmx/GetEndOfDayData";
        WWWForm form = new WWWForm();
        form.AddField("_Token", "D37747623D414496860789A99B4F28BA");
        form.AddField("Symbols", ticker);
        form.AddField("StartDate", startDate);
        form.AddField("EndDate", endDate);
        form.AddField("MarketCenters", "");
        WWW www = new WWW(url, form);

        StartCoroutine(WaitForRequest(www, card, HandleNASDAQResponse));
    }

    public void RemoveCard(Card card)
    {
        Destroy(card.gameObject);
    }

    #region API response handlers

    private void HandleGoogleVisionResponse(Card card, string json)
    {
        List<string> possibleCompanies = new List<string>();

        JSONNode node = JSON.Parse(json);
        JSONNode responses = node["responses"];
        if(responses.Count == 0)
        {
            Debug.Log("No matching logo found!");
            return;
        }
        JSONNode array = responses[0]["logoAnnotations"];

        for(int i = 0; i < array.Count; i++)
        {
            var description = array[i]["description"];
            possibleCompanies.Add(description);
        }
        
        string ticker = "";
        foreach(string company in possibleCompanies)
        {
            if(ticker.Equals(""))
            {
                ticker = TickerMap.getTicker(company);
                break;
            }
        }
        
        UpdateCard(card, ticker, "10/17/2016", "10/17/2016");
    }

    private void HandleNASDAQResponse(Card card, string xml)
    {
        List<StockDataPoint> points = new List<StockDataPoint>();

        // Parse XML
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        int isFirstTime = 0;
        XmlNode ArrayOfEndOfDayPriceCollection = doc.LastChild;
        foreach(XmlNode EndOfDayPriceCollection in ArrayOfEndOfDayPriceCollection.ChildNodes)
        {
            string symbol = EndOfDayPriceCollection.ChildNodes[3 - isFirstTime].InnerText;
            XmlNode Prices = EndOfDayPriceCollection.ChildNodes[4 - isFirstTime];
            XmlNode EndOfDayPrice = Prices.ChildNodes[0];
            string open = EndOfDayPrice.ChildNodes[2].InnerText;
            string close = EndOfDayPrice.ChildNodes[5].InnerText;
            
            points.Add(new StockDataPoint(symbol, float.Parse(open), float.Parse(close)));

            if(isFirstTime == 0)
            {
                isFirstTime = 1;
            }
        }

        // Update card components part II
        card.transform.FindChild("Date").GetComponent<TextMesh>().text = "10/17/2016";
        card.transform.FindChild("Open Price").GetComponent<TextMesh>().text = "$" + points[0].start.ToString("F2");
        card.transform.FindChild("Close Price").GetComponent<TextMesh>().text = "$" + points[0].end.ToString("F2");
        card.transform.FindChild("Percent Change").GetComponent<TextMesh>().text = ((points[0].end - points[0].start) / points[0].start).ToString("F3") + "%";

        // Make the text red if the price went down
        if(points[0].start > points[0].end)
        {
            card.transform.FindChild("Percent Change").GetComponent<TextMesh>().color = Color.red;
        }
        // Make the text green if the price went up
        else if(points[0].start < points[0].end)
        {
            card.transform.FindChild("Percent Change").GetComponent<TextMesh>().color = Color.green;
        }
        // Make the text black if the price stayed the same
        else
        {
            card.transform.FindChild("Percent Change").GetComponent<TextMesh>().color = Color.black;
        }
    }

    #endregion

    #region API request coroutine

    private IEnumerator WaitForRequest(WWW www, Card card, Action<Card, string> callback)
    {
        yield return www;

        if(www.error == null)
        {
            callback(card, www.text);
        }
        else
        {
            Debug.Log("Error: " + www.error);
        }
    }

    #endregion
}