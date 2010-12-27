// Tracing.cs
//
// Part of Cropper - used to aid in development only.  To use this
// class from within a plugin, build the plugin with the compile-time
// symbol "Trace" defined, and include this source file into the plugin dll.
//
// From the command line, like this:
//
//     msbuild /p:Platform=x86 /p:DefineConstants=Trace
//
// From within Visual Studio, right click on the solution, select Properties.
// In the Build tab, enter the name "Trace" there.
//
// If you do not define Trace, then this code does not compile into
// Cropper and will not run. This code should be used only during development
// and testing.
//


using System;

namespace CropperPlugins.Common
{
    public static class Tracing
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int pid);

        //[System.Diagnostics.Conditional("Trace")]
        private static void SetupDebugConsole()
        {
            if ( !AttachConsole(-1) )  // Attach to a parent process console
                AllocConsole(); // Alloc a new console

            _process= System.Diagnostics.Process.GetCurrentProcess();
            System.Console.WriteLine();
            _initialized= true;
        }

        [System.Diagnostics.Conditional("Trace")]
        public static void Trace(string format, params object[] args)
        {
            if (!_initialized)
            {
                SetupDebugConsole();
            }

            System.Console.Write("{0:D5} ", _process.Id);
            System.Console.WriteLine(format, args);
        }

        private static System.Diagnostics.Process _process;
        private static bool _initialized = false;
    }
}
