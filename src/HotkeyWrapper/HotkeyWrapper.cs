using AutoHotkey.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class HotkeyWrapper
{
    AutoHotkeyEngine bindModeThread;
    Dictionary<string, Profile> profileInstances = 
        new Dictionary<string, Profile>(StringComparer.OrdinalIgnoreCase);
    Dictionary<string, dynamic> hotkeyCallbacks =
        new Dictionary<string, dynamic>(StringComparer.OrdinalIgnoreCase);

    private string bindModeHotkey = null;
    private dynamic bindModeCallback = null;
    private string bindModeProfile = null;

    private const string defaultProfileName = "0";
    public HotkeyWrapper()
    {
        InitBindMode();
        //AddProfile(defaultProfileName);    // Default profile
        //bindModeThread.Eval(String.Format("bh.SetDetectionState(1)"));
    }

    public bool AddProfile(string profileName)
    {
        if (profileInstances.ContainsKey(profileName))
            return false;
        profileInstances.Add(profileName, new Profile(profileName, ProfileCallback));
        return true;
    }

    public bool BindHotkey(string hkName, dynamic callback, string profileName = defaultProfileName)
    {
        bindModeHotkey = hkName;
        bindModeCallback = callback;
        bindModeProfile = profileName;
        Console.WriteLine("Turning on hotkeys");
        bindModeThread.ExecRaw(String.Format("bh.SetDetectionState(1)"));
        return true;
    }

    public bool SetHotkey(string hkName, uint keyCode, dynamic callback, string profileName = defaultProfileName)
    {
        if (!profileInstances.ContainsKey(profileName))
            return false;
        hotkeyCallbacks[hkName] = callback;
        profileInstances[profileName].SetHotkey(hkName, keyCode);
        return true;
    }

    private void InitBindMode()
    {
        bindModeThread = new AutoHotkeyEngine();
        BindModeEvent bindModeEventDelegate = BindModeEventCallback;

        IntPtr eptr = Marshal.GetFunctionPointerForDelegate(bindModeEventDelegate);
        bindModeThread.LoadFile("BindModeThread.ahk");
        var tv = bindModeThread.GetVar("tv");

        bindModeThread.ExecRaw(String.Format("bh := new BindHandler({0})", eptr));
    }

    void BindModeEventCallback(uint evt, uint keyCode)
    {
        //bindModeThread.ExecRaw(String.Format("msgbox"));
        if (bindModeHotkey == null)
            return;

        if (evt == 0)
        {
            Console.WriteLine(String.Format("Bound to Hotkey {0}", keyCode));
            bindModeThread.Eval(String.Format("bh.SetDetectionState(0)"));
            SetHotkey(bindModeHotkey, keyCode, bindModeCallback, bindModeProfile);
            bindModeCallback = null;
            bindModeHotkey = null;
            bindModeProfile = null;
        }

        //Console.WriteLine("BindMode: " + evt + ", " + keyCode);
    }

    void ProfileCallback(string hkName, bool evt, uint keyCode)
    {
        hotkeyCallbacks[hkName](evt);
        //Console.WriteLine("Profile: " + evt + ", " + keyCode);
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate void BindModeEvent([MarshalAs(UnmanagedType.U4)]uint evt, [MarshalAs(UnmanagedType.U4)]uint keyCode);

    private class Profile
    {
        string profileName;
        AutoHotkeyEngine ahkThread = new AutoHotkeyEngine();
        dynamic profileCallback;

        public Profile(string name, Action<string, bool, uint> callback)
        {
            profileName = name;
            profileCallback = callback;
            ProfileEvent profileEventDelegate = profileEventCallback;

            IntPtr eptr = Marshal.GetFunctionPointerForDelegate(profileEventDelegate);
            ahkThread.LoadFile("ProfileThread.ahk");
            ahkThread.Eval(String.Format("ph := new ProfileHandler({0})", eptr));
        }

        // Used for eg reloading user settings on load of app
        public bool SetHotkey(string hkName, uint keyCode)
        {
            ahkThread.Eval(String.Format("ph.SetHotkey(\"{0}\", {1})", hkName, keyCode));
            return true;
        }

        // User requested change of hotkey
        public bool SelectHotkey()
        {

            return true;
        }

        private void profileEventCallback(string hkName, bool evt, uint keyCode)
        {
            //Console.WriteLine("Profile: " + evt + ", " + keyCode);
            profileCallback(hkName, evt, keyCode);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void ProfileEvent(
            [MarshalAs(UnmanagedType.LPWStr)]string hkName,
            [MarshalAs(UnmanagedType.Bool)]bool evt,
            [MarshalAs(UnmanagedType.U4)]uint keyCode);

    }
}
