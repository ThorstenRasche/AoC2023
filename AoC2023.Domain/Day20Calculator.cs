using System.Linq;

namespace AoC23.Domain;

[Day(20)]
public class Day20Calculator : IDayCalculator
{
    static Dictionary<string, PulseModule> modules = new();
    static Queue<string> processOrder = new();
    static Dictionary<string, long> rxConjunctions = new();
    static string rxFeeder;
    public long CalculatePart1(string filePath)
    {
        ReadFile(filePath);

        long lowPulses = 0;
        long highPulses = 0;

        for (int i = 0; i < 1000; i++)
        {
            modules["button"].incomingPulses.Enqueue(("finger", false));
            processOrder.Enqueue("button");

            while (processOrder.TryDequeue(out var nextPulseTarget))
            {
                (long pulsesSent, bool pulseVal) = modules[nextPulseTarget].ProcessPulse();

                if (pulseVal) highPulses += pulsesSent;
                else lowPulses += pulsesSent;
            }
        }

        return (highPulses * lowPulses);
    }    

    public long CalculatePart2(string filePath)
    {
        ReadFile(filePath);
        for (int i = 1; ; i++)
        {
            modules["button"].incomingPulses.Enqueue(("finger", false));
            processOrder.Enqueue("button");

            while (processOrder.TryDequeue(out var name))
            {
                (_, bool pulseVal) = modules[name].ProcessPulse();
                if (rxConjunctions.ContainsKey(name) && rxConjunctions[name] == 0 && pulseVal) rxConjunctions[name] = i;
            }

            if (rxConjunctions.Values.All(a => a != 0)) break;
        }

        return rxConjunctions.Values.Aggregate(1L, (a, b) => a = Extensions.FindLCM(a, b));
    }
   
    internal class PulseModule
    {
        public string name;
        public PulseModuleType Type = PulseModuleType.Unknown;
        public bool flipFlopState = false;
        public Dictionary<string, bool> lastReceivedPulses = new();
        public Queue<(string source, bool pulse)> incomingPulses = new();
        public List<string> outputs = new();

        public void Reset()
        {
            flipFlopState = false;
            foreach (var k in lastReceivedPulses.Keys)
            {
                lastReceivedPulses[k] = false;
            }
        }

        public (long pulsesSent, bool pulseVal) ProcessPulse()
        {
            long pulsesSent = 0;
            bool pulseVal = false;

            if (incomingPulses.TryDequeue(out var p))
            {
                (string src, var pulse) = p;
                if (Type == PulseModuleType.FlipFlop)
                {
                    if (!pulse)
                    { 

                        flipFlopState = !flipFlopState;
                        foreach (string n in outputs)
                        {
                            modules[n].incomingPulses.Enqueue((name, flipFlopState));
                            processOrder.Enqueue(n);
                            pulsesSent++;
                        }

                        pulseVal = flipFlopState;
                    }
                }
                else if (Type == PulseModuleType.Conjunction)
                {
                    lastReceivedPulses[src] = pulse;
                    pulseVal = lastReceivedPulses.Values.Any(a => !a);

                    foreach (string n in outputs)
                    {
                        modules[n].incomingPulses.Enqueue((name, pulseVal));
                        processOrder.Enqueue(n);
                        pulsesSent++;
                    }
                }
                else if (Type == PulseModuleType.Broadcast)
                {
                    pulseVal = pulse;
                    foreach (string n in outputs)
                    {
                        modules[n].incomingPulses.Enqueue((name, pulseVal));
                        processOrder.Enqueue(n);
                        pulsesSent++;
                    }
                }
            }
            return (pulsesSent, pulseVal);
        }
    }

    internal enum PulseModuleType
    {
        FlipFlop,
        Conjunction,
        Broadcast,
        Unknown
    }

    internal static void ReadFile(string filePath)
    {
        string Input = File.ReadAllText(filePath);
        foreach ((string n, PulseModule tmp) in Input.ExtractWords()
                     .Distinct()
                     .Select(n => new { n, tmp = new PulseModule() { name = n } })
                     .Select(@t => (@t.n, @t.tmp)))
        {
            modules[n] = tmp;
        }

        foreach ((string l, List<string> relModules, string curMod) in Input.SplitByNewline()
                     .Select(l => new { l, relModules = l.ExtractWords().ToList() })
                     .Select(@t => new { @t, curMod = @t.relModules[0] })
                     .Select(@t => (@t.@t.l, @t.@t.relModules, @t.curMod)))
        {
            modules[curMod].Type = (l[0]) switch
            {
                '&' => PulseModuleType.Conjunction,
                '%' => PulseModuleType.FlipFlop,
                _ => PulseModuleType.Broadcast
            };
            List<string> list = relModules[1..];
            foreach (string m in list)
            {
                modules[curMod].outputs.Add(m);
                modules[m].lastReceivedPulses[curMod] = false;
                if (m == "rx") rxFeeder = curMod;
            }
        }

        PulseModule button = new()
        {
            name = "button",
            Type = PulseModuleType.Broadcast,
            outputs = new() { "broadcaster" }
        };

        modules["button"] = button;

        foreach (PulseModule? m in modules.Values.Where(a => a.outputs.Contains(rxFeeder)))
        {
            rxConjunctions[m.name] = 0;
        }
    }
}
