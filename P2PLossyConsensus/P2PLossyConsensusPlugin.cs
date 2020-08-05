using System;
using Neo.Network.P2P;
using Neo.Network.P2P.Payloads;
using Neo.Consensus;

namespace Neo.Plugins
{
    public class P2PLossyConsensusPlugin : Plugin, IP2PPlugin
    {
        private static Random random = new Random();
        public override void Configure()
        {
            Settings.Load(GetConfiguration());
        }

        public bool OnP2PMessage(Message message)
        {
            return true;
        }

        public bool OnConsensusMessage(ConsensusPayload payload)
        {
            if (payload.ConsensusMessage.Type == ConsensusMessageType.PrepareResponse)
            {
                return random.NextDouble() > Settings.Default.LossRate;
            }
            return true;
        }
    }
}
