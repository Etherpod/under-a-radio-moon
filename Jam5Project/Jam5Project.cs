using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using OWML.Utils;
using System.Reflection;
using NewHorizons.Components;
using NewHorizons.External.Modules;
using NewHorizons.External.SerializableEnums;
using UnityEngine;
using NewHorizons.Utility.Files;
using UnityEngine.InputSystem;

namespace Jam5Project;

public class Jam5Project : ModBehaviour
{
    public static Jam5Project Instance;
    public static INewHorizons NHAPI;
    public static ItemType ShrinkablePlanetType;

    private float _downloadStartTime;
    private bool _downloadingTranslation = false;
    private float _downloadLength;

    private ShrinkerController _shrinkerController;

    private NotificationData _downloadStartNotification = new(NotificationTarget.Player, "TRANSLATOR: STARTING REMOTE DOWNLOAD", 5f, true);
    private NotificationData _downloadEndNotification = new(NotificationTarget.Player, "TRANSLATOR: COMPLETED DOWNLOAD", 5f, true);

    private void Awake()
    {
        Instance = this;
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        _downloadLength = new System.Random().Next(5, 15);
    }

    private void Start()
    {
        // ModHelper.Console.WriteLine($"My mod {nameof(Jam5Project)} is loaded!", MessageType.Success);

        ShrinkablePlanetType = EnumUtils.Create<ItemType>("ShrinkablePlanet");

        NHAPI = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
        NHAPI.LoadConfigs(this);

        LoadManager.OnStartSceneLoad += (scene, loadScene) =>
        {
            if (loadScene == OWScene.SolarSystem)
            {
                _shrinkerController = null;
            }
        };

        NHAPI.GetStarSystemLoadedEvent().AddListener(OnStarSystemLoaded);
    }

    private void OnStarSystemLoaded(string system)
    {
        if (system == "Jam5")
        {
            // Unwanted stuff
            var planet = NHAPI.GetPlanet("Radio Moon");
            planet.transform.Find("GravityWell").gameObject.SetActive(false);
            planet.transform.Find("Volumes").gameObject.SetActive(false);
            _shrinkerController = planet.GetComponentInChildren<ShrinkerController>();
        }
    }

    private void Update()
    {
        if (_downloadingTranslation && Time.time > _downloadStartTime + _downloadLength)
        {
            PlayerData.SetPersistentCondition("URM_HAS_ERNESTONIAN_TRANSLATOR", true);
            NotificationManager.SharedInstance.PostNotification(_downloadEndNotification, false);
            _downloadingTranslation = false;
        }
        
        /*if (Keyboard.current.numpadDivideKey.wasPressedThisFrame)
        {
            StartGameOver();
        }*/
    }

    public void StartTranslationDownload()
    {
        _downloadStartTime = Time.time;
        NotificationManager.SharedInstance.PostNotification(_downloadStartNotification, false);
        _downloadingTranslation = true;
    }

    public bool IsShrunken()
    {
        return _shrinkerController.IsPlayerShrunken();
    }

    public static void StartGameOver()
    {
        var gameOver = new GameOverModule
        {
            creditsType = NHCreditsType.Fast,
            text =
                "Sol never escaped, and was doomed to the same end as Mye.\n" +
                "The button didn't work. And now the hatchling joined their fate."
        };
        NHGameOverManager.Instance.StartGameOverSequence(gameOver, null, Instance);
    }

    public static void WriteDebugMessage(object msg)
    {
        msg ??= "null";
        Instance.ModHelper.Console.WriteLine(msg.ToString());
    }
}

[HarmonyPatch]
public static class Jam5ProjectPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(NomaiTranslatorProp), nameof(NomaiTranslatorProp.DisplayTextNode))]
    public static bool HideErnestonianText(NomaiTranslatorProp __instance)
    {
        bool flag = __instance._scanBeams[0]._nomaiTextLine != null && __instance._scanBeams[0]._nomaiTextLine
            .GetComponentInParent<NomaiText>() is ErnestonianText;

        if (flag && !PlayerData.GetPersistentCondition("URM_HAS_ERNESTONIAN_TRANSLATOR"))
        {
            if (!DialogueConditionManager.SharedInstance.GetConditionState("URM_STARTED_TRANSLATION_DOWNLOAD"))
            {
                Jam5Project.Instance.StartTranslationDownload();
                DialogueConditionManager.SharedInstance.SetConditionState("URM_STARTED_TRANSLATION_DOWNLOAD", true);
            }

            var dots = ".";
            if (Time.timeSinceLevelLoad % 3f > 1f)
            {
                dots += ".";
            }
            if (Time.timeSinceLevelLoad % 3f > 2f)
            {
                dots += ".";
            }
            __instance._textField.text = $"<!> Downloading translation files{dots} <!>";

            return false;
        }

        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(NomaiTranslatorProp), nameof(NomaiTranslatorProp.DisplayTextNode))]
    public static void ChangeErnestonianUnreadMessage(NomaiTranslatorProp __instance)
    {
        if (!PlayerData.GetPersistentCondition("URM_HAS_ERNESTONIAN_TRANSLATOR"))
        {
            return;
        }

        bool flag = __instance._scanBeams[0]._nomaiTextLine != null && __instance._scanBeams[0]._nomaiTextLine
            .GetComponentInParent<NomaiText>() is ErnestonianText;

        if (flag && __instance._translationTimeElapsed == 0f
            && !__instance._nomaiTextComponent.IsTranslated(__instance._currentTextID))
        {
            __instance._textField.text = TranslationHandler.GetTranslation("<!> Untranslated alien writing <!>", TranslationHandler.TextType.UI);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AssetBundleUtilities), nameof(AssetBundleUtilities.ReplaceShaders))]
    public static void FixEffectShader(GameObject prefab)
    {
        foreach (var ruleset in prefab.GetComponentsInChildren<EffectRuleset>(true))
        {
            var material = ruleset._cloudMaterial;
            if (material == null) continue;

            var replacementShader = Shader.Find(material.shader.name);
            if (replacementShader == null) continue;

            // preserve override tag and render queue (for Standard shader)
            // keywords and properties are already preserved
            if (material.renderQueue != material.shader.renderQueue)
            {
                var renderType = material.GetTag("RenderType", false);
                var renderQueue = material.renderQueue;
                material.shader = replacementShader;
                material.SetOverrideTag("RenderType", renderType);
                material.renderQueue = renderQueue;
            }
            else
            {
                material.shader = replacementShader;
            }
        }
    }

    /*[HarmonyPrefix]
    [HarmonyPatch(typeof(CanvasMarker), nameof(CanvasMarker.SetVisibility))]
    public static bool DisableHUDMarker(CanvasMarker __instance, bool value)
    {
        if (!Jam5Project.Instance.IsShrunken()) return true;

        bool tryEnable = value;
        bool isLogMarker = ShipLogEntryHUDMarker.s_entryLocation != null && __instance._visualTarget == ShipLogEntryHUDMarker.s_entryLocation.GetTransform();
        bool logMarkerOutsideCloak = !isLogMarker || !ShipLogEntryHUDMarker.s_entryLocation.IsWithinCloakField();
        bool playerInCloak = Locator.GetCloakFieldController() != null && Locator.GetCloakFieldController().isPlayerInsideCloak;
        bool logMarkerInCloak = isLogMarker && ShipLogEntryHUDMarker.s_entryLocation.IsWithinCloakField();

        if (tryEnable && (logMarkerOutsideCloak || (playerInCloak && logMarkerInCloak)))
        {
            return false;
        }
        return true;
    }*/
}