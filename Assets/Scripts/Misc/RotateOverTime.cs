using UnityEngine;

namespace Misc
{
    /// <summary>
    /// A simple script to rotate the game object it is attached to over time with the given settings.
    /// </summary>
    public class RotateOverTime : MonoBehaviour
    {
        [SerializeField] 
        private float _rotationSpeed;

        [SerializeField] 
        private Vector3 _rotateAxis;

        private Transform _transform;
        
        private void Awake()
        {
            _transform = this.transform;
        }

        private void Update()
        {
            _transform.Rotate(_rotateAxis, _rotationSpeed * Time.deltaTime);
        }
    }
}
