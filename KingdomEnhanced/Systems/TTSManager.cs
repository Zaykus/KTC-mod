using System;
using System.Collections.Generic;
using UnityEngine;

namespace KingdomEnhanced.Systems
{
    public static class TTSManager
    {
        private static object _synthesizer;
        private static bool _initialized = false;
        
        private static System.Reflection.MethodInfo _speakAsyncMethod;
        private static System.Reflection.MethodInfo _cancelAllMethod;
        private static System.Reflection.PropertyInfo _stateProperty;

        private static readonly Queue<string> _speechQueue = new Queue<string>();
        private static bool _isSpeaking = false;
        private static float _speakStartTime = 0f;
        private const float SpeakTimeout = 8f; 

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
                    
                    _speakAsyncMethod = type.GetMethod("SpeakAsync", new Type[] { typeof(string) });
                    _cancelAllMethod = type.GetMethod("SpeakAsyncCancelAll", Type.EmptyTypes);
                    
                    _stateProperty = type.GetProperty("State");

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

        public static void Update()
        {
            if (!_initialized || _synthesizer == null) return;
            if (!_isSpeaking) return;

            bool finished = false;

            if (Time.time - _speakStartTime > SpeakTimeout)
            {
                finished = true;
            }
            else if (_stateProperty != null)
            {
                try
                {
                    var state = _stateProperty.GetValue(_synthesizer, null);
                    int stateInt = (int)state;
                    if (stateInt == 0) 
                        finished = true;
                }
                catch { finished = true; }
            }
            else
            {
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

        public static void Speak(string text, bool interrupt = true)
        {
            if (string.IsNullOrEmpty(text) || !_initialized || _synthesizer == null) return;

            string cleanText = System.Text.RegularExpressions.Regex.Replace(text, "<.*?>", string.Empty);
            
            AddToHistory(cleanText);

            if (interrupt)
            {
                _speechQueue.Clear();
                SpeakImmediate(cleanText);
            }
            else
            {
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


        private static void AddToHistory(string text)
        {
            if (_messageLog.Count > 0 && _messageLog[_messageLog.Count - 1] == text)
                return;

            _messageLog.Add(text);
            
            if (_messageLog.Count > MaxHistory)
                _messageLog.RemoveAt(0);
            
            _historyIndex = _messageLog.Count;
        }

        public static void RepeatLast()
        {
            if (_messageLog.Count > 0)
            {
                string last = _messageLog[_messageLog.Count - 1];
                SpeakImmediate(last);
            }
        }

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

    }
}
