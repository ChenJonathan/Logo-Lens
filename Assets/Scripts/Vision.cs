using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using SimpleJSON;

public class Vision : MonoBehaviour
{
	public void Start()
    {
        string image64 = "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";
        DetectImage(image64);
    }

    public void DetectImage(string image)
    {
        string url = "https://vision.googleapis.com/v1/images:annotate?key=AIzaSyDNiOUrvRHj0anRBsC1NrrU7v8wwA90v8E";
        string json = "{\"requests\": [ { \"image\" : {\"content\":\"" + image + "\"},\"features\":[{\"type\":\"LOGO_DETECTION\",\"maxResults\":20}]}]}";
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        WWW www = new WWW(url, Encoding.ASCII.GetBytes(json.ToCharArray()), headers);
        StartCoroutine(WaitForRequest(www));
    }

    public IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        if(www.error == null)
        {
            List<string> possibleCompanies = new List<string>();

            var N = JSON.Parse(www.text);
            var responses = N["responses"];
            if (responses.Count == 0)
            {
                Debug.Log("ERROR no matching logo found!");
                // TODO catch to display error message
                yield break;
            }
            var array = responses[0]["logoAnnotations"];

            for (int i = 0; i < array.Count; i++)
            {
                var description = array[i]["description"];
                Debug.Log("Description = " + description);
                possibleCompanies.Add(description);
            }

            string ticker = "";
            GetTicker tickerScript = GetComponent<GetTicker>();
            int j = 0;

            while (ticker.Equals(""))
            {
                if (j >= possibleCompanies.Count)
                {
                    Debug.Log("ERROR no ticker found!");
                    yield break;
                }
                ticker = tickerScript.getTicker(possibleCompanies[j]);
                Debug.Log(possibleCompanies[j] + " = " + ticker);
                j++;
            }

            Debug.Log("Found! " + ticker);
            GetStockData stockScript = GetComponent<GetStockData>();
            stockScript.CallNasdaqAPI("09/16/2016", "09/16/2016", ticker);
        }
        else
        {
            Debug.Log("Error: " + www.error);
        }
    }
}