namespace Systems.Vehicle.Transmission.Model.GearBoxControl
{
    public class GearBoxPlanetaryControl : GearBoxManualControl
    {
        public GearBoxPlanetaryControl(Data.GearBoxManualControlData config, ContainerData.VehicleContainerData vehicleContainerData) : base(config, vehicleContainerData) {
            _isManual = false;
        }
    }
}