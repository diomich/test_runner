using System;
using System.Collections;
using Sound;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Gameplay.Objects
{
    /// <summary>
    /// MonoBehaviour-based implementation of ICharacterView interface.
    /// Animations are controlled by the Animator component.
    /// </summary>
    public class CharacterView : MonoBehaviour, ICharacterView
    {
        [SerializeField]
        private Animator _animator;
        [SerializeField] 
        private float _rotateToGreetDuration = 0.5f;
        [SerializeField] 
        private float _greetDuration = 2f;
        [SerializeField] 
        private float _rotateFromGreetDuration = 0.5f;

        private Transform _transform;
        
        void Awake()
        {
            _transform = this.transform;
        }
        
        void IWorldObjectView.SetPosition(Vector3 position)
        {
            _transform.position = new UnityEngine.Vector3(position.X, position.Y, position.Z);
        }

        void ICharacterView.Greet(ISoundManager soundManager, Action onComplete)
        {
            StartCoroutine(CoGreetings(soundManager, onComplete));
        }

        void ICharacterView.SetIsMoving(bool isMoving)
        {
            _animator.SetFloat("MoveSpeed", isMoving ? 1 : 0);
        }

        void ICharacterView.SetIsGrounded(bool isGrounded)
        {
            _animator.SetBool("Grounded", isGrounded);
        }

        private IEnumerator CoGreetings(ISoundManager soundManager, Action onComplete)
        {
            soundManager.PauseBackground();
            Quaternion initRotation = this.transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(0f, -180f, 0f);

            yield return RotateFromTo(this.transform, initRotation, targetRotation, _rotateToGreetDuration);
            _animator.SetTrigger("Wave");    
            soundManager.PlayOneshot(SoundConstants.SFX_GREET);
            yield return new WaitForSeconds(_greetDuration);
            yield return RotateFromTo(this.transform, targetRotation, initRotation, _rotateFromGreetDuration);
            onComplete?.Invoke();
            soundManager.ResumeBackground();
        }

        private IEnumerator RotateFromTo(Transform trans, Quaternion from, Quaternion to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                trans.rotation = Quaternion.Slerp(from, to, elapsed / duration);
                yield return null;
            }
        }
    }
}