using OWML.Common;
using OWML.ModHelper;

namespace Jam5Project;

public class Jam5Project : ModBehaviour
{
    public static Jam5Project Instance;
    public static INewHorizons NHAPI;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ModHelper.Console.WriteLine($"My mod {nameof(Jam5Project)} is loaded!", MessageType.Success);

        NHAPI = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
        NHAPI.LoadConfigs(this);

        NHAPI.GetStarSystemLoadedEvent().AddListener(OnStarSystemLoaded);
    }

    private void OnStarSystemLoaded(string system)
    {
        if (system == "Jam5")
        {
            // We need gravity to put the moon in orbit, but we don't actually want gravity
            NHAPI.GetPlanet("PlanetSwitcherBase").transform.Find("GravityWell").gameObject.SetActive(false);
        }
    }

    public static void WriteDebugMessage(object msg)
    {
        Instance.ModHelper.Console.WriteLine(msg.ToString());
    }
}
