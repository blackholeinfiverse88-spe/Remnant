using UnityEngine;

public class AssistantSimpleMovement : MonoBehaviour
{
    [Header("Destinations")]
    public Transform playerStopPoint;
    public Transform pcStopPoint;

    [Header("References")]
    public Transform player;
    public Animator animator;

    [Header("Movement")]
    public float runSpeed = 2.5f;
    public float pcWalkSpeed = 1.5f;
    public float rotationSpeed = 360f;
    public float stoppingDistance = 0.2f;

    [Header("Animation")]
    public string runningParameter = "IsRunning";

    private Transform currentDestination;
    private float currentSpeed;
    private bool isMoving;
    private bool movementStarted;

    public bool HasArrived { get; private set; }

    private void Awake()
    {
        // Find the Animator if it has not been assigned.
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogError("No Animator found on the assistant!", this);
    }

    private void Start()
    {
        SetRunning(false);
    }

    // Call this when the assistant's door opens.
    public void StartRunning()
    {
        if (movementStarted)
            return;

        if (playerStopPoint == null)
        {
            Debug.LogError("Player Stop Point is not assigned!", this);
            return;
        }

        movementStarted = true;
        MoveTo(playerStopPoint, runSpeed);

        Debug.Log("Assistant started running to the player.");
    }

    // Call this after the conversation finishes.
    public void GoToPC()
{
    if (pcStopPoint == null)
    {
        Debug.LogError("PC Stop Point is not assigned!", this);
        return;
    }

    // Run to the PC instead of walking.
    MoveTo(pcStopPoint, runSpeed);

    Debug.Log("Assistant is running to the PC.");
}

    private void MoveTo(Transform destination, float speed)
    {
        currentDestination = destination;
        currentSpeed = speed;
        isMoving = true;
        HasArrived = false;

        SetRunning(true);
    }

    private void Update()
    {
        if (!isMoving || currentDestination == null)
            return;

        Vector3 target = currentDestination.position;
        target.y = transform.position.y;

        Vector3 direction = target - transform.position;
        direction.y = 0f;

        if (direction.magnitude <= stoppingDistance)
        {
            isMoving = false;
            SetRunning(false);

            if (currentDestination == playerStopPoint)
            {
                FacePlayer();
            }
            else
            {
                HasArrived = true;
            }

            Debug.Log("Assistant reached destination.");
            return;
        }

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            currentSpeed * Time.deltaTime
        );
    }

    private void FacePlayer()
    {
        if (player == null)
        {
            HasArrived = true;
            return;
        }

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        HasArrived = true;
    }

    private void SetRunning(bool running)
    {
        if (animator == null)
            return;

        if (animator.isActiveAndEnabled)
        {
            animator.SetBool(runningParameter, running);

            Debug.Log(
                "Assistant animation: " + runningParameter +
                " = " + running
            );
        }
    }
}