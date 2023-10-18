using UnityEngine;

namespace Misc
{
    /// <summary>
    /// A simple script to ping pong object local position between two points over time.
    /// </summary>
    public class PingPongMovement : MonoBehaviour
    {
        [SerializeField] 
        private Vector3 _minPos;

        [SerializeField] 
        private Vector3 _maxPos;
        
        [SerializeField]
        float _fullCycleDuration;

        private Transform _transfrom;
        private float _elapsed;
        
        void Awake()
        {
            _transfrom = this.transform;
        }
        
        void Update()
        {
            _elapsed += Time.deltaTime;
            float val = Mathf.PingPong(_elapsed, _fullCycleDuration * 0.5f);
            _transfrom.localPosition = Vector3.Lerp(_minPos, _maxPos, val);
        }
    }
}
