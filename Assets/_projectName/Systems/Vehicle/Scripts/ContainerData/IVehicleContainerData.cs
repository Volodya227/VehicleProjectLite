namespace Systems.Vehicle.ContainerData
{
    public class AxleDataVehicle
    {
        public float TorqueLeft;
        public float TorqueRight;
        public float WheelGripLeft;
        public float WheelGripRight;
    }
    public interface IVehicleContainerData
    {
        public event System.Action<float> EventSteeringChange;
        public event System.Action EventPositionGear;
        public event System.Action EventChangeLowGear;
        public event System.Action EventChangeFuelUseable;
        public event System.Action EventChangeHandBrake;
        public float Speed { get; }
        public int RPM { get; }
        public int MaxRPM { get; }
        public string Gear { get; }
        public int Distance { get; }
        public float Clutch { get; }
        public float Brake { get; }
        public float Accelerator { get; }
        public float MaxFuel { get; }
        public float Fuel { get; }
        public bool LowGear { get; }
        public int FuelInt();
        public float FuelFill { get; }
        public float FuelUseable { get; }
        public AxleDataVehicle GetAxleData(int i);
        public int AxleDataCount {  get; }
        public int GearBoxVisualX { get; }
        public int GearBoxVisualZ { get; }
        public bool HandBrake { get; }
    }
}