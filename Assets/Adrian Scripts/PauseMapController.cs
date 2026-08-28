using UnityEngine;
using UnityEngine.Events;
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

    public UnityEvent OnSelectionChanged;

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
                visibleZoneCount = GetVisibleZoneCount(definition);
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
            NotifySelectionChanged();

            return;
        }

        int discObjectIndex =
            FindDiscObjectIndex(
                playerDisc.DiscId
            );

        if (discObjectIndex < 0)
        {
            selectedDiscIndex = 0;

            UpdateTargetStackPosition();
            NotifySelectionChanged();

            return;
        }

        selectedDiscIndex =
            discObjectIndex;

        int zoneIndex =
            GetVisibleZoneIndex(
                playerDisc,
                currentPlayerZoneId
            );

        if (
            zoneIndex >= 0 &&
            discObjects[selectedDiscIndex] != null
        )
        {
            discObjects[selectedDiscIndex]
                .SetSection(zoneIndex);
        }

        UpdateTargetStackPosition();
        NotifySelectionChanged();
    }

    public ProgressionDiscDefinition GetSelectedDiscDefinition()
    {
        if (
            collectibleManager == null ||
            discObjects == null ||
            selectedDiscIndex < 0 ||
            selectedDiscIndex >= discObjects.Length ||
            discObjects[selectedDiscIndex] == null
        )
        {
            return null;
        }

        return collectibleManager.GetDisc(
            discObjects[selectedDiscIndex].DiscId
        );
    }

    public ZoneProgressDefinition GetSelectedZoneDefinition()
    {
        ProgressionDiscDefinition disc =
            GetSelectedDiscDefinition();

        if (disc == null)
        {
            return null;
        }

        if (
            discObjects == null ||
            selectedDiscIndex < 0 ||
            selectedDiscIndex >= discObjects.Length ||
            discObjects[selectedDiscIndex] == null
        )
        {
            return null;
        }

        int visibleSectionIndex =
            discObjects[selectedDiscIndex]
                .CurrentSectionIndex;

        int currentVisibleIndex = 0;

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

            if (
                currentVisibleIndex ==
                visibleSectionIndex
            )
            {
                return zone;
            }

            currentVisibleIndex++;
        }

        return null;
    }

    public bool IsSelectedDiscRotating()
    {
        if (!HasSelectedDisc())
        {
            return false;
        }

        return discObjects[selectedDiscIndex]
            .IsRotating;
    }

    private int FindDiscObjectIndex(
        string discId
    )
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

    private int GetVisibleZoneCount(
        ProgressionDiscDefinition disc
    )
    {
        int count = 0;

        for (int i = 0; i < disc.Zones.Count; i++)
        {
            ZoneProgressDefinition zone =
                disc.Zones[i];

            if (
                zone != null &&
                zone.ShowInProgression
            )
            {
                count++;
            }
        }

        return Mathf.Max(
            1,
            count
        );
    }

    private void RotateSelectedDiscLeft()
    {
        if (!HasSelectedDisc())
        {
            return;
        }

        discObjects[selectedDiscIndex]
            .RotateLeft();

        NotifySelectionChanged();
    }

    private void RotateSelectedDiscRight()
    {
        if (!HasSelectedDisc())
        {
            return;
        }

        discObjects[selectedDiscIndex]
            .RotateRight();

        NotifySelectionChanged();
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
        NotifySelectionChanged();
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

        if (
            selectedDiscIndex >=
            discObjects.Length
        )
        {
            selectedDiscIndex = 0;
        }

        UpdateTargetStackPosition();
        NotifySelectionChanged();
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

    private void NotifySelectionChanged()
    {
        OnSelectionChanged.Invoke();
    }
}