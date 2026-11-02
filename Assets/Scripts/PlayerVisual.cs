using System.Collections;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Animator player_Animator;
    [SerializeField] private SpriteRenderer player_SpriteRenderer;

    [SerializeField] private Rigidbody2D player_RigidBody2D;
    [SerializeField] private float animationLerpDuration;

    private void Update()
    {
        float playerMovementX = Input.GetAxisRaw("Horizontal");
        //if (Mathf.Floor(player_RigidBody2D.linearVelocity.x) > 0)
        if (playerMovementX > 0)
        {
            player_SpriteRenderer.flipX = false;
            StartCoroutine(AnimationLerp(animationLerpDuration, "Movement Animation Value", 1));

        }
        //else if (Mathf.Floor(player_RigidBody2D.linearVelocity.x) < 0)
        else if (playerMovementX < 0)
        {
            player_SpriteRenderer.flipX = true;
            StartCoroutine(AnimationLerp(animationLerpDuration, "Movement Animation Value", -1));
        }
        //else if(Mathf.Floor(player_RigidBody2D.linearVelocity.x) == 0)
        else if(playerMovementX == 0)
        {
            StartCoroutine(AnimationLerp(animationLerpDuration, "Movement Animation Value", 0));
        }

        if(Mathf.Round(player_RigidBody2D.linearVelocity.y) > 0)
        {
            player_Animator.SetBool("Jump", true);
        }
        else
        {
            player_Animator.SetBool("Jump", false);
        }
    }

    IEnumerator AnimationLerp(float duration, string animationName, float value)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float clampValue = Mathf.Clamp01(elapsedTime / duration);
            float lerpValue = Mathf.Lerp(player_Animator.GetFloat(animationName), value, clampValue);
            player_Animator.SetFloat(animationName, lerpValue);
            yield return null;
        }
        player_Animator.SetFloat(animationName, value);
    }
}
