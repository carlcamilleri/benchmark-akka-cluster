using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace benchmark_akka_cluster
{
	public class PingPongHandlerActor : ReceiveActor,IWithUnboundedStash
	{
  

		private IActorRef _sender;

        public IStash Stash { get; set; }

        protected override void PreStart()
        {
            base.PreStart();
		 

		}
        public PingPongHandlerActor()
		{
			WaitRequest();


		}

		public void WaitRequest()
        {
			Receive<MsgSendPing>((msg) => {

				_sender = Sender;
				Startup._shardRegion.Tell(new ShardEnvelope<MsgPing>(msg.entityId, msg.msg));
				Become(WaitResult);



			});

 
		}

		public void WaitResult()
		{
			Receive<MsgPong>((msg) => {
				_sender.Tell(msg);
				Stash.UnstashAll();
				Become(WaitRequest);
			});

			ReceiveAny(msg =>
				Stash.Stash()
			);
		

		}






	}
}
