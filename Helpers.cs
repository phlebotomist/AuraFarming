using Bloodstone.API;
using ProjectM;
using Unity.Collections;

namespace AuraFarming;

public static class Helpers
{
    public static void LogMessage(string message)
    {
        Plugin.Logger.LogMessage(message);
    }

    public static void LogError(string message)
    {
        Plugin.Logger.LogError(message);
    }

    public static void LogWarning(string message)
    {
        Plugin.Logger.LogWarning(message);
    }
    public static void P(string message)
    {
        FixedString512Bytes fixedStr = message;
        ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, ref fixedStr);
    }
}