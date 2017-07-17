using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HotkeyWrapperTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var hw = new HotkeyWrapper();
            //hw.AddProfile("Default");
            //hw.SetHotkey("hk1", 65, new Action<bool>((value) => 
            //{
            //    Console.WriteLine("Hotkey 1 Value: " + value);
            //}));

            //hw.SetHotkey("hk2", 66, new Action<bool>((value) => 
            //{
            //    Console.WriteLine("Hotkey 2 Value: " + value);
            //}));

            hw.BindHotkey("hk1", new Action<bool>((value) =>
            {
                Console.WriteLine("Hotkey 1 Value: " + value);
            }));


            while (true)
                Thread.Sleep(100);
        }
    }
}
