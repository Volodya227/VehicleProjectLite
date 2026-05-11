namespace Systems.Core.FuelSystem
{
    public abstract class FuelSystemBase
    {
        protected readonly int _maxFuel;
        protected float _fuel;
        public float DeltaFuel => _maxFuel - _fuel;
        public float Fuel => _fuel;
        public int MaxFuel => _maxFuel;
        public FuelSystemBase(int maxFuel, float fuel)
        {
            _maxFuel = maxFuel;
            _fuel = fuel;
        }
        public bool AddFuel(float value)
        {
            if (_maxFuel - _fuel < value) return false;
            _fuel += value;
            ChangeFuel();
            return true;
        }
        public bool LoseFuel(float value)
        {
            if (value > _fuel) return false;
            _fuel -= value;
            ChangeFuel();
            return true;
        }
        protected virtual void ChangeFuel() { }
    }
}