using System.Collections.Generic;
using System.Linq;

namespace JeuDeLaVie
{
    public class FPSCounter
    {
        private float nextFrameTime = 0, frameTime;
        public bool availableFrameT { get; private set; } = true;
        public FPSCounter(int physicalMaxFPS = 60)
        {
            frameTime = 1 / (float)physicalMaxFPS;
        }

        public long FPSTotal { get; private set; }
        public float TotalS { get; private set; }
        public float AvgFPS { get; private set; } = 0;

        public float CurrentFPS
        {
            get;
            private set;
        } = 0;

        public int NbFrameCount { get; } = 100;
        private Queue<float> _sampleBuffer = new Queue<float>();
        public void Add(float deltaTime)
        {
            if(deltaTime!=0)
                CurrentFPS = 1.0f / deltaTime;

            _sampleBuffer.Enqueue(CurrentFPS);

            if (_sampleBuffer.Count > NbFrameCount)
            {
                _sampleBuffer.Dequeue();
                AvgFPS = _sampleBuffer.Average(i => i);
            }

            FPSTotal++;
            TotalS += deltaTime;
            if(TotalS >= nextFrameTime)
            {
                availableFrameT = true;
                nextFrameTime += frameTime;
            }
            else
                availableFrameT = false;
        }
    }
}
