using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Overswitch
{
    public class Core
    {
        #region P/INVOKE

        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(UInt32 idThread);

        [DllImport("user32.dll")]
        private static extern UInt32 GetKeyboardLayoutList(Int32 nBuff, IntPtr[] lpList);

        private const UInt32 KLF_SETFORPROCESS = 0x00000100;

        [DllImport("user32.dll")]
        private static extern UInt32 ActivateKeyboardLayout(IntPtr hkl, UInt32 flags);

        #endregion

        public class KeyboardLayout
        {
            public UInt32 Id { get; }

            public UInt16 LanguageId { get; }
            public UInt16 KeyboardId { get; }

            public String LanguageName { get; }
            public String KeyboardName { get; }

            internal KeyboardLayout(UInt32 id, UInt16 languageId, UInt16 keyboardId, String languageName,
                String keyboardName)
            {
                this.Id = id;
                this.LanguageId = languageId;
                this.KeyboardId = keyboardId;
                this.LanguageName = languageName;
                this.KeyboardName = keyboardName;
            }
        }

       
            public  KeyboardLayout GetThreadKeyboardLayout(Int32 threadId = 0)
            {
                var keyboardLayoutId = (UInt32) GetKeyboardLayout((UInt32) threadId);

                return CreateKeyboardLayout(keyboardLayoutId);
            }

            public  KeyboardLayout GetProcessKeyboardLayout(Int32 processId = 0)
            {
                var threadId = GetProcessMainThreadId(processId);
                return GetThreadKeyboardLayout(threadId);
            }

            public  KeyboardLayout[] GetSystemKeyboardLayouts()
            {
                var keyboardLayouts = new List<KeyboardLayout>();

                var count = GetKeyboardLayoutList(0, null);
                var keyboardLayoutIds = new IntPtr[count];
                GetKeyboardLayoutList(keyboardLayoutIds.Length, keyboardLayoutIds);

                foreach (var keyboardLayoutId in keyboardLayoutIds)
                {
                    var keyboardLayout = CreateKeyboardLayout((UInt32) keyboardLayoutId);
                    keyboardLayouts.Add(keyboardLayout);
                }

                return keyboardLayouts.ToArray();
            }

            private  Int32 GetProcessMainThreadId(Int32 processId = 0)
            {
                var process = 0 == processId ? Process.GetCurrentProcess() : Process.GetProcessById(processId);
                return process.Threads[0].Id;
            }

            private  KeyboardLayout CreateKeyboardLayout(UInt32 keyboardLayoutId)
            {
                var languageId = (UInt16) (keyboardLayoutId & 0xFFFF);
                var keyboardId = (UInt16) (keyboardLayoutId >> 16);

                return new KeyboardLayout(keyboardLayoutId, languageId, keyboardId, GetCultureInfoName(languageId),
                    GetCultureInfoName(keyboardId));

                string GetCultureInfoName(UInt16 cultureId)
                {
                    try
                    {
                        return new CultureInfo(cultureId).DisplayName;
                    }
                    catch
                    {
                        return new CultureInfo(1033).DisplayName;
                    }
                }
            }

        
    }
}