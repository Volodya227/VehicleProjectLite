using UnityEngine;
namespace Systems.Vehicle.Data {
    [CreateAssetMenu(fileName = "data Vehicle item", menuName = "Vehicle/DATA/GearBox")]
    public class GearBoxManualControlData : ScriptableObject
    {
        [Header("View")]
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int angleX;
        [SerializeField] private int angleZ;
        public GameObject Prefab => _prefab;
        public int AngleX => angleX;
        public int AngleZ => angleZ;
        [Header("Model"), Header("Shift Limits")]
        [SerializeField] private Vector2Int _xLimit = new Vector2Int(-1, 1);
        [SerializeField] private Vector2Int _zLimit = new Vector2Int(-2, 2);
        //manual
        [Header("Forward Gears (Gear forward)")]
        [SerializeField] private Vector2Int[] _forwardGearPositions;
        [Header("Reverse Gears (Gear reverse)")]
        [SerializeField] private Vector2Int[] _reverseGearPositions;
        public Vector2Int XLimit => _xLimit;
        public Vector2Int ZLimit => _zLimit;
        public Vector2Int[] ForwardGearPositions => _forwardGearPositions;
        public Vector2Int[] ReverseGearPositions => _reverseGearPositions;
    }
}