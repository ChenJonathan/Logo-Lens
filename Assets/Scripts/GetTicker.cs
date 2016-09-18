using UnityEngine;
using System.Collections.Generic;
using System.IO;
public class GetTicker : MonoBehaviour
{
    private Dictionary<string, string[]> stocks = new Dictionary<string, string[]>();
    public void Start()
    {
        TextAsset reader = Resources.Load("companylist") as TextAsset;
        string raw = reader.text;
        string[] lines = raw.Split('\n');
        for(int i = 0; i < lines.Length; i++)
        {
            var values = lines[i].Split(',');
            if(values.Length - 2 >= 0)
            {
                string[] info = new string[values.Length - 2];
                info[0] = values[0];
                for(int j = 2; j < values.Length - 1; j++)
                {
                    info[j - 1] = values[j];
                }
                if(!stocks.ContainsKey(values[1].ToLower().Replace(' ', '-')))
                {
                    stocks.Add(values[1].ToLower().Replace(' ', '-'), info);
                }
            }
        }
    }
    // Does the ticker thingy of the company that is named "name"
    public string getTicker(string name)
    {
        name = name.ToLower().Replace(" ", "-");
        foreach(KeyValuePair<string, string[]> entry in stocks)
        {
            if(entry.Key.Contains(name))
            {
                string ticker = entry.Value[0];
                ticker = ticker.Replace("\"", "");
                return ticker;
            }
        }
        return "";
    }
    // Returns the info of the company that is named "name" where infoPositions
    // contains the positions in the infoList of the company that is desired
    string[] getInfo(string name, int[] infoPositions)
    {
        name = name.ToLower().Replace(" ", "-");
        foreach(KeyValuePair<string, string[]> entry in stocks)
        {
            if(entry.Key.Contains(name))
            {
                string[] info = new string[infoPositions.Length];
                for(int j = 0; j < info.Length; j++)
                {
                    info[j] = entry.Value[infoPositions[j]];
                }
            }
        }
        return new string[] { "" };
    }
}