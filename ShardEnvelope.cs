using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace benchmark_akka_cluster
{

    public interface IShardEnvelope
    {
        string EntityId { get; }
        object ObjectPayload { get; }
    }

    public sealed class ShardEnvelope<T>: IShardEnvelope
    {
         

        public ShardEnvelope(string entityId, T payload)
        {
            EntityId = entityId;
            Payload = payload;
        }

        public string EntityId { get; }
        public T Payload { get; private set; }

        public object ObjectPayload => Payload;

        public override string ToString()
        {
            return EntityId;
        }        
    }
}
