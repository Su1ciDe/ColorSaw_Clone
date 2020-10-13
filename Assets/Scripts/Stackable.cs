using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Stackable : MonoBehaviour
{
    private Vector3 shapeSize = new Vector3(.95f, 1, .95f);

    private Material originalMat;
    private Vector3 pivot;

    private void Awake()
    {
        SetupPieces();
    }

    private void SetupPieces()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        originalMat = meshRenderer.material;
        Vector3 size = meshRenderer.bounds.size;

        pivot = new Vector3((size.x - 1) / 2, 0, (size.z - 1) / 2);

        if (size.x > 1 || size.z > 1)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    CreatePiece(x, z);
                }
            }
        }

        gameObject.SetActive(false);
    }

    private void CreatePiece(int x, int z)
    {
        GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Cube);

        piece.transform.localScale = shapeSize;
        piece.transform.position = transform.position + new Vector3(Mathf.Round(shapeSize.x * x), 0, Mathf.Round(shapeSize.z * z)) - pivot;

        piece.transform.SetParent(transform.parent);
        piece.GetComponent<MeshRenderer>().material = originalMat;

        piece.AddComponent<Block>();

        piece.GetComponent<BoxCollider>().isTrigger = true;
    }
}