using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    public float x = 18, z = 10;

    public Vector3 GetRandomPositionOnBoundary( int side, float XOffset = 0, float ZOffset = 0) // SIDE ======= 0 = up, 1 = right , 2 = down, 3 = left
    {
        Vector3 result = new Vector3();

        switch (side)
        {
            case 0:
                result = new Vector3(Random.Range(-x + 2, x - 2), 0, z + 3);
                break;
            case 1:
                result = new Vector3(x + 3, 0, Random.Range(-z + 2, z - 2));
                break;
            case 2:
                result = new Vector3(Random.Range(-x + 2, x - 2), 0, -z - 3);
                break;
            case 3:
                result = new Vector3(-x - 3, 0,  Random.Range(-z + 2, z - 2));
                break;
        }

        return result;
    }

    public Vector3 GetDirectionBySide(int side) // SIDE ======= 0 = up, 1 = right , 2 = down, 3 = left
    {
        Vector3 result = new Vector3();

        switch (side)
        {
            case 0:
                result = -Vector3.forward;
                break;
            case 1:
                result = -Vector3.right;
                break;
            case 2:
                result = Vector3.forward;
                break;
            case 3:
                result = Vector3.right;
                break;
        }

        return result;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(x, 0, z) * 2);
    }
}
