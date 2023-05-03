using System.Diagnostics;
using System.Security;

namespace Luval.AI.Database.MVM
{
    public class ChatMVM
    {
        private DataAnalyzer _analyzer;

        public ChatMVM(DataAnalyzer analyzer)
        {
            _analyzer = analyzer;
            _analyzer.StreamLogMessage += DoLogMessage;
        }

        public event EventHandler RequestCompleted;
        public event EventHandler RequestFailed;
        public event EventHandler RequestStarted;
        public event EventHandler RequestMessage;

        public string? Prompt { get; set; }
        public string? Response { get; set; }
        public string? Chart { get; set; }
        public string? SqlQuery { get; set; }
        public string? LogMessage { get; set; }
        public string? ErrorMessage { get; set; }
        public bool InProgress { get; set; }

        public IEnumerable<IDictionary<string, object>> Data { get; set; }


        public async void Run()
        {
            if (_analyzer == null) return;
            if (string.IsNullOrWhiteSpace(Prompt)) return;
            Clear();

            InProgress = true;
            RequestStarted?.Invoke(this, EventArgs.Empty);
            var result = default(DataResponse);
            try
            {
                result = await _analyzer.SendAsync(Prompt);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                InProgress = false;
                ErrorMessage = ex.ToString();
                RequestFailed?.Invoke(this, EventArgs.Empty);
                return;
            }

            Response = result.Response;
            Chart = result.HtmlChart.Replace("</code>", "").Replace("<code>", "");
            Data = result.Data.Data;
            SqlQuery = result.SqlQuery;

            InProgress = false;
            RequestCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void Clear()
        {
            ErrorMessage = null;
            LogMessage = null;
            Response = null;
            SqlQuery = null;
            Chart = null;
            Data = new List<Dictionary<string, object>>();
        }

        private void DoLogMessage(object? sender, MessageEventArgs e)
        {
            LogMessage = e.LogMessage;
            RequestMessage?.Invoke(this, EventArgs.Empty);
        }


    }
}