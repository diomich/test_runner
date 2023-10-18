using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Gameplay.Objects
{
    /// <summary>
    /// A simple monobehaviour implementation of IWorldObjectView interface.
    /// Used for all simple world objects.
    /// </summary>
    public class MonobehaviourView : MonoBehaviour, IWorldObjectView
    {
        private Transform _transform;
        void Awake()
        {
            _transform = this.transform;
        }
        
        public void SetPosition(Vector3 speed)
        {
            _transform.position = new UnityEngine.Vector3(speed.X, speed.Y, speed.Z);
        }
    }
}