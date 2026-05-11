namespace Systems.Vehicle.Transmission.Model
{
    public class GearBoxManual : GearBoxBase
    {
        private int _gear;
        public override int GetGear => _gear;
        public override int GetMaxGear => _gearKoef.Length;
        private readonly float[] _gearKoef;
        private readonly float _gearKoefReverse;
        private readonly int _minIndex;
        private readonly int _maxIndex;
        public GearBoxManual(Data.VehicleData vehicleData, ContainerData.VehicleContainerData vehicleContainerData, TorqueConverter.TorqueConverterBase torqueConverter) : base(vehicleData, vehicleContainerData, torqueConverter)
        {
            _gearKoef = new float[vehicleData.gearKoef.Count];
            _gear = 0;
            for(int i = 0; i< _gearKoef.Length;i++)
            {
                _gearKoef[i] = vehicleData.gearKoef[i];
            }// TODO reverse koef from config
            _gearKoefReverse = vehicleData.GetGearKoefReverse;
            _minIndex = -1;
            _maxIndex = _gearKoef.Length;
            GetGearContainerData();

        }
        public override bool TrySetGear(int newGear) {
            if (!_torqueConverter.CanChangeGear) return false;
            SetGear(newGear);
            return true;
        }
        protected override void SetGear(int gear)
        {
            if (_torqueConverter.CanChangeGear)
            {
                _gear = System.Math.Clamp(gear, _minIndex, _maxIndex);
                GetGearContainerData();
                UpdateGearBox();
            }
        }
        protected override float GetKoef()
        {
            if (_gear == 0) return 0;
            if (_gear < 0)
                return _gearKoefReverse;
            return _gearKoef[_gear - 1];
        }
        private void GetGearContainerData()
        {
            if (_gear == 0) _vehicleContainerData.SetGear("N");
            else if (_gear < 0) _vehicleContainerData.SetGear("R" + (-_gear).ToString());
            else _vehicleContainerData.SetGear(_gear.ToString());
        }
    }
    public class GearBoxAuto : GearBoxBase
    {
        private int _gear;
        public override int GetGear => _gear;
        public override int GetMaxGear => _gearKoef.Length;
        private readonly float[] _gearKoef;
        private readonly float _gearKoefReverse;
        private readonly int _minIndex;
        private readonly int _maxIndex;
        public GearBoxAuto(Data.VehicleData vehicleData, ContainerData.VehicleContainerData vehicleContainerData, TorqueConverter.TorqueConverterBase torqueConverter) : base(vehicleData, vehicleContainerData, torqueConverter)
        {
            _gearKoef = new float[vehicleData.gearKoef.Count];
            _gear = 0;
            for (int i = 0; i < _gearKoef.Length; i++)
            {
                _gearKoef[i] = vehicleData.gearKoef[i];
            }
            _gearKoefReverse = vehicleData.GetGearKoefReverse;
            _minIndex = -1;
            _maxIndex = _gearKoef.Length;
            GetGearContainerData();

        }
        public override bool TrySetGear(int newGear)
        {
            SetGear(newGear);
            return true;
        }
        protected override void SetGear(int gear)
        {
            if (_torqueConverter.CanChangeGear)
            {
                _gear = System.Math.Clamp(gear, _minIndex, _maxIndex);
                UpdateGearBox();
                GetGearContainerData();
            }
        }
        protected override float GetKoef()
        {
            if (_gear == 0) return 0;
            if (_gear < 0)
                return _gearKoefReverse;
            return _gearKoef[_gear - 1];

        }
        private void GetGearContainerData()
        {
            if (_gear == 0) _vehicleContainerData.SetGear("N");
            else if (_gear < 0) _vehicleContainerData.SetGear("R" + (-_gear).ToString());
            else _vehicleContainerData.SetGear("D" + _gear.ToString());
        }
    }
}