using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Dictionary<Vector3, Block> blocks = new Dictionary<Vector3, Block>();

    [HideInInspector] public int furthestBlockZ = 0;

    public Transform missingPart;

    private List<Transform> originBlocks = new List<Transform>();
    [HideInInspector] public int originBlockCount;

    private bool isMoving = false;

    private void Awake()
    {
        GameManager.player = this;
    }

    private void Start()
    {
        SetupBlocks();
    }

    private void SetupBlocks()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Block childBlock = child.GetComponent<Block>();
            if (childBlock != null)
            {
                blocks.Add(child.localPosition, childBlock);

                if (childBlock.origin)
                {
                    originBlockCount++;
                    originBlocks.Add(childBlock.transform);
                }
                else
                {
                    if (furthestBlockZ < child.localPosition.z)
                        furthestBlockZ = (int)child.localPosition.z;
                }
            }
        }

        RefreshBlocks();
    }

    public void RefreshBlocks()
    {
        foreach (var block in blocks.Values)
        {
            block.connected = false;
        }

        // Find the connected blocks
        foreach (var block in blocks.Values)
        {
            if (block.origin)
            {
                block.CheckConnection();
            }
        }

        // Find the disconnected blocks
        List<Block> notConnectedBlocks = new List<Block>();
        foreach (var block in blocks.Values)
        {
            if (!block.connected)
            {
                notConnectedBlocks.Add(block);
            }
        }

        // Destroy the disconnected blocks
        foreach (var block in notConnectedBlocks)
        {
            block.DestroyBlock(false);
        }

        if (originBlockCount == blocks.Count)
        {
            // Move to the missing part
            StartCoroutine(MoveToMissingPart());
        }
    }

    public IEnumerator MoveToMissingPart()
    {
        GetComponent<PlayerController>().enabled = false;

        // Setup a parent for origin
        GameObject parent = new GameObject("OriginParent");
        parent.transform.position = transform.position;
        parent.transform.SetParent(transform.parent);

        foreach (Transform blocks in originBlocks)
        {
            blocks.SetParent(parent.transform);
        }

        yield return new WaitForSeconds(.5f);
        StartCoroutine(MoveFunc(parent.transform, new Vector3(missingPart.position.x, missingPart.position.y + 1, parent.transform.position.z), 20));
        yield return new WaitForSeconds(1);
        StartCoroutine(MoveFunc(parent.transform, new Vector3(missingPart.position.x, missingPart.position.y + 1, missingPart.position.z), (parent.transform.position - missingPart.position).magnitude * 2));
        yield return new WaitForSeconds(1);
        StartCoroutine(MoveFunc(parent.transform, new Vector3(missingPart.position.x, missingPart.position.y, missingPart.position.z), 20));

        yield return new WaitForSeconds(1);
        LevelManager.instance.Advance();
    }

    private IEnumerator MoveFunc(Transform obj, Vector3 newPos, float speed)
    {
        while (true)
        {
            if (!isMoving)
                obj.position = Vector3.MoveTowards(obj.position, newPos, speed * Time.deltaTime);

            if (obj.position == newPos)
            {
                isMoving = false;
                yield break;
            }

            yield return null;
        }
    }

    public Block BlockByKey(Vector3 key)
    {
        return blocks.ContainsKey(key) ? blocks[key] : null;
    }
}