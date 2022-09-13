using System;
using Neo.Consensus;
using Neo.ConsoleService;
using Neo.Network.P2P;
using Neo.Network.P2P.Payloads;

namespace Neo.Plugins
{
    public class P2PLossyConsensusPlugin : Plugin
    {
        private double lossRate = 0;
        private Random rand = new Random();

        protected override void OnSystemLoaded(NeoSystem system)
        {
            RemoteNode.MessageReceived += Handler;
            Console.WriteLine($"handler registered, lossRate={lossRate}");
        }

        public bool Handler(NeoSystem system, Message message)
        {
            if (message.Command == MessageCommand.Extensible)
            {
                var extensible = message.Payload as ExtensiblePayload;
                if (extensible.Category == "dBFT")
                {
                    // var consensus = ConsensusMessage.DeserializeFrom(extensible.Data);
                    double sample = rand.Next(100);
                    if (sample * 1.0 / 100 < lossRate)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        [ConsoleCommand("loss", Category = "P2PLossyConsensus", Description = "Loss consensus message")]
        private void OnLoss(string rate)
        {
            if (rate != "")
            {
                lossRate = double.Parse(rate);
                if (lossRate < 0 || lossRate > 1)
                {
                    Console.WriteLine("invalid rate, must between 0 and 1");
                    return;
                }
            }
            Console.WriteLine($"lossRate update to {lossRate}");
        }

        [ConsoleCommand("lossrate", Category = "P2PLossyConsensus", Description = "Loss consensus message")]
        private void OnLossRate()
        {
            Console.WriteLine($"lossRate={lossRate}");
        }
    }
}
