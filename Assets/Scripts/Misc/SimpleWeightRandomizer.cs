using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Misc
{
    /// <summary>
    /// Simple implementation of weight-based random. Provides an intuitive
    /// interface to set up the relative probability for every entry. 
    /// </summary>
    /// <typeparam name="T">Type of value to be randomized.</typeparam>
    public class SimpleWeightRandomizer<T>
    {
        private List<int> _weights;
        private List<T> _values;
        private List<int> _resultWeight;

        private int _totalWeight;
        
        public SimpleWeightRandomizer(int length = 0)
        {
            _weights = new List<int>(length);
            _resultWeight = new List<int>(length);
            _values = new List<T>(length);
        }

        public void Add(int weight, T value)
        {
            _weights.Add(weight);
            _values.Add(value);
            RecalculateWeights();
        }

        public T Next()
        {
            int val = Random.Range(0, _totalWeight);
            int resultIndex = 0;
            while ( resultIndex < _weights.Count && _resultWeight[resultIndex] <= val)
            {
                resultIndex++;
            }

            return _values[resultIndex];
        }

        private void RecalculateWeights()
        {
            _totalWeight = _weights.Sum();
            _resultWeight.Clear();
            int prev = 0;
            for (int i = 0; i < _weights.Count; i++)
            {
                _resultWeight.Add(prev + _weights[i]);
                prev = _resultWeight[i];
            }
        }
    }
}