using UnityEngine;

static public class GridGenerator
{
    public static Vector3[,] GenerateGridPositions(int rows, int columns, float spacingX, float spacingZ, Transform origin = null)
    {
        Vector3[,] positions = new Vector3[rows, columns];
        Vector3 startPos = origin != null ? origin.position : Vector3.zero;

        float offsetX = (columns - 1) * spacingX * 0.5f;
        float offsetZ = (rows - 1) * spacingZ * 0.5f;

        for (int z = 0; z < rows; z++)
        {
            for (int x = 0; x < columns; x++)
            {
                positions[z, x] = startPos + new Vector3(x * spacingX - offsetX, 0, z * spacingZ - offsetZ);
            }
        }

        return positions;
    }
    public static Vector3 GetGridPosition(int index, int xCount, int zCount, float spacing, Transform origin = null)
    {
        int x = index % xCount;
        int z = index / xCount;
        Vector3 startPos = origin != null ? origin.position : Vector3.zero;
        return startPos + new Vector3(x * spacing, 0, z * spacing);
    }

}
