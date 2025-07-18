using System.Text.Json.Serialization;

namespace mdcheckerwpf.MVVM
{
    public class Settings
    {
        [JsonPropertyName("checkMainParts")]
        public bool CheckMainParts { get; set; }

        [JsonPropertyName("startPage")]
        public string StartPage { get; set; } = "model";

        [JsonPropertyName("checkLength")]
        public bool CheckLength { get; set; }

        [JsonPropertyName("checkMaterial")]
        public bool CheckMaterial { get; set; }

        [JsonPropertyName("checkDetailDrawings")]
        public bool CheckDetailDrawings { get; set; }

        [JsonPropertyName("checkBoltLength")]
        public bool CheckBoltLength { get; set; }

        [JsonPropertyName("checkScrewAssembly")]
        public bool CheckScrewAssembly { get; set; }

        [JsonPropertyName("checkRounding")]
        public bool CheckRounding { get; set; }

        [JsonPropertyName("checkReflectedView")]
        public bool CheckReflectedView { get; set; }

        [JsonPropertyName("checkDrawnCheckedBy")]
        public bool CheckDrawnCheckedBy { get; set; }

        [JsonPropertyName("checkPartMarkMissing")]
        public bool CheckPartMarkMissing { get; set; }
    }
}
