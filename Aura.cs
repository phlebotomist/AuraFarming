using ProjectM;
using Unity.Entities;
using ProjectM.Network;
using Stunlock.Core;
using System.Collections.Generic;
using ProjectM.Shared;
using Bloodstone.API;

namespace AuraFarming;

class Aura
{
    //  T1 yellow +unholy particul 5 Kill:
    // .buff 1688799287 m 30
    // .buff 2144624015 m 30
    // .buff 514720473  m 30

    //  T2 ashes +red 10 kill:
    // .buff -106492795 m 30
    // .buff -748506838 m 30

    // T3 flame +trail 15 kill:
    // .buff 1670636401 m 30
    // .buff -1124645803 m 30


    // vfx only by default:
    public static PrefabGUID red_glow = new PrefabGUID(784366378);
    public static PrefabGUID vermintrat = new PrefabGUID(933825031);

    // t1:
    public static PrefabGUID Unholy_Vampire_Buff_Bane = new PrefabGUID(1688799287);
    public static PrefabGUID Buff_ChurchOfLight_Paladin_FinalStageBuff = new PrefabGUID(2144624015);
    // Buff_ChurchOfLight_Paladin_FinalStageBuff triggers on kill: .debuff 358972271
    public static PrefabGUID Buff_ChurchOfLight_Cleric_Intervene_Shield = new PrefabGUID(514720473);

    // t2:
    public static PrefabGUID Buff_Cultist_BloodFrenzy_Buff = new PrefabGUID(-106492795);

    public static PrefabGUID Buff_MountainBeast_DashRotationImpair = new PrefabGUID(-748506838);

    // t3:
    public static PrefabGUID AB_Manticore_Flame_Chaos_Burn_LongDebuff = new PrefabGUID(1670636401);
    public static PrefabGUID Buff_Militia_InkCrawler_TrailEffect = new PrefabGUID(-1124645803);

    // public static List<PrefabGUID> aurasT1 = [Unholy_Vampire_Buff_Bane, Buff_ChurchOfLight_Paladin_FinalStageBuff, Buff_ChurchOfLight_Cleric_Intervene_Shield];
    public static List<PrefabGUID> aurasT1 = [red_glow];
    public static List<PrefabGUID> aurasT2 = [vermintrat];
    public static List<PrefabGUID> aurasT3 = [AB_Manticore_Flame_Chaos_Burn_LongDebuff, Buff_Militia_InkCrawler_TrailEffect];
    public static List<PrefabGUID> allAuras = [.. aurasT1, .. aurasT2, .. aurasT3];

    private static void AddLifeTime(Entity buffEntity)
    {
        if (buffEntity.Has<LifeTime>())
        {
            var lifetime = buffEntity.Read<LifeTime>();
            lifetime.EndAction = LifeTimeEndAction.None;
            lifetime.Duration = -1;
            buffEntity.Write(lifetime);
        }
        else
        {
            buffEntity.Add<LifeTime>();
            buffEntity.Write(new LifeTime
            {
                EndAction = LifeTimeEndAction.None,
                Duration = -1,
            });
        }
    }

    public static void ApplyAura(Entity characterUserEntity, Entity userEntity, PrefabGUID buffGUID)
    {
        var debugEventSystem = VWorld.Server.GetExistingSystemManaged<DebugEventsSystem>();
        var em = VWorld.Server.EntityManager;

        if (BuffUtility.TryGetBuff(em, characterUserEntity, buffGUID, out Entity _))
        {
            return;
        }

        ApplyBuffDebugEvent buffEvent = new()
        {
            BuffPrefabGUID = buffGUID
        };
        FromCharacter fromCharacter = new()
        {
            User = userEntity,
            Character = characterUserEntity
        };

        debugEventSystem.ApplyBuff(fromCharacter, buffEvent);

        if (!BuffUtility.TryGetBuff(em, characterUserEntity, buffGUID, out Entity buffEntity))
        {
            Plugin.Logger.LogError($"Failed to get buff entity for {buffGUID} on {characterUserEntity}");
            return;
        }
        AddLifeTime(buffEntity);
    }
    public static void TryRemoveAllAuras(Entity playerCharacter)
    {
        foreach (var buffPrefab in allAuras)// Keep it stupid simple
        {
            if (BuffUtility.TryGetBuff(VWorld.Server.EntityManager, playerCharacter, buffPrefab, out var buffEntity))
            {
                DestroyUtility.Destroy(VWorld.Server.EntityManager, buffEntity, DestroyDebugReason.TryRemoveBuff);
            }
        }
    }

    public static void ApplyAuraSet(Entity characterUserEntity, Entity userEntity, List<PrefabGUID> auraList)
    {
        TryRemoveAllAuras(characterUserEntity);
        foreach (var buffPrefab in auraList)
        {
            ApplyAura(characterUserEntity, userEntity, buffPrefab);
        }
    }
}