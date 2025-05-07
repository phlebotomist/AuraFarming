
using System;
using System.Runtime.InteropServices;
using Bloodstone.API;
using Il2CppInterop.Runtime;
using Unity.Entities;

#pragma warning disable CS8500
internal static class ECSExtensions
{

    public unsafe static void Write<T>(this Entity entity, T componentData) where T : struct
    {
        ComponentType componentType = new ComponentType(Il2CppType.Of<T>());
        byte[] array = StructureToByteArray(componentData);
        int size = Marshal.SizeOf<T>();
        fixed (byte* data = array)
        {
            VWorld.Server.EntityManager.SetComponentDataRaw(entity, componentType.TypeIndex, data, size);
        }
    }

    public static byte[] StructureToByteArray<T>(T structure) where T : struct
    {
        int num = Marshal.SizeOf(structure);
        byte[] array = new byte[num];
        IntPtr intPtr = Marshal.AllocHGlobal(num);
        Marshal.StructureToPtr(structure, intPtr, fDeleteOld: true);
        Marshal.Copy(intPtr, array, 0, num);
        Marshal.FreeHGlobal(intPtr);
        return array;
    }

    public static void Add<T>(this Entity entity)
    {
        ComponentType componentType = new ComponentType(Il2CppType.Of<T>());
        VWorld.Server.EntityManager.AddComponent(entity, componentType);
    }
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
