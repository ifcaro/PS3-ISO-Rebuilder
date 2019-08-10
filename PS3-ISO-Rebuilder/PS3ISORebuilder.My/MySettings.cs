using Microsoft.VisualBasic.CompilerServices;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PS3ISORebuilder.My
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    [CompilerGenerated]
    internal sealed class MySettings : ApplicationSettingsBase
    {
        private static MySettings defaultInstance = (MySettings)SettingsBase.Synchronized(new MySettings());

        private static bool addedHandler;

        private static object addedHandlerLockObject = RuntimeHelpers.GetObjectValue(new object());

        public static MySettings Default
        {
            get
            {
                if (!addedHandler)
                {
                    object obj = addedHandlerLockObject;
                    ObjectFlowControl.CheckForSyncLockOnValueType(obj);
                    bool lockTaken = false;
                    try
                    {
                        Monitor.Enter(obj, ref lockTaken);
                        if (!addedHandler)
                        {
                            MyProject.Application.Shutdown += AutoSaveSettings;
                            addedHandler = true;
                        }
                    }
                    finally
                    {
                        if (lockTaken)
                        {
                            Monitor.Exit(obj);
                        }
                    }
                }
                return defaultInstance;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DebuggerNonUserCode]
        private static void AutoSaveSettings(object sender, EventArgs e)
        {
            if (MyProject.Application.SaveMySettingsOnExit)
            {
                MySettingsProperty.Settings.Save();
            }
        }
    }
}
