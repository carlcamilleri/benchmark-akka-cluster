using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace benchmark_akka_cluster
{
	public class PingPongActor : ReceiveActor
	{
        private string _hostName;
        private int _pid;
        private string _identifier;

        protected override void PreStart()
        {
            base.PreStart();
			_hostName = Dns.GetHostName();
			_pid = Process.GetCurrentProcess().Id;
			_identifier = Self.Path.ToString() + "(pid=" + _pid + ",hostname=" + _hostName + ")";

		}
        public PingPongActor()
		{
			ReceiveAny((msg) => Sender.Tell(new MsgPong(_identifier)));

			
		}


		 
	}
}
