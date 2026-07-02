using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    [SerializeField] private PlayerAttackType attackType;
    [SerializeField] private PlayerAttackRecoil playerRecoil;

    public PlayerAttackType AttackType => attackType;
    public PlayerAttackRecoil PlayerRecoil => playerRecoil;

    private void Awake()
    {
        if (playerRecoil == null)
        {
            playerRecoil = GetComponentInParent<PlayerAttackRecoil>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyWeakPoint weakPoint = other.GetComponent<EnemyWeakPoint>();

        if (weakPoint == null)
        {
            return;
        }

        weakPoint.ReceiveAttack(this);
    }
}