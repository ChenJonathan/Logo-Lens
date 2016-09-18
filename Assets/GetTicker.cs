using System.Collections.Generic;
using System.IO;

public class Program {
    private static List<string> namesList = new List<string>();
    private static List<string[]> infoList = new List<string[]>();

    static void Main(string[] args) {
        var reader = new StreamReader(File.OpenRead("companylist.csv"));
        while (!reader.EndOfStream) {
            var line = reader.ReadLine();
            var values = line.Split(',');

            namesList.Add(values[1].ToLower().Replace(" ", "-"));
            string[] info = new string[values.Length - 2];
            info[0] = values[0];
            for (int i = 2; i < values.Length - 1; i++) {
                info[i - 1] = values[i];
            }
            infoList.Add(info);
        }
    }

    // Returns the ticker of the company that is named "name"
    static string getTicker(string name) {
        name = name.ToLower().Replace(" ", "-");
        for (int i = 0; i < namesList.Count; i++) {
            if (namesList[i].Contains(name)) {
                return infoList[i][0];
            }
        }
        return "";
    }

    // Returns the info of the company that is named "name" where infoPositions
    // contains the positions in the infoList of the company that is desired
    static string[] getInfo(string name, int[] infoPositions) {
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