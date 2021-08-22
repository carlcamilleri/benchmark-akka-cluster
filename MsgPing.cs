using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace benchmark_akka_cluster
{
    public class MsgPing
    {
    }

    public record MsgSendPing(string entityId, MsgPing msg);
}
