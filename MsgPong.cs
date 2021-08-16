using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace benchmark_akka_cluster
{
    public class MsgPong
    {
        public MsgPong(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}
