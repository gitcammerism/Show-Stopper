using UnityEngine;
using System;

public static class HintSettings
{
    private const string HintsEnabledKey = "HintsEnabled";

    public static event Action<bool> OnHintsChanged;

    // Enable hints by default
    public static bool IsEnabled
    {
        get
        {
            return PlayerPrefs.GetInt(HintsEnabledKey, 1) == 1;
        }
    }

    // Change bool to enable or disabled
    public static void SetEnabled(bool enabled)
    {
        int settingValue = enabled ? 1 : 0;

        PlayerPrefs.SetInt(HintsEnabledKey, settingValue);
        PlayerPrefs.Save();

        OnHintsChanged?.Invoke(enabled);
    }
}