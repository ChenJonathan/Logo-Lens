using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GetTicker : MonoBehaviour
{
    private List<string> namesList = new List<string>();
    private List<string[]> infoList = new List<string[]>();

    void Start()
    {
        var reader = new StreamReader(File.OpenRead("Assets/companylist.csv"));
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(',');

            namesList.Add(values[1].ToLower().Replace(" ", "-"));
            string[] info = new string[values.Length - 2];
            info[0] = values[0];
            for (int i = 2; i < values.Length - 1; i++)
            {
                info[i - 1] = values[i];
            }
            infoList.Add(info);
        }
    }

    // Does the ticker thingy of the company that is named "name"
    public string getTicker(string name) {
        name = name.ToLower().Replace(" ", "-");
        for (int i = 0; i < namesList.Count; i++) {
            if (namesList[i].Contains(name)) {
                string ticker = infoList[i][0];
                ticker = ticker.Replace("\"", "");
                return ticker;
            }
        }

        return "";
    }

    // Returns the info of the company that is named "name" where infoPositions
    // contains the positions in the infoList of the company that is desired
    string[] getInfo(string name, int[] infoPositions) {
        name = name.ToLower().Replace(" ", "-");
        for (int i = 0; i < namesList.Count; i++) {
            if (namesList[i].Contains(name)) {
                string[] info = new string[infoPositions.Length];
                for (int j = 0; j < info.Length; j++) {
                    info[j] = infoList[i][infoPositions[j]];
                }
            }
        }
        return new string[] { "" };
    }
}