

namespace Decoder.Logic
{
    public class Satellite
    {
        public string Name { get; set; }
        public (double x, double y) Possition { get; set; }
    }

    public enum Possition
    {
        START = 10,
        MIDDLE = 20,
        END = 30
    }
}
