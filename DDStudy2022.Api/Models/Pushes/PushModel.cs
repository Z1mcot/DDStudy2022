using Newtonsoft.Json.Linq;

namespace DDStudy2022.Api.Models.Pushes
{
    public class PushModel
    {
        public class AlertModel
        {
            public string? Title { get; set; }
            public string? Subtitle { get; set; }
            public string? Body { get; set; }
        }
        /// <summary>
        /// Счётчик у иконки приложения
        /// </summary>
        public int? Badge { get; set; }

        public string? Sound { get; set; }

        public AlertModel Alert { get; set; } = null!;

        public Dictionary<string, object>? CustomData { get; set; }
    }
}
