using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerAttackRecoil : MonoBehaviour
{
    [SerializeField] private float recoilDistance = 2f;
    [SerializeField] private float recoilDuration = 0.15f;
    [SerializeField] private float upwardRecoil = 0.4f;

    private CharacterController characterController;
    private Coroutine recoilCoroutine;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void RecoilFrom(Vector3 sourcePosition)
    {
        Vector3 direction = transform.position - sourcePosition;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
        {
            direction = -transform.forward;
        }

        direction.Normalize();

        if (recoilCoroutine != null)
        {
            StopCoroutine(recoilCoroutine);
        }

        recoilCoroutine = StartCoroutine(Recoil(direction));
    }

    private IEnumerator Recoil(Vector3 direction)
    {
        float timer = 0f;
        Vector3 recoilVelocity = direction * (recoilDistance / recoilDuration);
        recoilVelocity.y = upwardRecoil / recoilDuration;

        while (timer < recoilDuration)
        {
            characterController.Move(recoilVelocity * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        recoilCoroutine = null;
    }
}