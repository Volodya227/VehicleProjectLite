namespace Systems.Vehicle.Visual
{
    using ContainerData;
    using Data;
    public class VehicleVisualSystem
    {
        private readonly VehicleContainerData _vehicleContainerData;
        private readonly ModelItem _visualView;
        public ModelItem GetModelItem => _visualView;
        public VehicleVisualSystem(ModelItem visual, VehicleContainerData vehicleContainerData, VehicleData vehicleData) {
            _vehicleContainerData = vehicleContainerData;
            _visualView = visual;
            _visualView.SetGearBoxValue(vehicleData.GearBoxManualControlData.Prefab, vehicleData.GearBoxManualControlData.AngleX, vehicleData.GearBoxManualControlData.AngleZ);
            EnableEvent();
        }
        private void EnableEvent()
        {
            _vehicleContainerData.EventSteeringChange += SetSteering;
            _vehicleContainerData.EventPositionGear += ChangeGearBoxView;
        }
        private void SetSteering(float steer)
        {
            _visualView.SteerWheel(steer);
        }
        private void ChangeGearBoxView()
        {
            _visualView.UpdateGearBox(_vehicleContainerData.GearBoxVisualX, _vehicleContainerData.GearBoxVisualZ);
        }
    }
}