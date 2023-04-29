using Luval.OpenAI.Completion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.AI.Database
{
    public class RequestEventArgs : EventArgs
    {
        public RequestEventArgs(CompletionResponse response)
        {
            Response = response;
        }

        public CompletionResponse Response { get; private set; }
    }
}
