using UnityEngine;
namespace Systems.Vehicle.Data
{
    [CreateAssetMenu(fileName = "data Vehicle item", menuName = "Vehicle/DATA/GearBoxAutomatic")]
    public class GearBoxAutomaticControlData : ScriptableObject
    {
        [Header("View")]
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int angleX;
        [SerializeField] private int angleZ;
        public GameObject Prefab => _prefab;
        public int AngleX => angleX;
        public int AngleZ => angleZ;
        [Header("Model"), Header("Shift Limits")]
        [SerializeField] private Vector2Int _xLimit = new(0, 1);
        [SerializeField] private Vector2Int _zLimit = new(-3, 1);
        [Header("Mode Positions")]
        [SerializeField] private Vector2Int _modeR = new(0, 1);
        [SerializeField] private Vector2Int _modeN = new(0, 0);
        [SerializeField] private Vector2Int _modeD = new(0, -1);
        [SerializeField] private Vector2Int _modeM = new(1, -2);
        public Vector2Int XLimit => _xLimit;
        public Vector2Int ZLimit => _zLimit;
        public Vector2Int ModeR => _modeR;
        public Vector2Int ModeN => _modeN;
        public Vector2Int ModeD => _modeD;
        public Vector2Int ModeM => _modeM;
    }
}