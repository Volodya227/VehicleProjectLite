namespace Systems.Vehicle.Transmission.Model
{
    [System.Serializable]
    public class TransmissionModel
    {
        private readonly TorqueConverter.TorqueConverterBase _torqueConverter;
        [UnityEngine.SerializeField] private Engine _engine;
        private readonly GearBoxControl.GearBoxControlBase _gearBoxControlBase;
        private readonly GearBoxBase _gearBox;
        private readonly TCU.TransmissionControlUnitBase _transmissionControlUnitBase;
        public TransmissionModel(Data.VehicleData data, ContainerData.VehicleContainerData vehicleContainerData, FuelSystem.FuelSystem fuelSystem) {
            data.EngineData.SetSyncWindowRPM(data.SyncWindowRPM);
            TransmissionModelGeneratedState state;
            TransmissionModelContext context = new()
            {
                data = data,
                vehicleContainerData = vehicleContainerData,
                fuelSystem = fuelSystem
            };
            if (data.TypeControlTransmission == Data.TypeControlTransmission.Manual)
            {
                state = TransmissionModelFactory.CreateManual(context);
            }
            else if(data.TypeControlTransmission == Data.TypeControlTransmission.AutomatedManual)
            {
                state = TransmissionModelFactory.CreateAutomatedManual(context);
            }
            else if (data.TypeControlTransmission == Data.TypeControlTransmission.Planerary)
            {
                state = TransmissionModelFactory.CreatePlanetary(context);
            }
            else if (data.TypeControlTransmission == Data.TypeControlTransmission.Automatic)
            {
                state = TransmissionModelFactory.CreateAutomated(context);
            }
            else
            {
                state = TransmissionModelFactory.CreateDefault(context);
            }
            _engine = state.Engine;
            _torqueConverter = state.TorqueConverter;
            _gearBox = state.GearBox;
            _gearBoxControlBase = state.GearControl;
            _transmissionControlUnitBase = state.TCU;
        }
        public void Dispose()
        {
            _transmissionControlUnitBase.Dispose();
        }
        public void SetInput(Input.VehicleInput input)
        {
            _engine.SetInput(input);
            _gearBoxControlBase.SetInput(input);
            _gearBox.SetInput(input);//only low Gear
        }
        public float GetTorque(float transmissionRPM) {
            _transmissionControlUnitBase.Update();
            _gearBox.SetTransmissionRPM(transmissionRPM);
            _engine.Update();
            return _gearBox.GetTorque();
        }
    }
}