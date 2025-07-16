using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using OWML.Utils;
using System.Reflection;
using UnityEngine;

namespace Jam5Project;

public class Jam5Project : ModBehaviour
{
    public static Jam5Project Instance;
    public static INewHorizons NHAPI;
    public static ItemType ShrinkablePlanetType;

    private float _downloadStartTime;
    private bool _downloadingTranslation = false;
    private float _downloadLength;

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
        ModHelper.Console.WriteLine($"My mod {nameof(Jam5Project)} is loaded!", MessageType.Success);

        ShrinkablePlanetType = EnumUtils.Create<ItemType>("ShrinkablePlanet");

        NHAPI = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
        NHAPI.LoadConfigs(this);

        LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
        {
            if (loadScene == OWScene.SolarSystem)
            {
                PlayerData.SetPersistentCondition("URM_HAS_ERNESTONIAN_TRANSLATOR", false);
            }
        };
    }

    private void Update()
    {
        if (_downloadingTranslation && Time.time > _downloadStartTime + _downloadLength)
        {
            PlayerData.SetPersistentCondition("URM_HAS_ERNESTONIAN_TRANSLATOR", true);
            NotificationManager.SharedInstance.PostNotification(_downloadEndNotification, false);
            _downloadingTranslation = false;
        }
    }

    public void StartTranslationDownload()
    {
        _downloadStartTime = Time.time;
        NotificationManager.SharedInstance.PostNotification(_downloadStartNotification, false);
        _downloadingTranslation = true;
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
            __instance._textField.text = TranslationHandler.GetTranslation("<!> Untranslated Ernestonian writing <!>", TranslationHandler.TextType.UI);
        }
    }
}