using System;
using Bloodstone.API;
using ProjectM;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;
using VampireCommandFramework;

namespace AuraFarming;
public static class D
{

    public static void logWarn(string msg)
    {
        Plugin.Logger.LogWarning(msg);
    }
    public static void logError(string msg)
    {
        Plugin.Logger.LogError(msg);
    }
    public static void logInfo(string msg)
    {
        Plugin.Logger.LogInfo(msg);
    }
    public static void P(string msg)
    {
        FixedString512Bytes FixedStringkillMessage = new FixedString512Bytes(msg);
        ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, ref FixedStringkillMessage);
    }

    public static void GetBuffInfo(ChatCommandContext ctx, int buffId)
    {
        var em = VWorld.Server.EntityManager;
        PrefabGUID buffPrefab = new PrefabGUID(buffId);
        if (BuffUtility.TryGetBuff(em, ctx.Event.SenderCharacterEntity, buffPrefab, out Entity buffEntity))
        {
            PrintBuffDetails(buffEntity);
            ctx.Reply($"character has {buffId}, detetails printed in the log");
            return;
        }
        ctx.Reply($"character does not have {buffId} right now");
        return;
    }

    public static void GiveBuffAndGetBuffInfo(ChatCommandContext ctx, int buffId)
    {
        var newBuff = new PrefabGUID(buffId);
        Entity character = ctx.Event.SenderCharacterEntity;
        Entity user = ctx.Event.SenderUserEntity;
        var b = SetAndReturnBuff(character, user, newBuff);
        if (b == null)
        {
            ctx.Reply("Failed to get buff entity for red speed buff");
            return;
        }
        else
        {
            ctx.Reply($"We found the buff and set it now logging info {b.Value}");
            PrintBuffDetails(b.Value);
        }
        ctx.Reply("complete");
    }
    public static Entity? SetAndReturnBuff(Entity characterUserEntity, Entity userEntity, PrefabGUID buffGUID)
    {
        var debugEventSystem = VWorld.Server.GetExistingSystemManaged<DebugEventsSystem>();
        var em = VWorld.Server.EntityManager;
        if (BuffUtility.TryGetBuff(em, characterUserEntity, buffGUID, out Entity b)) // already have the buff
        {
            return b;
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

        if (BuffUtility.TryGetBuff(em, characterUserEntity, buffGUID, out Entity buffEntity))
        {
            // AdjustSpeed(buffEntity);
            return buffEntity;
        }
        Plugin.Logger.LogError($"Failed to get buff entity for {buffGUID} on {characterUserEntity}");
        return null;
    }

    public static void AdjustSpeed(Entity buffEntity)
    {
        if (buffEntity.Has<ModifyMovementSpeedBuff>())
        {
            var msModify = buffEntity.Read<ModifyMovementSpeedBuff>();
            msModify.MultiplyAdd = false; // Set to 50% speed
            msModify.MoveSpeed = 1;
            buffEntity.Write(msModify);
            P("Set MultiplyAdd to false ");
        }
    }

    public static unsafe void PrintBuffDetails(Entity buffEntity)
    {
        EntityManager em = VWorld.Server.EntityManager;
        var componentTypes = em.GetComponentTypes(buffEntity);
        try
        {
            Plugin.Logger.LogInfo($"=====================================================");
            Plugin.Logger.LogInfo($"===== START COMPONENT TYPES FOR : {buffEntity} ========");
            Plugin.Logger.LogInfo($"=====================================================");
            foreach (var type in componentTypes)
            {
                try
                {
                    Plugin.Logger.LogInfo($"Component: {type.ToString()}");
                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogWarning($"Could not read {type}: {ex.Message}");
                }
            }
            Plugin.Logger.LogInfo($"=====================================================");
            Plugin.Logger.LogInfo($"===== END COMPONENT TYPES FOR : {buffEntity} ========");
            Plugin.Logger.LogInfo($"=====================================================");
        }
        catch (Exception ex)
        {
            Helpers.P($"Failed to get component types for buff entity: {ex.Message}");
            Plugin.Logger.LogError($"Failed to get component types for buff entity: {ex.Message}");
            return;
        }
        finally
        {
            componentTypes.Dispose();
        }
    }


}