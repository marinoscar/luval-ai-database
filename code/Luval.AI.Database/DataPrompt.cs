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
    public class DataPrompt
    {
        public CompletionEndpoint Api { get; private set; }
        public SqlDatabase Database { get; private set; }
        public string DbSchema { get; private set; }

        public DataPrompt(SecureString apikey, string connectionString, string dataSchema)
        {
            Database = new SqlDatabase(() => new SqlConnection(connectionString));
            Api = new CompletionEndpoint(new ApiAuthentication(apikey));
            DbSchema = dataSchema;
        }

        public async Task<DataResponse> SendAsync(string prompt)
        {
            var response = new DataResponse();
            var dataPrompt = PrePrompt(prompt);
            var queryResponse = await Api.SendAsync(dataPrompt, temperature: 0);
            response.SqlQuery = queryResponse.Choice.Text;
            response.Data = Database.ExecuteToDs(queryResponse.Choice.Text);
            var responsePrompt = PostPrompt(prompt, queryResponse.Choice.Text, response.Data.Array);
            var aiResponse = await Api.SendAsync(responsePrompt, temperature: 0.7);
            response.Response = aiResponse.Choice.Text;
            var chartPrompt = ChartPrompt(prompt, queryResponse.Choice.Text, response.Data.Array);
            var chartResponse = await Api.SendAsync(chartPrompt);
            response.HtmlChart = chartResponse.Choice.Text;
            return response;

        }

        private string PrePrompt(string prompt, int top = 1000)
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
