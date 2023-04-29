using System.Diagnostics;
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
        public event EventHandler RequestFailed;
        public event EventHandler RequestStarted;

        public string? Prompt { get; set; }
        public string? Response { get; set; }
        public string? Chart { get; set; }
        public string? SqlQuery { get; set; }
        public bool InProgress { get; set; }

        public IEnumerable<IDictionary<string, object>> Data { get; set; }


        public async void Run()
        {
            if (_dataPrompt == null) return;
            if (string.IsNullOrWhiteSpace(Prompt)) return;
            Clear();

            InProgress = true;
            RequestStarted?.Invoke(this, EventArgs.Empty);
            var result = default(DataResponse);
            try
            {
                result = await _dataPrompt.SendAsync(Prompt);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                InProgress = false;
                RequestFailed?.Invoke(this, EventArgs.Empty);
                return;
            }

            Response = result.Response;
            Chart = result.HtmlChart;
            Data = result.Data.Data;
            SqlQuery = result.SqlQuery;

            InProgress = false;
            RequestCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void Clear()
        {
            Response = null;
            Chart = null;
            Data = new List<Dictionary<string, object>>();
        }


    }
}