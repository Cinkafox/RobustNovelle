using Robust.Shared.Map;

namespace Content.Game.Location.Managers;

public interface ILocationManager
{
    public void Initialize();
    public MapId GetCurrentLocationId();
    public void LoadLocation(string prototype);
}