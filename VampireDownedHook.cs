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

    private static void HandleDownedEntity(Entity downedEntity)
    {
        var steamIds = VampireDownedHelpers.GetKillerAndVictimIdFromDownedEntity(downedEntity);
        if (steamIds == null)
            return;

        // this is duplicate 
        if (!VampireDownedServerEventSystem.TryFindRootOwner(downedEntity, 1, VWorld.Server.EntityManager, out var victimEntity))
            return;

        if (!VampireDownedServerEventSystem.TryFindRootOwner(downedEntity.Read<VampireDownedBuff>().Source, 1, VWorld.Server.EntityManager, out var killerEntity))
            return;
        var killerPlayerChar = killerEntity.Read<PlayerCharacter>();
        var victimPlayerChar = victimEntity.Read<PlayerCharacter>();
        var victimUser = victimPlayerChar.UserEntity.Read<User>();
        var killerUser = killerPlayerChar.UserEntity.Read<User>();
        // needs cleanup^^^^

        ulong killerId = steamIds.Value.Item1;
        ulong victimId = steamIds.Value.Item2;
        DataStore.PlayerDatas.TryGetValue(killerId, out var killerData);
        DataStore.PlayerDatas.TryGetValue(victimId, out var victimData);

        // Just keep it simple for now and always try to remove the victims aura incase he has one.
        Aura.TryRemoveAllAuras(victimUser.LocalCharacter._Entity);

        if (killerData.CurrentStreak == 5)
        {
            Helpers.P($"{killerUser.CharacterName} is on a: {killerData.CurrentStreak} and has the T1 Aura applied.");
            Aura.ApplyAuraSet(killerUser.LocalCharacter._Entity, killerPlayerChar.UserEntity, Aura.aurasT1);
        }
        else if (killerData.CurrentStreak == 10)
        {
            Helpers.P($"{killerUser.CharacterName} is on a: {killerData.CurrentStreak} and has the T2 Aura applied.");
            Aura.ApplyAuraSet(killerUser.LocalCharacter._Entity, killerPlayerChar.UserEntity, Aura.aurasT2);
        }
        else if (killerData.CurrentStreak == 15)
        {
            Helpers.P($"{killerUser.CharacterName} is on a: {killerData.CurrentStreak} and has the T3 Aura applied.");
            Aura.ApplyAuraSet(killerUser.LocalCharacter._Entity, killerPlayerChar.UserEntity, Aura.aurasT3);
        }
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