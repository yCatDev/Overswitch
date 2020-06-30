using Overswitch;
using System;


namespace OverswitchConsoleTester
{
    
        
    internal class Program
    {
        public static void Main(string[] args)
        {
            var overswitch = new Core();
            var current = overswitch.GetThreadKeyboardLayout().LanguageName;
            var keyboardLayouts = overswitch.GetSystemKeyboardLayouts();
            foreach (var layout in keyboardLayouts)
            {
                Console.Write(layout.LanguageName);
                if (current == layout.LanguageName)
                    Console.Write(" <<");
                Console.Write(Environment.NewLine);
            }
        }
    }
}