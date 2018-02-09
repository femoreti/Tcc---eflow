using System.Collections.Generic;

public class SpreadsheetReader
{

    public static List<List<string>> Read(string raw, char separator, bool invertLineColn = false)
    {
        List<List<string>> grid = new List<List<string>>();

        raw = raw.Replace("\r", "");

        List<string> lines = BreakString(raw, '\n');

        int lineCount = lines.Count;
        for (int x = 0; x < lineCount; x++)
        {
            List<string> brLine = BreakString(lines[x], separator);
            if (!invertLineColn)
            {
                grid.Add(brLine);
            }
            else
            {
                int brCount = brLine.Count;
                while (grid.Count < brCount)
                {
                    grid.Add(new List<string>());
                    if (grid.Count > 1)
                    {
                        while (grid[grid.Count - 1].Count < grid[grid.Count - 2].Count)
                        {
                            grid[grid.Count - 1].Add("");
                        }
                    }
                }

                for (int n = 0; n < brCount; n++)
                {
                    grid[n].Add(brLine[n]);
                }
            }
        }
        return grid;
    }

    private static List<string> BreakString(string raw, char separator)
    {
        List<string> result = new List<string>();

        if (string.IsNullOrEmpty(raw))
            raw = "";

        int index = 0;
        int count = raw.Length;
        while (index < count)
        {
            int start = index;
            int length = 0;
            bool stop = false;
            bool quotesOpen = false;
            while (!stop)
            {
                if (!quotesOpen)
                {
                    if (raw[index] == separator)
                    {
                        stop = true;
                    }
                    else
                    {
                        length++;
                        if (raw[index] == '"')
                        {
                            quotesOpen = true;
                        }
                    }
                }
                else
                {
                    if (raw[index] == '"')
                    {
                        if (index + 1 < count)
                        {
                            if (raw[index + 1] == '"')
                            {
                                index++;
                                length++;
                            }
                            else
                            {
                                quotesOpen = false;
                            }
                        }
                        else
                        {
                            quotesOpen = false;
                        }
                        length++;
                    }
                    else
                    {
                        length++;
                    }
                }

                index++;

                if (index >= count)
                    stop = true;
            }

            string endString = raw.Substring(start, length);

            if (endString.StartsWith("\"") && endString.EndsWith("\"" + separator))
                endString = endString.Substring(1, endString.Length - 3);
            else if (endString.StartsWith("\"") && endString.EndsWith("\""))
                endString = endString.Substring(1, endString.Length - 2);

            result.Add(endString);
        }

        return result;
    }
}
