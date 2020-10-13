using UnityEngine;

public class Block : MonoBehaviour
{
    public bool origin;
    [HideInInspector]
    public bool connected;

    private Vector3 localPos;

    private MeshRenderer meshRenderer;
    [HideInInspector] public Color originalColor;

    private void Awake()
    {
        localPos = transform.localPosition;

        if (origin)
            connected = true;

        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color;
    }

    public void DestroyBlock(bool refresh = true)
    {
        GameManager.player.blocks.Remove(localPos);
        // particles
        ObjectPooler.instance.SpawnFromPool("BlockDestroyEffects", transform.position, Quaternion.identity, originalColor);

        if (origin)
        {
            GameManager.instance.StageFailed();
        }
        else
        {
            if (refresh)
            {
                GameManager.player.RefreshBlocks();
            }
            Destroy(gameObject);
        }
    }

    public void CheckConnection()
    {
        connected = true;

        var rightBlock = GameManager.player.BlockByKey(localPos + new Vector3(1, 0, 0));
        var leftBlock = GameManager.player.BlockByKey(localPos + new Vector3(-1, 0, 0));
        var topBlock = GameManager.player.BlockByKey(localPos + new Vector3(0, 0, 1));
        var bottomBlock = GameManager.player.BlockByKey(localPos + new Vector3(0, 0, -1));

        if (rightBlock && !rightBlock.connected)
        {
            rightBlock.CheckConnection();
        }
        if (leftBlock && !leftBlock.connected)
        {
            leftBlock.CheckConnection();
        }
        if (topBlock && !topBlock.connected)
        {
            topBlock.CheckConnection();
        }
        if (bottomBlock && !bottomBlock.connected)
        {
            bottomBlock.CheckConnection();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SawPath"))
        {
            Color color = meshRenderer.material.color;
            Color.RGBToHSV(color, out float H, out float S, out float V);
            S = Mathf.Clamp(S - .25f, 0, 1);
            color = Color.HSVToRGB(H, S, V);

            meshRenderer.material.color = color;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SawPath"))
        {
            meshRenderer.material.color = originalColor;
        }
    }
}