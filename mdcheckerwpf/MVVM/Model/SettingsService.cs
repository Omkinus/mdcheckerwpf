using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using mdcheckerwpf.MVVM.Model;

public class SettingsService
{
    private const string SettingsFilePath = "settings.json";

    public async Task<SettingsModel> LoadSettingsAsync()
    {
        if (!File.Exists(SettingsFilePath))
        {
            return new SettingsModel();
        }

        var json = await Task.Run(() => File.ReadAllText(SettingsFilePath));
        return JsonSerializer.Deserialize<SettingsModel>(json) ?? new SettingsModel();
    }


    public async Task SaveSettingsAsync(SettingsModel settings)
    {
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        await Task.Run(() => File.WriteAllText(SettingsFilePath, json));
    }

}
