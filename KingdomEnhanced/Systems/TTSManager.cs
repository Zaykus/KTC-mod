using System;

using UnityEngine;
// using CrossSpeak; // Uncomment if you add the DLL reference

namespace KingdomEnhanced.Systems
{
    /// <summary>
    /// Handles Text-to-Speech via System.Speech (Windows SAPI) using Reflection.
    /// This prevents crashes if System.Speech.dll is missing on the user's system.
    /// </summary>
    public static class TTSManager
    {
        private static object _synthesizer; // Changed from SpeechSynthesizer to object
        private static bool _initialized = false;
        
        // Reflection MethodInfo caches to improve performance
        private static System.Reflection.MethodInfo _speakAsyncMethod;
        private static System.Reflection.MethodInfo _cancelAllMethod;

        public static void Initialize()
        {
            if (_initialized) return;

            try
            {
                // dynamic lookup to avoid hard dependency on System.Speech.dll
                Type type = Type.GetType("System.Speech.Synthesis.SpeechSynthesizer, System.Speech, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                
                if (type == null)
                    type = Type.GetType("System.Speech.Synthesis.SpeechSynthesizer, System.Speech");

                if (type != null)
                {
                    _synthesizer = Activator.CreateInstance(type);
                    
                    // Cache methods
                    _speakAsyncMethod = type.GetMethod("SpeakAsync", new Type[] { typeof(string) });
                    _cancelAllMethod = type.GetMethod("SpeakAsyncCancelAll", Type.EmptyTypes);

                    // Set default output (Reflection)
                    var setOutputMethod = type.GetMethod("SetOutputToDefaultAudioDevice");
                    if (setOutputMethod != null) setOutputMethod.Invoke(_synthesizer, null);
                    
                    _initialized = true;
                    Debug.Log("[TTSManager] System.Speech initialized successfully via Reflection.");
                }
                else 
                {
                    Debug.LogError("[TTSManager] System.Speech.dll not found. TTS Disabled.");
                    _initialized = false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TTSManager] Failed to initialize System.Speech: {ex.Message}");
                _initialized = false;
            }
        }

        public static void Speak(string text)
        {
            if (string.IsNullOrEmpty(text) || !_initialized || _synthesizer == null) return;

            // Strip Unity Rich Text Tags
            string cleanText = System.Text.RegularExpressions.Regex.Replace(text, "<.*?>", string.Empty);

            try
            {
                // Invoke via Reflection
                if (_cancelAllMethod != null) 
                    _cancelAllMethod.Invoke(_synthesizer, null);
                
                if (_speakAsyncMethod != null) 
                    _speakAsyncMethod.Invoke(_synthesizer, new object[] { cleanText });
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[TTSManager] Speech error (Disabling TTS): {ex.Message}");
                _initialized = false;
                _synthesizer = null;
            }
        }

        private static bool TrySpeakCrossSpeak(string text)
        {
            // If the user drops CrossSpeak.dll into the folder, they can uncomment or we can use reflection here.
            // For now, we return false to default to System.Speech which is built-in.
            // Converting this to reflection would be complex without knowing the exact API of CrossSpeak.
            return false;
        }
    }
}
