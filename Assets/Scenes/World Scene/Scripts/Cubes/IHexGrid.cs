namespace Assets.Scripts.Cubes
{
    public interface IHexGrid
    {
        bool IsValid(Hex hex);
        bool IsObstacle(Hex hex);
    }
}