using UnityEngine;

public class EnemyWeakPoint : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private PlayerAttackType requiredAttackType;
    [SerializeField] private bool recoilOnWrongAttack = true;

    private void Awake()
    {
        if (enemy == null)
        {
            enemy = GetComponentInParent<Enemy>();
        }
    }

    public void ReceiveAttack(PlayerAttackHitbox attackHitbox)
    {
        if (enemy == null || enemy.IsDead)
        {
            return;
        }

        if (attackHitbox.AttackType == requiredAttackType)
        {
            enemy.Die();
            return;
        }

        if (recoilOnWrongAttack && attackHitbox.PlayerRecoil != null)
        {
            attackHitbox.PlayerRecoil.RecoilFrom(transform.position);
        }
    }
}