using System;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using pmclient.Settings;

namespace pmclient.Services;

public class SettingsService
{
    private readonly string _settingsPath;

    public AppSettings CurrentSettings { get; set; }

    public SettingsService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _settingsPath = Path.Combine(appData, "PassManager", "settings.json");
        Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath)!);

        Load();
    }

    public void SetLanguage(string languageCode)
    {
        var oldLanguageCode = languageCode != "En" ? "En" : "Ru";

        if (Application.Current != null)
            Application.Current.Resources.MergedDictionaries.Remove(
                (ResourceDictionary)Application.Current.Resources[$"{oldLanguageCode}"]!);

        Application.Current!.Resources.MergedDictionaries.Add(
            (ResourceDictionary)Application.Current.Resources[$"{languageCode}"]!);

        CurrentSettings.Language = languageCode;

        Save();
    }

    public void SetTheme(string themeName)
    {
        var oldTheme = themeName != "Light" ? "Light" : "Dark";

        if (Application.Current != null)
            Application.Current.Resources.MergedDictionaries.Remove(
                (ResourceDictionary)Application.Current.Resources[$"{oldTheme}"]!);

        Application.Current!.Resources.MergedDictionaries.Add(
            (ResourceDictionary)Application.Current.Resources[$"{themeName}"]!);

        CurrentSettings.Theme = themeName;

        Save();
    }

    public void Load()
    {
        if (!File.Exists(_settingsPath))
        {
            CurrentSettings = new AppSettings();
            return;
        }

        var json = File.ReadAllText(_settingsPath);
        CurrentSettings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(CurrentSettings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_settingsPath, json);
    }
}