using Neo.Consensus;
using Neo.IO;
using Neo.ConsoleService;
using Neo.Network.P2P;
using Neo.Network.P2P.Payloads;
using System;

namespace Neo.Plugins
{
    public class P2PLossyConsensusPlugin : Plugin
    {
        protected override void OnSystemLoaded(NeoSystem system)
        {

        }

        public bool Handler(NeoSystem system, Message message)
        {
            if (message.Command == MessageCommand.Extensible)
            {
                var extensible = message.Payload as ExtensiblePayload;
                if (extensible.Category == "dBFT")
                {
                    var consensus = ConsensusMessage.DeserializeFrom(extensible.Data);
                    // if (consensus.Type == ConsensusMessageType.PrepareRequest
                    //     || consensus.Type == ConsensusMessageType.ChangeView
                    //     || consensus.Type == ConsensusMessageType.PrepareResponse)
                    //     return false;
                    if (consensus.ValidatorIndex == 2)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        [ConsoleCommand("start loss", Category = "P2PLossyConsensus", Description = "Loss consensus message")]
        private void OnStartLoss()
        {
            RemoteNode.MessageReceived += Handler;
            Console.WriteLine("handler registered");
        }

        [ConsoleCommand("stop loss", Category = "P2PLossyConsensus", Description = "Loss consensus message")]
        private void OnStopLoss()
        {
            RemoteNode.MessageReceived -= Handler;
            Console.WriteLine("handler removed");
        }
    }
}
