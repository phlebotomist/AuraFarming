using HarmonyLib;
using ProjectM;
using Unity.Collections;
using Killfeed;
using Unity.Entities;
using Bloodstone.API;
using ProjectM.Network;

namespace AuraFarming;

public class VampireDownedHelpers
{
    public static (ulong, ulong)? GetKillerAndVictimIdFromDownedEntity(Entity downedEntity)
    {
        if (!VampireDownedServerEventSystem.TryFindRootOwner(downedEntity, 1, VWorld.Server.EntityManager, out var victimEntity))
            return null;

        if (!VampireDownedServerEventSystem.TryFindRootOwner(downedEntity.Read<VampireDownedBuff>().Source, 1, VWorld.Server.EntityManager, out var killerEntity))
            return null;

        if (killerEntity.Has<UnitLevel>())
            return null; //ignore if the killer is a unit

        if (!victimEntity.Has<PlayerCharacter>() || !killerEntity.Has<PlayerCharacter>())
            return null;

        var killerPlayerChar = killerEntity.Read<PlayerCharacter>();
        var victimPlayerChar = victimEntity.Read<PlayerCharacter>();

        if (killerPlayerChar.UserEntity == victimPlayerChar.UserEntity)
            return null; //ignore if the killer is the same as the victim (happens with silver)

        var killerId = killerPlayerChar.UserEntity.Read<User>().PlatformId;
        var victimId = victimPlayerChar.UserEntity.Read<User>().PlatformId;

        return (killerId, victimId);
    }
}

[HarmonyPatch(typeof(VampireDownedServerEventSystem), nameof(VampireDownedServerEventSystem.OnUpdate))]
[HarmonyAfter("gg.deca.Killfeed")]
public static class VampireDownedAfterKFHook
{
    public static void Prefix(VampireDownedServerEventSystem __instance)
    {
        var downedEvents = __instance.__query_1174204813_0.ToEntityArray(Allocator.Temp);
        foreach (var entity in downedEvents)
        {
            HandleDownedEntity(entity);
        }
        downedEvents.Dispose();
    }

    private static void HandleDownedEntity(Entity downedEnt)
    {
        var steamIds = VampireDownedHelpers.GetKillerAndVictimIdFromDownedEntity(downedEnt);
        if (steamIds == null)
            return;

        ulong killerId = steamIds.Value.Item1;
        ulong victimId = steamIds.Value.Item2;
        var killerData = DataStore.PlayerDatas[killerId];
        var victimData = DataStore.PlayerDatas[victimId];

        Helpers.P($"killer streak AFTER KF RAN: {killerData.CurrentStreak}, victim streak: {victimData.CurrentStreak}");
    }
}

// [HarmonyPatch(typeof(VampireDownedServerEventSystem), nameof(VampireDownedServerEventSystem.OnUpdate))]
// [HarmonyBefore("gg.deca.Killfeed")]
// public static class VampireDownedBeforeKFHook
// {
//     public static void Prefix(VampireDownedServerEventSystem __instance)
//     {
//         var downedEvents = __instance.__query_1174204813_0.ToEntityArray(Allocator.Temp);
//         foreach (var entity in downedEvents)
//         {
//             HandleDownedEntity(entity);
//         }
//         downedEvents.Dispose();
//     }

//     private static void HandleDownedEntity(Entity downedEnt)
//     {
//         var steamIds = VampireDownedHelpers.GetKillerAndVictimIdFromDownedEntity(downedEnt);
//         if (steamIds == null)
//             return;

//         ulong killerId = steamIds.Value.Item1;
//         ulong victimId = steamIds.Value.Item2;
//         var killerData = DataStore.PlayerDatas[killerId];
//         var victimData = DataStore.PlayerDatas[victimId];

//         Helpers.P($"killer streak Before KF RAN: {killerData.CurrentStreak}, victim streak: {victimData.CurrentStreak}");
//     }
// }