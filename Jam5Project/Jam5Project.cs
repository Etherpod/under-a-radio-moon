using OWML.Common;
using OWML.ModHelper;
using OWML.Utils;

namespace Jam5Project;

public class Jam5Project : ModBehaviour
{
    public static Jam5Project Instance;
    public static INewHorizons NHAPI;
    public static ItemType ShrinkablePlanetType;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ModHelper.Console.WriteLine($"My mod {nameof(Jam5Project)} is loaded!", MessageType.Success);

        ShrinkablePlanetType = EnumUtils.Create<ItemType>("ShrinkablePlanet");

        NHAPI = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
        NHAPI.LoadConfigs(this);
    }

    public static void WriteDebugMessage(object msg)
    {
        Instance.ModHelper.Console.WriteLine(msg.ToString());
    }
}
