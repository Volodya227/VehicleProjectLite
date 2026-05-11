namespace Systems.Vehicle.Transmission.Model
{
    public class IdleSupport
    {
        //electronics
        private float _koefSupport = 1;
        private bool _useElectronics = true;
        public bool UseElectronics => _useElectronics;
        //public float idleRPM = 800f;
        public float maxGain = .7f;
        public float cutoffRPM = 1500f;
        //base
        private float _deltaRPM;
        private float _minRPM;
        private float _maxSupport;
        private float _idleSupport;
        private float _rangeRPM;
        public float RangeRPM => _rangeRPM;
        public IdleSupport(bool electronics, float deltaRPM, float minRPM, float maxSupport, float idleSupport)
        {
            _useElectronics = electronics;
            _deltaRPM = deltaRPM;
            _minRPM = minRPM;
            _maxSupport = maxSupport;
            _idleSupport = idleSupport;

            _rangeRPM = ComputeRangeRPM();
        }
        private float ComputeRangeRPM() {
            float deltaSupport = _maxSupport - _idleSupport;
            if (deltaSupport <= 0.02f)
            {
                _maxSupport += deltaSupport * 2;
                deltaSupport = _maxSupport - _idleSupport;
            }
            return _minRPM + (_maxSupport / deltaSupport) * _deltaRPM;
        }
        private float Support(float rpm)
        {
            if (rpm >= _rangeRPM)
                return 0f;

            float t = (rpm - _minRPM) / (_rangeRPM - _minRPM);
            return _maxSupport * (1 - t);
        }
        public float Evaluate(float rpm, float gasInput)
        {
            float support = Support(rpm) * _koefSupport;
            _koefSupport = 1;
            if (support > maxGain)
            {
                support = maxGain;
            }
            return support + (1 - support) * gasInput;
        }
        public void UpdateDynamicCoefficient(float engineTorque, float engineFriction, float clutchFriction)
        {
            float totalLosses = engineFriction + clutchFriction;
            _koefSupport = totalLosses / engineTorque;
            _koefSupport /= _idleSupport;
        }
    }
}