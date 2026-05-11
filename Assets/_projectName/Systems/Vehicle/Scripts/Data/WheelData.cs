using UnityEngine;
namespace Systems.Vehicle.Data
{
    public enum SurfaceType
    {
        Default,
        Asphalt,
        Dirt
    }
    [System.Serializable]
    public class WheelFriction
    {
        [Header("Forward Friction")]
        public float forwardExtremumSlip = 0.4f;
        public float forwardExtremumValue = 1.0f;
        public float forwardAsymptoteSlip = 0.8f;
        public float forwardAsymptoteValue = 0.5f;
        [Header("Sideways Friction")]
        public float sidewaysExtremumSlip = 0.4f;
        public float sidewaysExtremumValue = 1.0f;
        public float sidewaysAsymptoteSlip = 0.5f;
        public float sidewaysAsymptoteValue = 0.75f;
    }
    [CreateAssetMenu(fileName = "data Wheel", menuName = "Vehicle/DATA/Wheel")]
    public class WheelData : ScriptableObject
    {
        [Header("Wheel Physical")]
        [SerializeField] private float _radius = 0.5f;
        [SerializeField] private float _width = 0.3f;
        [SerializeField] private float _wheelMass = 200;
        [SerializeField] private GameObject _prefabMeshWheel;
        [Header("Base Stiffness")]
        [SerializeField] private float _forwardStiffness = 1.0f;
        [SerializeField] private float _sidewaysStiffness = 1.0f;
        [Header("Surface Friction Settings")]
        [SerializeField] private WheelFriction _defaultFriction = new();
        [SerializeField] private WheelFriction _asphaltFriction = new();
        [SerializeField] private WheelFriction _dirtFriction = new();
        [Header("Dynamic System")]
        [SerializeField, Range(0.1f, 1)] private float _minSidewaysFriction = 0.3f;
        public float Radius => _radius;
        public float Width => _width;
        public float WheelMass => _wheelMass;
        public GameObject PrefabMesh => _prefabMeshWheel;
        public float ForwardStiffness => _forwardStiffness;
        public float SidewaysStiffness => _sidewaysStiffness;
        public float MinSidewaysFriction => _minSidewaysFriction;
        public WheelFriction GetFrictionForSurface(SurfaceType surface)
        {
            if (surface == SurfaceType.Asphalt) return _asphaltFriction;
            if (surface == SurfaceType.Dirt) return _dirtFriction;
            return _defaultFriction;//can get null
        }
    }
}