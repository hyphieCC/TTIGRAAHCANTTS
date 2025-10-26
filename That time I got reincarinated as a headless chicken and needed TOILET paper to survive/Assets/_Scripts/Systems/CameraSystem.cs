using UnityEngine;

namespace Systems
{
    public class CameraSystem : MonoBehaviour
    {
        public Transform player;
        public float smoothTime = 0.15f;
        private Vector3 velocity;

        [Header("Shake Settings")]
        public float shakeDuration = 0.15f;
        public float shakeMagnitude = 0.25f;
        private float shakeTimer = 0f;
        private Vector3 shakeOffset = Vector3.zero;

        void LateUpdate()
        {
            if (!player) return;
            Vector3 target = new Vector3(player.position.x, player.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);

            if (shakeTimer > 0)
            {
                shakeOffset = Random.insideUnitSphere * shakeMagnitude;
                transform.position += shakeOffset;
                shakeTimer -= Time.deltaTime;
            }
        }

        public void Shake(float duration = 0f, float magnitude = 0f)
        {
            shakeTimer = duration > 0 ? duration : shakeDuration;
            shakeMagnitude = magnitude > 0 ? magnitude : this.shakeMagnitude;
        }
    }
}