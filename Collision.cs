namespace projekt;

public static class Collision
{
    public static bool CheckAABBCollision(Transform a, Transform b)
    {
        return (a.BBoxMin.X <= b.BBoxMax.X && a.BBoxMax.X >= b.BBoxMin.X) &&
               (a.BBoxMin.Y <= b.BBoxMax.Y && a.BBoxMax.Y >= b.BBoxMin.Y) &&
               (a.BBoxMin.Z <= b.BBoxMax.Z && a.BBoxMax.Z >= b.BBoxMin.Z);

    }
}