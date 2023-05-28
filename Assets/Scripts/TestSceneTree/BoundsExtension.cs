using UnityEngine;

public static class BoundsExtension
{
    public static bool IsInViewport(this Bounds bound, Camera camera)
    {
        System.Func<Vector4, int> computeCode = (projectionPos) =>
        {
            int code = 0;
            if (projectionPos.x < -projectionPos.w) code |= 1;
            if (projectionPos.x > projectionPos.w) code |= 2;
            if (projectionPos.y < -projectionPos.w) code |= 4;
            if (projectionPos.y > projectionPos.w) code |= 8;
            if (projectionPos.z < -projectionPos.w) code |= 16;
            if (projectionPos.z > projectionPos.w) code |= 32;
            return code;
        };

        Vector4 worldPos = Vector4.one;
        int resultCode = 63;
        for (int i = -1; i <= 1; i += 2)
        {
            for (int j = -1; j <= 1; j += 2)
            {
                for (int k = -1; k <= 1; k += 2)
                {
                    worldPos.x = bound.center.x + i * bound.extents.x;
                    worldPos.y = bound.center.y + j * bound.extents.y;
                    worldPos.z = bound.center.z + k * bound.extents.z;

                    resultCode &= computeCode(camera.projectionMatrix * camera.worldToCameraMatrix * worldPos);
                }
            }
        }
        return resultCode == 0;
    }
}
