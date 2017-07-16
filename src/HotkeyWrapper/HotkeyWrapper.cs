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
    AutoHotkeyEngine bindModeThread = new AutoHotkeyEngine();

    public HotkeyWrapper()
    {

        InitBindMode();
        bindModeThread.Eval(String.Format("bh.SetDetectionState(1)"));

        while (true)
            Thread.Sleep(100);
    }

    private void InitBindMode()
    {
        IntPtr eptr = Marshal.GetFunctionPointerForDelegate(bindModeEventDelegate);
        bindModeThread.LoadFile("BindModeThread.ahk");
        bindModeThread.Eval(String.Format("bh := new BindHandler({0})", eptr));
    }

    static void BindModeEventCallback(bool evt, uint keyCode)
    {
        Console.WriteLine(evt + ", " + keyCode);
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate void BindModeEvent([MarshalAs(UnmanagedType.Bool)]bool evt, [MarshalAs(UnmanagedType.U4)]uint keyCode);

    static BindModeEvent bindModeEventDelegate = BindModeEventCallback;
}
