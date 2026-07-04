using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerDamageKnockback : MonoBehaviour
{
    [SerializeField] private float knockbackDistance = 3f;
    [SerializeField] private float knockbackDuration = 0.18f;
    [SerializeField] private float upwardKnockback = 0.6f;

    private CharacterController characterController;
    private Coroutine knockbackCoroutine;

    public bool IsKnockingBack => knockbackCoroutine != null;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void KnockbackFrom(Vector3 sourcePosition)
    {
        Vector3 direction = transform.position - sourcePosition;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
        {
            direction = -transform.forward;
        }

        direction.Normalize();

        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
        }

        knockbackCoroutine = StartCoroutine(Knockback(direction));
    }

    private IEnumerator Knockback(Vector3 direction)
    {
        float timer = 0f;
        Vector3 knockbackVelocity = direction * (knockbackDistance / knockbackDuration);
        knockbackVelocity.y = upwardKnockback / knockbackDuration;

        while (timer < knockbackDuration)
        {
            characterController.Move(knockbackVelocity * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        knockbackCoroutine = null;
    }
}