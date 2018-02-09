using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public static class LanguageManager
{
    private static string resourcesPath = "strings";
    private static Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
    public static List<string> loadedLanguages = new List<string>();
    public static bool ready { get; private set; }
    public static string currentLanguage { get; private set; }
    public static int currentLanguageIndex { get; private set; }

    private static UnityEvent onLanguageChange = new UnityEvent();

    public static void RegisterLangChangeEvent(UnityAction action)
    {
        onLanguageChange.AddListener(action);

        if (ready) action.Invoke();
    }

    public static void UnregisterLangChangeEvent(UnityAction action)
    {
        onLanguageChange.RemoveListener(action);
    }

    public static void SetLanguage(string languageCode)
    {
        Init();
        if (!loadedLanguages.Contains(languageCode))
        {
            Debug.LogWarning("LanguageManager: tried to access " + languageCode + " language but it is missing.");
            return;
        }

        currentLanguage = languageCode;
        currentLanguageIndex = loadedLanguages.IndexOf(languageCode);
        onLanguageChange.Invoke();
    }

    public static void SetLanguage(int languageIndex)
    {
        Init();
        if (languageIndex < 0 || languageIndex > loadedLanguages.Count - 1)
        {
            Debug.LogWarning("LanguageManager: tried to access index " + languageIndex.ToString("0") + " language but it is missing.");
            return;
        }

        currentLanguage = loadedLanguages[languageIndex];
        currentLanguageIndex = languageIndex;
        onLanguageChange.Invoke();
    }

    public static string GetWord(string key)
    {
        Init();
        return GetWord(currentLanguage, key);
    }
    public static string GetWord(string languageCode, string key)
    {
        Init();

        if (string.IsNullOrEmpty(key))
            return "";

        key = key.Trim();
        if (key.Contains("<&>"))
        {
            string[] split = key.Split(new string[1] { "<&>" }, System.StringSplitOptions.None);
            for (int x = 0; x < split.Length; x++)
            {
                split[x] = split[x].Trim();
                if (split[x].StartsWith("\"") && split[x].EndsWith("\""))
                {
                    split[x] = split[x].Substring(1, split[x].Length - 2);
                }
                else
                {
                    split[x] = GetWord(languageCode, split[x]);
                }
            }
            return string.Join("", split);
        }
        else
        {
            if (!loadedLanguages.Contains(languageCode))
            {
                Debug.LogWarning("LanguageManager: tried to access " + languageCode + " language but it is missing.");
                return "[" + languageCode.ToUpper() + " missing]";
            }
            if (!dictionary.ContainsKey(key))
            {
                return "[" + languageCode.ToUpper() + " key \"" + key + "\" missing]";
            }

            return GetWord(loadedLanguages.IndexOf(languageCode), key);
        }
    }
    public static string GetWord(int languageIndex, string key)
    {
        Init();
        if (languageIndex < 0 || loadedLanguages.Count <= languageIndex)
        {
            Debug.LogWarning("LanguageManager: tried to access index " + languageIndex.ToString("0") + " language but it is missing.");
            return "[lang. index " + languageIndex.ToString("0").ToUpper() + " missing]";
        }
        if (!dictionary.ContainsKey(key))
        {
            return "[lang. index " + languageIndex.ToString("0").ToUpper() + " key \"" + key + "\" missing]";
        }

        return dictionary[key][languageIndex];
    }

    private static void LoadDictionary()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(resourcesPath);
        if (textAsset != null)
        {
            List<List<string>> csv = SpreadsheetReader.Read(textAsset.text, ',', true);

            int colnCount = csv.Count;
            int lineCount = csv[0].Count;
            int langCount = colnCount - 1;
            int indexOfKeys = csv[0].IndexOf("keys");
            if (indexOfKeys < 0)
            {
                Debug.LogError("LanguageManager: invalid CSV format. Couldn't find \"keys\" column.");
                Debug.Break();
            }
            else
            {
                dictionary.Clear();

                for (int l = 1; l < colnCount; l++)
                {
                    string langName = csv[l][indexOfKeys];
                    loadedLanguages.Add(langName.Substring(0,2));
                }

                for (int x = 0; x < lineCount; x++)
                {
                    string key = csv[0][x];
                    if (!dictionary.ContainsKey(key))
                    {
                        if (!string.IsNullOrEmpty(key) && !key.StartsWith("//"))
                        {
                            string[] lnTranslation = new string[langCount];

                            for (int l = 0; l < langCount; l++)
                            {
                                lnTranslation[l] = csv[l + 1][x];
                            }

                            dictionary.Add(key, lnTranslation);
                        }
                    }
                }

                string currSysLang = "pt";
                
                if (!loadedLanguages.Contains(currSysLang.Substring(0, 2)))
                {
                    currSysLang = loadedLanguages[0];
                }

                currentLanguage = currSysLang;
                currentLanguageIndex = loadedLanguages.IndexOf(currentLanguage);

                ready = true;
                onLanguageChange.Invoke();

            }
        }
        else
        {
            Debug.LogError("LanguageManager: Couldn't find dictionary file at \"Resources/" + resourcesPath + "\".");
            Debug.Break();
        }
        Resources.UnloadAsset(textAsset);
    }

    public static void Init()
    {
        if (ready) return;
        LoadDictionary();
    }
}