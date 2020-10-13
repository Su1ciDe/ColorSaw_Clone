using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float speed = 10f;
    public float minSpeed = 1.5f;
    public float sensitivity = 0.02f;
    public float turnSensitivity = 10f;
    public float stopThreshold = 0.4f;

    private bool movementReset;

    private Rigidbody rb;

    private Vector3 playerPos;
    private Vector3 playerStartPos;

    private Vector3 mousePos;
    private Vector3 mouseStartPos;

    private Vector3 direction;
    private Vector3 targetPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ResetMovement();
    }

    private void Update()
    {
        GetTargetPosition();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void GetTargetPosition()
    {
        if (GameManager.instance.gameOver || GameManager.instance.stageFailed)
            return;

        playerPos = rb.position;
        mousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            ResetMovement();
        }
        else if (Input.GetMouseButton(0))
        {
            var mouseDir = mousePos - mouseStartPos;
            if (direction == Vector3.zero)
            {
                if (mouseDir.magnitude < turnSensitivity)
                {
                    return;
                }
                direction = mouseDir.normalized;
                direction = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? new Vector3(1, 0, 0) : new Vector3(0, 0, 1);
            }

            var mouseDistance = mousePos - mouseStartPos;
            if (direction.x != 0 && mousePos.x != mouseStartPos.x)
            {
                var distance = new Vector3(Mathf.RoundToInt(mouseDistance.x * sensitivity), 0, mouseDistance.y);
                targetPos = playerStartPos + direction * distance.x;
            }
            else if (direction.z != 0 && mousePos.y != mouseStartPos.y)
            {
                var distance = new Vector3(mouseDistance.x, 0, Mathf.RoundToInt(mouseDistance.y * sensitivity));
                targetPos = playerStartPos + direction * distance.z;
            }
            else
            {
                targetPos = playerStartPos;
            }
        }

        if ((targetPos - playerPos).magnitude < stopThreshold)
        {
            if (!movementReset)
            {
                ResetMovement();
            }
        }
        else
        {
            movementReset = false;
        }
    }

    private void Move()
    {
        if (playerPos != targetPos)
        {   // Constraint moves by ground's size
            targetPos = new Vector3(Mathf.Clamp(targetPos.x, -12, 12), 0, Mathf.Clamp(targetPos.z, -25, 25 - GameManager.player.furthestBlockZ));
            rb.MovePosition(Vector3.MoveTowards(playerPos, targetPos, speed * Mathf.Max(minSpeed, (playerPos - targetPos).magnitude) * Time.fixedDeltaTime));
        }
    }

    private void ResetMovement()
    {
        movementReset = true;
        direction = Vector3.zero;
        mouseStartPos = Input.mousePosition;
        playerStartPos = new Vector3(Mathf.RoundToInt(playerPos.x), 0, Mathf.RoundToInt(playerPos.z));
    }
}