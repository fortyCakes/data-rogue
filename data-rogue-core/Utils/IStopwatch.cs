using System.Diagnostics;

namespace data_rogue_core.Systems
{
    public interface IStopwatch
    {
        void Start();
        void Stop();
        void Restart();
        long ElapsedMilliseconds { get; }
    }

    public class EncapsulatedStopwatch : IStopwatch
    {
        private Stopwatch _stopwatch;

        public EncapsulatedStopwatch()
        {
            _stopwatch = new Stopwatch();
        }

        public void Start()
        {
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public void Restart()
        {
            _stopwatch.Restart();
        }

        public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;
    }
}