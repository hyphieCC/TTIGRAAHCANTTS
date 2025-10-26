using UnityEngine;
using System.Collections;

namespace Systems
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        public Animator animator;
        private PlayerSystem player;

        [Header("Squish Settings")]
        [Range(0.5f, 1.5f)] public float squishX = 0.9f;
        [Range(0.5f, 1.5f)] public float squishY = 1.1f;
        [Range(0.01f, 0.3f)] public float squishDuration = 0.08f;

        private bool isSquishing;
        private string currentState = "";
        private bool isDead = false;
        private Direction lastFacingDirection;

        void Awake()
        {
            player = GetComponent<PlayerSystem>();
            if (animator == null)
                animator = GetComponent<Animator>();
            lastFacingDirection = player.facingDirection;
        }

        void Update()
        {
            if (isDead || player == null) return;

            string nextState = GetStateForCurrentDirectionAndStamina();
            if (nextState != currentState)
                ChangeAnimationState(nextState);

            if (player.facingDirection != lastFacingDirection)
            {
                if (!isSquishing)
                    StartCoroutine(SquishEffect());
                lastFacingDirection = player.facingDirection;
            }
        }

        string GetStateForCurrentDirectionAndStamina()
        {
            string dir = "";
            switch (player.facingDirection)
            {
                case Direction.Up: dir = "Up"; break;
                case Direction.Down: dir = "Down"; break;
                case Direction.Left: dir = "Left"; break;
                case Direction.Right: dir = "Right"; break;
            }

            string staminaState = "Healthy";
            if (player.inventory.stamina <= 49) staminaState = "Medium";
            if (player.inventory.stamina <= 24) staminaState = "Risky";

            return $"Idle_{dir}_{staminaState}";
        }

        void ChangeAnimationState(string newState)
        {
            if (string.IsNullOrEmpty(newState) || newState == currentState)
                return;

            animator.Play(newState);
            currentState = newState;
        }

        public void PlayDeathAnimation()
        {
            isDead = true;
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            animator.Play("Player_Death", 0, 0f);
            currentState = "Player_Death";
            Debug.Log("[PlayerAnimationController] Death animation triggered.");
        }

        public float GetDeathClipLength()
        {
            if (animator == null || animator.runtimeAnimatorController == null)
                return 2f;

            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == "Player_Death")
                    return clip.length;
            }

            return 2f; // fallback
        }

        private IEnumerator SquishEffect()
        {
            isSquishing = true;
            Transform t = transform;

            Vector3 normalScale = t.localScale;
            Vector3 squishScale = new Vector3(normalScale.x * squishX, normalScale.y * squishY, normalScale.z);

            float elapsed = 0f;

            while (elapsed < squishDuration)
            {
                t.localScale = Vector3.Lerp(normalScale, squishScale, elapsed / squishDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < squishDuration)
            {
                t.localScale = Vector3.Lerp(squishScale, normalScale, elapsed / squishDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            t.localScale = normalScale;
            isSquishing = false;
        }
    }
}