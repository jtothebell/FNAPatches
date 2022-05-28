using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FezEngine.Tools;

[assembly: IgnoresAccessChecksTo("FezEngine")]
[assembly: IgnoresAccessChecksTo("Microsoft.Xna.Framework")]
[ModEntryPoint]
public static class FezPatches
{
    public static void Main()
    {
        Console.Out.WriteLine("Found FezPatches, running...");
        try
        {
            new Harmony("com.github.jtothebell.FezPatches").PatchAll(Assembly.GetExecutingAssembly());
        }
        catch (Exception e) 
        {
            Console.Out.WriteLine($"Failed: {e.ToString()}");
            throw e;
        }
    }

    [HarmonyPatch(typeof(FezEngine.Tools.SettingsManager), nameof(FezEngine.Tools.SettingsManager.InitializeCapabilities))]
    private class PatchInitializeCapabilities
    {
        static Exception Finalizer()
        {
            //supress exceptions
            return null;
        }

        private static void Postfix()
        {
            //set these values that didn't get set properly
            FezEngine.Tools.SettingsManager.Settings.HardwareInstancing = 
                FNA3D.FNA3D_SupportsHardwareInstancing(SettingsManager.DeviceManager.GraphicsDevice.GLDevice) > 0;
            FezEngine.Tools.SettingsManager.Settings.MultiSampleCount =
                SettingsManager.DeviceManager.GraphicsDevice.PresentationParameters.MultiSampleCount;
        }
    }
    
}

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class IgnoresAccessChecksToAttribute : Attribute
    {
        public IgnoresAccessChecksToAttribute(string assemblyName)
        {
            AssemblyName = assemblyName;
        }

        public string AssemblyName { get; }
    }
}