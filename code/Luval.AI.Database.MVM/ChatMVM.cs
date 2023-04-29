using System.Security;

namespace Luval.AI.Database.MVM
{
    public class ChatMVM
    {
        private DataAnalyzer _dataPrompt;

        public ChatMVM(DataAnalyzer dataPrompt)
        {
            _dataPrompt = dataPrompt;
        }


        public event EventHandler RequestCompleted;

        public string? Prompt { get; set; }
        public string? Response { get; set; }
        public string? Chart { get; set; }
        public string? SqlQuery { get; set; }

        public IEnumerable<IDictionary<string, object>> Data { get; set; }


        public async void Run()
        {
            if (_dataPrompt == null) return;
            if (string.IsNullOrWhiteSpace(Prompt)) return;
            Clear();
            var result = await _dataPrompt.SendAsync(Prompt);

            Response = result.Response;
            Chart = result.HtmlChart;
            Data = result.Data.Data;
            SqlQuery = result.SqlQuery;

            RequestCompleted?.Invoke(this, new EventArgs());
        }

        private void Clear()
        {
            Response = null;
            Chart = null;
            Data = new List<Dictionary<string, object>>();
        }


    }
}