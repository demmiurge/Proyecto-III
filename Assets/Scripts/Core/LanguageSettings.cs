using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class LanguageSettings : MonoBehaviour
{
    private List<Locale> availableLanguages;
    private int currentLanguagesIndex;

    void Start()
    {
        availableLanguages = new List<Locale>();
        currentLanguagesIndex = 1;

        var availableLocales = LocalizationSettings.AvailableLocales.Locales;
       
        foreach (Locale local in availableLocales)
            availableLanguages.Add(local);

        Locale fixedLocate = GameManager.instance.GetLocale();

        if (fixedLocate != null) { 
            for (int i = 0; i < availableLanguages.Count; i++) 
            { 
                if (availableLanguages[i] == fixedLocate)
                {
                    currentLanguagesIndex = i;
                }
            }

            Debug.Log("LOAD: fixedLocate " + fixedLocate.name);
        }

        LocalizationSettings.SelectedLocale = availableLanguages[currentLanguagesIndex];
    }

    public void ChangeLanguageByName(string languageName)
    {
        var availableLocales = LocalizationSettings.AvailableLocales.Locales;

        foreach (var locale in availableLocales)
        {
            if (locale.LocaleName == languageName)
            {
                LocalizationSettings.SelectedLocale = locale;
                break;
            }
        }
    }

    public void ApplyLanguage(bool next)
    {
        if (next)
            currentLanguagesIndex++;
        else
            currentLanguagesIndex--;

        if (currentLanguagesIndex < 0)
            currentLanguagesIndex = availableLanguages.Count - 1;
        else if (currentLanguagesIndex >= availableLanguages.Count)
            currentLanguagesIndex = 0;

        LocalizationSettings.SelectedLocale = availableLanguages[currentLanguagesIndex];
        GameManager.instance.SetLocale(availableLanguages[currentLanguagesIndex]);
        Debug.Log("SAVE: currentLanguagesIndex " + currentLanguagesIndex);
    }
}
