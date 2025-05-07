
using System;
using Bloodstone.API;
using Il2CppInterop.Runtime;
using Unity.Entities;

#pragma warning disable CS8500
internal static class ECSExtensions
{
    internal static bool Has<T>(this Entity entity) where T : struct
    {
        return VWorld.Game.EntityManager.HasComponentRaw(entity, TypeManager.GetTypeIndex(Il2CppType.Of<T>()));
    }

    internal unsafe static T Read<T>(this Entity entity) where T : struct
    {

        T* rawData = (T*)VWorld.Game.EntityManager.GetComponentDataRawRO(entity, TypeManager.GetTypeIndex(Il2CppType.Of<T>()));
        if (rawData == null)
        {
            throw new InvalidOperationException($"failed to read <{typeof(T).Name}> from entity({entity}).");
        }
        return *rawData;
    }
}
