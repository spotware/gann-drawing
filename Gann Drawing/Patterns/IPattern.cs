using System;

namespace cAlgo.Patterns
{
    public interface IPattern
    {
        event Action<IPattern> DrawingStarted;

        event Action<IPattern> DrawingStopped;

        void Initialize();

        void StartDrawing();

        void StopDrawing();

        string Name { get; }
    }
}