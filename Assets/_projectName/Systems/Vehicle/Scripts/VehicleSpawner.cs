using System.Collections.Generic;

using UnityEngine;
namespace Systems.Vehicle.Core
{
    using Visual.Mirror;
    [System.Serializable]
    public class ItemVehicle
    {
        public Data.VehicleData vehicleData;
    }
    public class VehicleSpawner : MonoBehaviour
    {
        [SerializeField] private VehicleSystemBehaviour _prefabVehicleSystem;
        [SerializeField] private MirrorVehicleSystem _mirrorVehicleSystem;
        public List<ItemVehicle> vehiclesData = new();
        [SerializeField] private List<VehicleSystemBehaviour> _vehicles;
        //[Header("position for create Vehicle")]
        public List<Transform> positions { private get; set; }
        public void SetVehicleMirror(MirrorVehicleSystem mirrorVehicleSystem)
        {
            _mirrorVehicleSystem = mirrorVehicleSystem;
        }
        private void CreateVehicle(Transform transform1, int id)
        {
            VehicleSystemBehaviour newVehicle = Instantiate(original: _prefabVehicleSystem, position: transform1.position, rotation: transform1.rotation);
            IVehicleLifecycle System = newVehicle.VehicleLifecycle;
            System.SetData(vehiclesData[id].vehicleData);
            System.SetMirrorSystem(_mirrorVehicleSystem);
            newVehicle.enabled = true;
            _vehicles.Add(newVehicle);
        }
        private void Start()
        {
            _vehicles = new List<VehicleSystemBehaviour>();
            for (int id = 0; id < vehiclesData.Count; id++)
            {
                CreateVehicle(positions[id], id);
            }
        }
    }
}