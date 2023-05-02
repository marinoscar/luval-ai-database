using Luval.OpenAI;
using Luval.OpenAI.Completion;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Luval.AI.Database
{
    public class DataAnalyzer
    {
        public CompletionEndpoint Api { get; private set; }
        public SqlDatabase Database { get; private set; }
        public string DbSchema { get; private set; }

        public EventHandler<RequestEventArgs> StreamQueryResult;
        public EventHandler<RequestEventArgs> StreamChartResult;
        public EventHandler<RequestEventArgs> StreamResponseResult;
        public EventHandler<MessageEventArgs> StreamLogMessage;


        public DataAnalyzer(CompletionEndpoint api, string connectionString, string dataSchema)
        {
            Database = new SqlDatabase(() => new SqlConnection(connectionString));
            Api = api;
            DbSchema = dataSchema;
        }

        public DataAnalyzer(ApiAuthentication apiAuth, string connectionString, string dataSchema) : this(new CompletionEndpoint(apiAuth), connectionString, dataSchema)
        {
        }

        public DataAnalyzer(SecureString apikey, string connectionString, string dataSchema) : this(new ApiAuthentication(apikey), connectionString, dataSchema)
        {

        }

        public async Task<DataResponse> SendAsync(string prompt)
        {
            OnStreamLogMessage("Starting request");
            var result = new DataResponse();
            var dataPrompt = PrePrompt(prompt);
            var query = new StringBuilder();
            OnStreamLogMessage("Sending data questions");

            await foreach (var q in Api.StreamAsync(dataPrompt, temperature: 0))
            {
                OnStreamQueryResult(q);
                query.Append(q.ToString());
            }

            OnStreamLogMessage("Getting data");
            result.SqlQuery = query.ToString();
            result.Data = Database.ExecuteToDs(query.ToString());

            var responsePrompt = PostPrompt(prompt, query.ToString(), result.Data.Array);

            OnStreamLogMessage("Getting AI response");
            var response = new StringBuilder();
            await foreach (var q in Api.StreamAsync(responsePrompt, temperature: 0.7))
            {
                OnStreamResponseResult(q);
                response.Append(q.ToString());
            }
            result.Response = response.ToString();

            OnStreamLogMessage("Building chart");
            var chartPrompt = ChartPrompt(prompt, query.ToString(), result.Data.Array);
            var chart = new StringBuilder();
            await foreach (var q in Api.StreamAsync(chartPrompt, temperature: 0))
            {
                OnStreamChartResult(q);
                chart.Append(q.ToString());
            }

            result.HtmlChart = chart.ToString();
            return result;
        }

        private void OnStreamLogMessage(string logMessage)
        {
            StreamLogMessage?.Invoke(this, new MessageEventArgs(logMessage));
        }

        private void OnStreamQueryResult(CompletionResponse completionResponse)
        {
            StreamQueryResult?.Invoke(this, new RequestEventArgs(completionResponse));
        }

        private void OnStreamChartResult(CompletionResponse completionResponse)
        {
            StreamChartResult?.Invoke(this, new RequestEventArgs(completionResponse));
        }

        private void OnStreamResponseResult(CompletionResponse completionResponse)
        {
            StreamResponseResult?.Invoke(this, new RequestEventArgs(completionResponse));
        }

        private string PrePrompt(string prompt, int top = 100)
        {
            return Template.SqlPrefix
                .Replace("{top}", top.ToString())
                .Replace("{schema}", DbSchema)
                .Replace("{input}", prompt);
        }

        private string PostPrompt(string prompt, string sqlQuery, string sqlResult)
        {
            return Template.SqlSufix
                .Replace("{sql}", sqlQuery)
                .Replace("{schema}", DbSchema)
                .Replace("{result}", sqlResult)
                .Replace("{input}", prompt);
        }

        private string ChartPrompt(string prompt, string sqlQuery, string sqlResult)
        {
            return Template.SqlChart
                .Replace("{sql}", sqlQuery)
                .Replace("{schema}", DbSchema)
                .Replace("{result}", sqlResult)
                .Replace("{input}", prompt);
        }
    }
}
