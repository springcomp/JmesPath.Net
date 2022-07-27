using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Interop
{
    public interface IReduceAccumulator {

        void PushSeed(JToken seed);
        void PopSeed();

        JToken Accumulator { get; set; }
    }
}