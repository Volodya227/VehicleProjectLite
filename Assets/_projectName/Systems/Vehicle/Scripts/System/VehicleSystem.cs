using UnityEngine;
namespace Systems.Vehicle.Core
{
    public interface IVehicleSystemPassenger
    {
        public Transform Transform { get; }
        public Passenger.VehiclePassengerSystem GetVehiclePassengerSystem { get; }

        public void SetInput(Input.VehicleInput input);
        public void SetActiveMirror(bool player = false);
    }
    public interface IVehicleLifecycle
    {
        public void SetData(Data.VehicleData data);
        public void SetMirrorSystem(Visual.Mirror.MirrorVehicleSystem value);
    }
    [System.Serializable]
    public class VehicleSystem : IVehicleSystemPassenger, IVehicleLifecycle
    {
        private bool _destroyed;
        private float _distance;
        private Visual.Mirror.MirrorVehicleSystem _vehicleMirror;
        private bool _player;
        private readonly GameObject _prefabAxleWheelCollider;
        private readonly GameObject _prefabAxleWheelController;
        private Input.VehicleInput _input;
        private readonly Input.VehicleInputNullRef _inputNullRef;
        private ContainerData.VehicleContainerData _vehicleContainerData;
        [SerializeField] private Transmission.Core.TransmissionSystem _transmission;
        private Visual.VehicleVisualSystem _visualSystem;
        private Passenger.VehiclePassengerSystem _passengerSystem;
        private Trailer.Attach.Core.AttachSystem _attachSystem;
        private readonly Rigidbody _body;
        [SerializeField] private Data.VehicleData _vehicleData;
        private FuelSystem.FuelSystem _fuelSystem;
        [SerializeField] private AudioSystem.AudioSystem _audioSource;
        public Transform Transform => _body.transform;
        public Passenger.VehiclePassengerSystem GetVehiclePassengerSystem => _passengerSystem;
        public VehicleSystem(Rigidbody body, GameObject prefabAxleWheelCollider, GameObject prefabAxleWheelController)
        {
            _inputNullRef = new Input.VehicleInputNullRef();
            _body = body;
            _destroyed = false;
            _prefabAxleWheelCollider = prefabAxleWheelCollider;
            _prefabAxleWheelController = prefabAxleWheelController;
        }
        public void Init(AudioSource audioSource)
        {
            _vehicleContainerData = new ContainerData.VehicleContainerData(axleCount: _vehicleData.AxleCount);
            InitVisualSystem();
            _body.centerOfMass = _visualSystem.GetModelItem.CenterOfMass;
            _body.mass = _vehicleData.VehicleMass;
            InitFuelSystem();
            InitTransmissionSystem();
            InitPassengerSystem();
            InitAttachSystem();
            SetInput(null);
            _audioSource = new AudioSystem.AudioSystem(audioSource, _vehicleContainerData.EngineState, _vehicleData);
        }
        public void Destroyed() {
            _destroyed = true;
            SetInput(null);
            _audioSource.Disable();
            _transmission.Dispose();
        }
        public void SetData(Data.VehicleData data)
        {
            _vehicleData = data;
        }
        public void SetMirrorSystem(Visual.Mirror.MirrorVehicleSystem value) { _vehicleMirror = value; }
        private void InitTransmissionSystem()
        {
            _transmission = new Transmission.Core.TransmissionSystem(_body, _vehicleData, _prefabAxleWheelCollider, _prefabAxleWheelController, _vehicleContainerData, _fuelSystem);
        }
        private void InitFuelSystem()
        {
            _fuelSystem = new FuelSystem.FuelSystem(_vehicleData, _vehicleContainerData);
        }
        private void InitVisualSystem()
        {
            Visual.ModelItem modelItem = Object.Instantiate(original: _vehicleData.ModelPrefab, parent: _body.transform);
            _visualSystem = new Visual.VehicleVisualSystem(modelItem, _vehicleContainerData, _vehicleData);
            modelItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
        private void InitPassengerSystem()
        {
            _passengerSystem = new Passenger.VehiclePassengerSystem(_visualSystem.GetModelItem, _vehicleContainerData);
            _passengerSystem.ResetInput += () => SetInput(null);
        }
        private void InitAttachSystem()
        {
            _attachSystem = new Trailer.Attach.Core.AttachSystem(_body);
            foreach (Trailer.Attach.Core.Attachment item in _visualSystem.GetModelItem.Attachments)
            {
                _attachSystem.AddAttach(item);
            }
        }
        public void SetInput(Input.VehicleInput input = null)
        {
            float accelerator = 0;
            if (_input != null)
            {
                accelerator = _input.Accelerator;
                _input.SetActive(false);
            }
            _input = input ?? _inputNullRef;
            _transmission.SetInput(_input);
            _attachSystem.SetInput(_input);
            //TODO _visualSystem.setInput(_input);
            if (_input != null)
            {
                _input.SetActive(true);
            }
            _input.SetAcceleratorFake(accelerator);
        }
        public void SetActiveMirror(bool player = false)
        {
            if (_destroyed) return;
            if (_player)
            {
                if (!player)
                {
                    _vehicleMirror.OffMirror();//TODO visual
                }
            }
            _player = player;
            if (_player)
            {
                _vehicleMirror.OnMirror(_visualSystem.GetModelItem._leftMirror, _visualSystem.GetModelItem._rightMirror, _visualSystem.GetModelItem._centerMirror);
            }
        }
        public void Update()
        {
            _vehicleContainerData.SetSpeed((int)(_body.velocity.magnitude * 3.6f));
            _distance += _body.velocity.magnitude * Time.fixedDeltaTime;
            _vehicleContainerData.SetDistance((int)_distance);
            if (_input != null)
            {
                //set input
                _vehicleContainerData.SetClutch(_input.Clutch);
                _vehicleContainerData.SetBrake(_input.Braking);
                //_vehicleContainerData.SetAccelerator(_input.Accelerator);
            }
            _transmission.Update();
            _audioSource.Update();
        }
    }
}