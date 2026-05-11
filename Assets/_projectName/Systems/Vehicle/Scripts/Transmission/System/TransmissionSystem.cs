using UnityEngine;
namespace Systems.Vehicle.Transmission.Core
{
    [System.Serializable]
    public class TransmissionSystem
    {
        [SerializeField] private View.TransmissionView _view;
        [SerializeField] private Model.TransmissionModel _model;
        public TransmissionSystem(Rigidbody body, Data.VehicleData data, GameObject prefabAxleCollider, GameObject prefabAxleController, ContainerData.VehicleContainerData vehicleContainerData, FuelSystem.FuelSystem fuelSystem)
        {
            _view = new View.TransmissionView(data, prefabAxleCollider, prefabAxleController, body, vehicleContainerData);
            _model = new Model.TransmissionModel(data, vehicleContainerData, fuelSystem);
        }
        public void SetInput(Input.VehicleInput input)
        {
            _model.SetInput(input);
            _view.SetInput(input);
        }
        public void Update()
        {
            _view.Update(_model.GetTorque(_view.GetRPM()));
        }
        public void Dispose()
        {
            _model.Dispose();
        }
    }
}