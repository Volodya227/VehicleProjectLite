namespace Systems.Vehicle.Input
{
    public abstract class TrailerInput
    {
        public bool ActiveTrailer { get; private set; }
        protected float _brakingTrailer = 0;
        public float BrakingTrailer => _brakingTrailer;
        public void SetActiveTrailer(bool value)
        {
            ActiveTrailer = value;
            if (!value) {
                _brakingTrailer = 0;
            }
        }
    }
    public class TrailerInputNullRef : TrailerInput
    {
        public TrailerInputNullRef() {
            _brakingTrailer = 1;
        }
    }
}