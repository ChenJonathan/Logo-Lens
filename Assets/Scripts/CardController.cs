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

    public void UpdateCard(Card card, string ticker, DateTime startDate, DateTime endDate)
    {
        // Update card: Part I
        card.SetElementText("Ticker", ticker);

        // NASDAQ API request
        string url = "http://ws.nasdaqdod.com/v1/NASDAQAnalytics.asmx/GetEndOfDayData";
        WWWForm form = new WWWForm();
        form.AddField("_Token", "D37747623D414496860789A99B4F28BA");
        form.AddField("Symbols", ticker);
        form.AddField("StartDate", (startDate.Month + "").PadLeft(2, '0') + "/" + (startDate.Day + "").PadLeft(2, '0') + "/" + startDate.Year);
        form.AddField("EndDate", (endDate.Month + "").PadLeft(2, '0') + "/" + (endDate.Day + "").PadLeft(2, '0') + "/" + endDate.Year);
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
        
        if(!ticker.Equals(""))
        {
            DateTime start = DateTime.Now.AddDays(-8);
            DateTime end = DateTime.Now.AddDays(-1);
            UpdateCard(card, ticker, start, end);
        }
        else
        {
            // TODO
        }
    }

    private void HandleNASDAQResponse(Card card, string xml)
    {
        // Parse XML
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        Debug.Log(xml);
        XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
        manager.AddNamespace("NASDAQ", "http://ws.nasdaqdod.com/services/v1/");
        string ticker = doc.SelectSingleNode("//NASDAQ:Symbol", manager).InnerText;
        XmlNodeList pricesList = doc.SelectSingleNode("//NASDAQ:Prices", manager).SelectNodes(".//NASDAQ:EndOfDayPrice", manager);
        float positionX = -16f;
        List<Vector2> points = new List<Vector2>();
        for(int i = 0; i < 10; i++)
        {
            points.Add(new Vector2(i, i));
        }

        // Update basic card components: Part II
        card.SetElementText("Date", "12/7/2016");
        card.SetElementText("Open Price", "$" + points[0].y.ToString("F2"));
        card.SetElementText("Close Price", "$" + points[points.Count - 1].y.ToString("F2"));
        card.SetElementText("Percent Change", ((points[points.Count - 1].y - points[0].y) / points[0].y).ToString("F3") + "%");

        // Set percent change text color
        if(points[0].y > points[points.Count - 1].y)
        {
            card.SetElementColor("Percent Change", Color.red);
        }
        else if(points[0].y < points[points.Count - 1].y)
        {
            card.SetElementColor("Percent Change", Color.green);
        }
        else
        {
            card.SetElementColor("Percent Change", Color.white);
        }

        // Update graph
        card.SetGraphPoints(points);
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
            Debug.Log("Error: " + www.error + "\n" + www.text);
        }
    }

    #endregion
}