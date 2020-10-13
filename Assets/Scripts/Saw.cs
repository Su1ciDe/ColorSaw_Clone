using UnityEngine;

public class Saw : MonoBehaviour
{
    [Header("Rotation")]
    public bool rotate = false;
    public Transform partToRotate;
    public Vector3 rotation;

    [Header("Movement")]
    public bool move = false;
    public Vector3 movePosition;
    public float moveSpeed = 1;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
        if (move)
            DrawPath();
    }

    private void FixedUpdate()
    {
        if (move)
            Move();

        if (rotate)
            Rotate();
    }

    private void Move()
    {
        transform.position = Vector3.Lerp(startPosition, movePosition, Mathf.PingPong(Time.time * moveSpeed, 1));
    }

    private void Rotate()
    {
        partToRotate.Rotate(rotation, rotation.magnitude * Time.deltaTime);
    }

    private void DrawPath()
    {
        float sizeX = transform.localScale.x;
        float sizeZ = transform.localScale.z;

        GameObject sawPath = Instantiate(GameManager.instance.sawPath_Sprite, transform.parent);
        Vector3 pos = (movePosition + startPosition) / 2;
        pos.y = -0.49f;
        sawPath.transform.position = pos;

        Vector3 dir = movePosition - startPosition;
        float scaleX = dir.x != 0 ? Mathf.Ceil((dir.x + sizeX - 1) / sizeX) : 1;
        float scaleZ = dir.z != 0 ? Mathf.Ceil((dir.z + sizeZ - 1) / sizeZ) : 1;
        sawPath.transform.localScale = new Vector3(sawPath.transform.localScale.x * sizeX * scaleX, sawPath.transform.localScale.y * sizeZ * scaleZ, sawPath.transform.localScale.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        Block block = other.GetComponent<Block>();
        if (block)
            block.DestroyBlock();
    }
}