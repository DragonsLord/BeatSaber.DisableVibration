using HarmonyLib;
using IPA;
using UnityEngine.XR;
using System.Reflection;
using IPALogger = IPA.Logging.Logger;

namespace BeatSaber.DisableVibration
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class DisableVibrationPlugin
    {
        internal static MethodInfo InputDevice_SendHapticImpulseMethod = AccessTools.Method(typeof(InputDevice), nameof(InputDevice.SendHapticImpulse));
        internal static MethodInfo InputDevice_SendHapticImpulseMethod_Patch = AccessTools.Method(typeof(DisableVibrationPlugin), nameof(SendHapticImpulsePrefix));

        private static Harmony _harmony = new Harmony(nameof(DisableVibrationPlugin));

        internal static DisableVibrationPlugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger)
        {
            Instance = this;
            Log = logger;
            Log.Info("BeatSaber.DisableVibration initialized.");
        }

        static bool SendHapticImpulsePrefix(ref bool __result)
        {
            __result = false;
            return false;
        }

        [OnEnable]
        public void OnPluginEnabled()
        {
            Log.Info("Disabling Controller Vibration ...");
            _harmony.Patch(InputDevice_SendHapticImpulseMethod, prefix: new HarmonyMethod(InputDevice_SendHapticImpulseMethod_Patch));
            Log.Info("Controller Vibration Disabled");
        }

        [OnDisable]
        public void OnPluginDisabled()
        {
            Log.Info("Enabling Controller Vibration");
            _harmony.Unpatch(InputDevice_SendHapticImpulseMethod, InputDevice_SendHapticImpulseMethod_Patch);
            Log.Info("Controller Vibration Enabled");
        }
    }
}
