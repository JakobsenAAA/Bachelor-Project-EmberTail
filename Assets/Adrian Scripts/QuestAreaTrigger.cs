
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class QuestAreaTrigger : MonoBehaviour
{
    [Header("Quest Area")]
    [SerializeField] private string areaId;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private bool triggerOnlyOnce = true;

    private bool triggered;

    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered && triggerOnlyOnce)
        {
            return;
        }

        if ((playerLayer.value & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }

        if (QuestManager.Instance == null)
        {
            return;
        }

        QuestManager.Instance.ReportAreaReached(areaId);

        if (triggerOnlyOnce)
        {
            triggered = true;
        }
    }
}
