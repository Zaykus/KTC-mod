using System;
using System.Collections.Generic;
using UnityEngine;

namespace KingdomEnhanced.Systems
{
    /// <summary>
    /// Handles Text-to-Speech via System.Speech (Windows SAPI) using Reflection.
    /// v3.1: Replaced SpeakCompleted event with polling (fixes IL2CPP event binding failure).
    /// </summary>
    public static class TTSManager
    {
        private static object _synthesizer;
        private static bool _initialized = false;
        
        // Reflection caches
        private static System.Reflection.MethodInfo _speakAsyncMethod;
        private static System.Reflection.MethodInfo _cancelAllMethod;
        private static System.Reflection.PropertyInfo _stateProperty;

        // v3.1: Polling-based Speech Queue (replaces broken SpeakCompleted event)
        private static readonly Queue<string> _speechQueue = new Queue<string>();
        private static bool _isSpeaking = false;
        private static float _speakStartTime = 0f;
        private const float SpeakTimeout = 8f; // Safety: max seconds per message

        // Message History
        private static readonly List<string> _messageLog = new List<string>();
        private static int _historyIndex = -1;
        private const int MaxHistory = 10;

        public static void Initialize()
        {
            if (_initialized) return;

            try
            {
                Type type = Type.GetType("System.Speech.Synthesis.SpeechSynthesizer, System.Speech, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                
                if (type == null)
                    type = Type.GetType("System.Speech.Synthesis.SpeechSynthesizer, System.Speech");

                if (type != null)
                {
                    _synthesizer = Activator.CreateInstance(type);
                    
                    // Cache methods
                    _speakAsyncMethod = type.GetMethod("SpeakAsync", new Type[] { typeof(string) });
                    _cancelAllMethod = type.GetMethod("SpeakAsyncCancelAll", Type.EmptyTypes);
                    
                    // v3.1: Cache State property for polling
                    _stateProperty = type.GetProperty("State");

                    // Set default output
                    var setOutputMethod = type.GetMethod("SetOutputToDefaultAudioDevice");
                    if (setOutputMethod != null) setOutputMethod.Invoke(_synthesizer, null);

                    _initialized = true;
                    Debug.Log("[TTSManager] System.Speech initialized with polling queue.");
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

        /// <summary>
        /// v3.1: Must be called every frame (from AccessibilityFeature.Update).
        /// Polls synthesizer state and drains the speech queue.
        /// </summary>
        public static void Update()
        {
            if (!_initialized || _synthesizer == null) return;
            if (!_isSpeaking) return;

            bool finished = false;

            // Check timeout first
            if (Time.time - _speakStartTime > SpeakTimeout)
            {
                finished = true;
            }
            else if (_stateProperty != null)
            {
                try
                {
                    // SynthesizerState enum: Ready=0, Speaking=1, Paused=2
                    var state = _stateProperty.GetValue(_synthesizer, null);
                    int stateInt = (int)state;
                    if (stateInt == 0) // Ready
                        finished = true;
                }
                catch { finished = true; }
            }
            else
            {
                // No State property — just use timeout
            }

            if (finished)
            {
                _isSpeaking = false;
                if (_speechQueue.Count > 0)
                {
                    string next = _speechQueue.Dequeue();
                    SpeakImmediate(next);
                }
            }
        }

        /// <summary>
        /// Speak text. If interrupt is true, cancel current speech and clear queue.
        /// If interrupt is false, queue the message to play after current speech finishes.
        /// </summary>
        public static void Speak(string text, bool interrupt = true)
        {
            if (string.IsNullOrEmpty(text) || !_initialized || _synthesizer == null) return;

            // Strip Unity Rich Text Tags
            string cleanText = System.Text.RegularExpressions.Regex.Replace(text, "<.*?>", string.Empty);
            
            // Add to message history
            AddToHistory(cleanText);

            if (interrupt)
            {
                // Clear queue and speak immediately
                _speechQueue.Clear();
                SpeakImmediate(cleanText);
            }
            else
            {
                // Queue mode: if not currently speaking, speak now; otherwise enqueue
                if (!_isSpeaking)
                {
                    SpeakImmediate(cleanText);
                }
                else
                {
                    _speechQueue.Enqueue(cleanText);
                }
            }
        }

        private static void SpeakImmediate(string cleanText)
        {
            try
            {
                if (_cancelAllMethod != null) 
                    _cancelAllMethod.Invoke(_synthesizer, null);
                
                if (_speakAsyncMethod != null)
                {
                    _speakAsyncMethod.Invoke(_synthesizer, new object[] { cleanText });
                    _isSpeaking = true;
                    _speakStartTime = Time.time;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[TTSManager] Speech error (Disabling TTS): {ex.Message}");
                _initialized = false;
                _synthesizer = null;
            }
        }

        #region Message History

        private static void AddToHistory(string text)
        {
            // Don't add duplicates of the most recent entry
            if (_messageLog.Count > 0 && _messageLog[_messageLog.Count - 1] == text)
                return;

            _messageLog.Add(text);
            
            // Cap at MaxHistory
            if (_messageLog.Count > MaxHistory)
                _messageLog.RemoveAt(0);
            
            // Reset history browsing position to end
            _historyIndex = _messageLog.Count;
        }

        /// <summary>
        /// F11: Repeat the last spoken message.
        /// </summary>
        public static void RepeatLast()
        {
            if (_messageLog.Count > 0)
            {
                string last = _messageLog[_messageLog.Count - 1];
                SpeakImmediate(last);
            }
        }

        /// <summary>
        /// Shift+F11: Cycle backwards through message history.
        /// </summary>
        public static void ReadPreviousMessage()
        {
            if (_messageLog.Count == 0) return;

            _historyIndex--;
            
            if (_historyIndex < 0)
            {
                _historyIndex = 0;
                SpeakImmediate("Beginning of history");
                return;
            }

            string msg = _messageLog[_historyIndex];
            SpeakImmediate($"{_historyIndex + 1} of {_messageLog.Count}: {msg}");
        }

        #endregion
    }
}
