using System.Numerics;

namespace Content.Client.Utils;

public static class DirectionFlagHelper
{
    public static Angle ToAngle(this DirectionFlag dir)
    {
        if(dir.AsDirSafe() is not {} direction) return  
            Angle.Zero;
        
        return direction.ToAngle();
    }
    
    public static Direction? AsDirSafe(this DirectionFlag directionFlag)
    {
        switch (directionFlag)
        {
            case DirectionFlag.South:
                return Direction.South;
            case DirectionFlag.SouthEast:
                return Direction.SouthEast;
            case DirectionFlag.East:
                return Direction.East;
            case DirectionFlag.NorthEast:
                return Direction.NorthEast;
            case DirectionFlag.North:
                return Direction.North;
            case DirectionFlag.NorthWest:
                return Direction.NorthWest;
            case DirectionFlag.West:
                return Direction.West;
            case DirectionFlag.SouthWest:
                return Direction.SouthWest;
            default:
                return null;
        }
    }

   
    public static Vector2 ToVec(this DirectionFlag dir)
    {
        if(dir.AsDirSafe() is not {} direction) return  
            Vector2.Zero;
        return direction.ToVec();
    }
}