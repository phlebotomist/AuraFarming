using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using VampireCommandFramework;

namespace AuraFarming;

[BepInPlugin(AuraFarmingInfo.PLUGIN_GUID, AuraFarmingInfo.PLUGIN_NAME, AuraFarmingInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
[BepInDependency("gg.deca.Bloodstone")]
[Bloodstone.API.Reloadable]
public class Plugin : BasePlugin
{
    Harmony _harmony;
    public static ManualLogSource Logger;
    public override void Load()
    {
        Log.LogInfo($"Plugin {AuraFarmingInfo.PLUGIN_GUID} version {AuraFarmingInfo.PLUGIN_VERSION} is loaded!");

        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

        CommandRegistry.RegisterAll();

        Logger = Log;
    }

    public override bool Unload()
    {
        CommandRegistry.UnregisterAssembly();
        _harmony?.UnpatchSelf();
        return true;
    }
}
