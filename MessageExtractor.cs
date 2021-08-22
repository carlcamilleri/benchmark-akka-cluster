using Akka.Cluster.Sharding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace benchmark_akka_cluster
{
    public sealed class MessageExtractor : HashCodeMessageExtractor
    {
        public MessageExtractor(int maxNumberOfShards) : base(maxNumberOfShards) { }
        public override string EntityId(object message) => message.ToString();

        public override object EntityMessage(object message) =>
            message;
    }
}
