using System;
using System.Collections.Generic;
using Neo.Consensus;
using Neo.ConsoleService;
using Neo.Network.P2P;
using Neo.Network.P2P.Payloads;

namespace Neo.Plugins
{
    public class P2PLossyConsensusPlugin : Plugin
    {
        private double lossRate = 0;
        private readonly Random rand = new Random();
        private readonly HashSet<ConsensusMessageType> excludes = new();

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
                    var consensus = ConsensusMessage.DeserializeFrom(extensible.Data);
                    if (excludes.Contains(consensus.Type)) return false;
                    double sample = rand.Next(100);
                    if (sample * 1.0 / 100 < lossRate)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        [ConsoleCommand("lossm", Category = "P2PLossyConsensus", Description = "Loss consensus message")]
        private void OnLossm(string msgs)
        {
            excludes.Clear();
            Console.WriteLine("excludes clear!");
            if (msgs == "") return;
            var ms = msgs.Split(' ');
            foreach (var msg in ms)
            {
                if (Enum.TryParse<ConsensusMessageType>(msg, true, out var toExclude))
                {
                    if (!excludes.Add(toExclude))
                        Console.WriteLine($"{toExclude} already blocked!");
                    else
                        Console.WriteLine($"{toExclude} excluded");
                }
                else
                {
                    Console.WriteLine($"{toExclude} invalid message type!");
                }
            }
        }

        [ConsoleCommand("lossmsg", Category = "P2PLossyConsensus", Description = "Loss consensus message")]
        private void OnLossmsg()
        {
            Console.WriteLine(string.Join(", ", excludes));
        }

        [ConsoleCommand("lossr", Category = "P2PLossyConsensus", Description = "Loss consensus message")]
        private void OnLossr(string rate)
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
