﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.AI.Database
{
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(string msg)
        {
            LogMessage = msg;
        }
        public string LogMessage { get; set; } 
    }
}
