using Robust.Shared.Map;

namespace Cinka.Game.Location.Managers;

public interface ILocationManager
{
    public void Initialize();
    public MapId GetCurrentLocationId();
    public void LoadLocation(string prototype);
}