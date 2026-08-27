using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMapController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PauseManager pauseManager;
    [SerializeField] private CollectibleManager collectibleManager;

    [Header("Discs")]
    [SerializeField] private PauseMapDisc[] discObjects;

    [Header("Disc Stack")]
    [SerializeField] private Transform discStack;
    [SerializeField] private float discSpacing = 1.2f;
    [SerializeField] private float stackMoveSpeed = 6f;

    [Header("Current Player Location")]
    [SerializeField] private string currentPlayerZoneId = "zone1";

    private int selectedDiscIndex;
    private Vector3 stackStartPosition;
    private Vector3 targetStackPosition;

    public int SelectedDiscIndex => selectedDiscIndex;

    private void Awake()
    {
        if (collectibleManager == null)
        {
            collectibleManager = CollectibleManager.Instance;
        }

        if (discStack != null)
        {
            stackStartPosition = discStack.localPosition;
            targetStackPosition = stackStartPosition;
        }
    }

    private void Start()
    {
        InitializeDiscs();
        FocusPlayerLocation();
    }

    private void Update()
    {
        if (pauseManager == null || !pauseManager.IsPaused)
        {
            return;
        }

        HandleInput();
        UpdateStackPosition();
    }

    private void HandleInput()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            RotateSelectedDiscLeft();
        }

        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            RotateSelectedDiscRight();
        }

        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            SelectPreviousDisc();
        }

        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            SelectNextDisc();
        }
    }

    private void InitializeDiscs()
    {
        if (collectibleManager == null || discObjects == null)
        {
            return;
        }

        for (int i = 0; i < discObjects.Length; i++)
        {
            PauseMapDisc discObject = discObjects[i];

            if (discObject == null)
            {
                continue;
            }

            ProgressionDiscDefinition definition =
                collectibleManager.GetDisc(discObject.DiscId);

            int visibleZoneCount = 1;

            if (definition != null)
            {
                visibleZoneCount = 0;

                for (int zoneIndex = 0;
                     zoneIndex < definition.Zones.Count;
                     zoneIndex++)
                {
                    ZoneProgressDefinition zone =
                        definition.Zones[zoneIndex];

                    if (zone != null && zone.ShowInProgression)
                    {
                        visibleZoneCount++;
                    }
                }

                visibleZoneCount = Mathf.Max(
                    1,
                    visibleZoneCount
                );
            }

            discObject.Initialize(
                0,
                visibleZoneCount
            );
        }
    }

    public void FocusPlayerLocation()
    {
        if (collectibleManager == null || discObjects == null)
        {
            return;
        }

        ProgressionDiscDefinition playerDisc =
            collectibleManager.GetDiscContainingZone(
                currentPlayerZoneId
            );

        if (playerDisc == null)
        {
            selectedDiscIndex = 0;
            UpdateTargetStackPosition();
            return;
        }

        int discObjectIndex =
            FindDiscObjectIndex(playerDisc.DiscId);

        if (discObjectIndex < 0)
        {
            selectedDiscIndex = 0;
            UpdateTargetStackPosition();
            return;
        }

        selectedDiscIndex = discObjectIndex;

        int zoneIndex =
            GetVisibleZoneIndex(
                playerDisc,
                currentPlayerZoneId
            );

        if (zoneIndex >= 0)
        {
            discObjects[selectedDiscIndex]
                .SetSection(zoneIndex);
        }

        UpdateTargetStackPosition();
    }

    private int FindDiscObjectIndex(string discId)
    {
        for (int i = 0; i < discObjects.Length; i++)
        {
            if (
                discObjects[i] != null &&
                discObjects[i].DiscId == discId
            )
            {
                return i;
            }
        }

        return -1;
    }

    private int GetVisibleZoneIndex(
        ProgressionDiscDefinition disc,
        string zoneId
    )
    {
        if (disc == null)
        {
            return -1;
        }

        int visibleIndex = 0;

        for (int i = 0; i < disc.Zones.Count; i++)
        {
            ZoneProgressDefinition zone =
                disc.Zones[i];

            if (
                zone == null ||
                !zone.ShowInProgression
            )
            {
                continue;
            }

            if (zone.ZoneId == zoneId)
            {
                return visibleIndex;
            }

            visibleIndex++;
        }

        return -1;
    }

    private void RotateSelectedDiscLeft()
    {
        if (!HasSelectedDisc())
        {
            return;
        }

        discObjects[selectedDiscIndex]
            .RotateLeft();
    }

    private void RotateSelectedDiscRight()
    {
        if (!HasSelectedDisc())
        {
            return;
        }

        discObjects[selectedDiscIndex]
            .RotateRight();
    }

    private void SelectPreviousDisc()
    {
        if (
            discObjects == null ||
            discObjects.Length == 0
        )
        {
            return;
        }

        selectedDiscIndex--;

        if (selectedDiscIndex < 0)
        {
            selectedDiscIndex =
                discObjects.Length - 1;
        }

        UpdateTargetStackPosition();
    }

    private void SelectNextDisc()
    {
        if (
            discObjects == null ||
            discObjects.Length == 0
        )
        {
            return;
        }

        selectedDiscIndex++;

        if (selectedDiscIndex >= discObjects.Length)
        {
            selectedDiscIndex = 0;
        }

        UpdateTargetStackPosition();
    }

    private void UpdateTargetStackPosition()
    {
        if (discStack == null)
        {
            return;
        }

        targetStackPosition =
            stackStartPosition +
            Vector3.up *
            selectedDiscIndex *
            discSpacing;
    }

    private void UpdateStackPosition()
    {
        if (discStack == null)
        {
            return;
        }

        discStack.localPosition =
            Vector3.MoveTowards(
                discStack.localPosition,
                targetStackPosition,
                stackMoveSpeed *
                Time.unscaledDeltaTime
            );
    }

    private bool HasSelectedDisc()
    {
        return
            discObjects != null &&
            selectedDiscIndex >= 0 &&
            selectedDiscIndex < discObjects.Length &&
            discObjects[selectedDiscIndex] != null;
    }
}