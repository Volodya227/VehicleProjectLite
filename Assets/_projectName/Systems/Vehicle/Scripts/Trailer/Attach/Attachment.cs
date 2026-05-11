using UnityEngine;
namespace Systems.Vehicle.Trailer.Attach.Core
{
    public class Attachment : MonoBehaviour
    {
        public event System.Action EventInputChange;
        public event System.Action EventChanged;
        public event System.Action EventAttached;
        public event System.Action EventDettached;
        public Data.DataAttach Data { private set; get; }
        public bool Active { private set; get; } = false;
        private int _newMass;
        private Input.TrailerInput _input;
        public Input.TrailerInput GetInput => _input;

        public void SetData(Data.DataAttach data)
        {
            if (Data != null) return;
            Data = data;
        }
        public bool TryAttach()
        {
            //check type attach return false
            Active = true;
            EventAttached?.Invoke();
            return true;
        }
        public void Dettach()
        {
            if (Active)
            {
                Active = false;
                EventDettached?.Invoke();
            }
        }
        public int GetMass()
        {
            return _newMass;
        }
        public void UpdateMass(int mass)
        {
            if (Active)
            {
                _newMass = mass;
                EventChanged?.Invoke();
            }
        }
        public void SetInput(Input.TrailerInput input)
        {
            _input = input;
            EventInputChange?.Invoke();
        }
    }
}