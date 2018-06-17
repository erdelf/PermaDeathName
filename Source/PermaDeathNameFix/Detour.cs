using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace PermaDeathNameFix
{
    /**
     * CCL Code.. which means, if you copy it, give credit to "1000101"
     */

    public static class Detour
    {

        private static readonly List<string> detoured = new List<string>();
        private static readonly List<string> destinations = new List<string>();

        public static unsafe bool TryDetourFromTo(MethodInfo source, MethodInfo destination)
        {
            // error out on null arguments
            if (source == null)
            {
                Log.Error(text: "Source MethodInfo is null");
                return false;
            }

            if (destination == null)
            {
                Log.Error(text: "Destination MethodInfo is null");
                return false;
            }

            // keep track of detours and spit out some messaging
            string sourceString = source.DeclaringType?.FullName + "." + source.Name + " @ 0x" + source.MethodHandle.GetFunctionPointer().ToString(format: "X" + (IntPtr.Size * 2).ToString());
            string destinationString = destination.DeclaringType?.FullName + "." + destination.Name + " @ 0x" + destination.MethodHandle.GetFunctionPointer().ToString(format: "X" + (IntPtr.Size * 2).ToString());


            if (detoured.Contains(item: sourceString))
            {
                Log.Warning(text: "Source method ('" + sourceString + "') is previously detoured to '" + destinations[index: detoured.IndexOf(item: sourceString)] + "'");
            }

            //Log.Message("Detouring '" + sourceString + "' to '" + destinationString + "'");


            detoured.Add(item: sourceString);
            destinations.Add(item: destinationString);

            if (IntPtr.Size == sizeof(long))
            {
                // 64-bit systems use 64-bit absolute address and jumps
                // 12 byte destructive

                // Get function pointers
                long sourceBase = source.MethodHandle.GetFunctionPointer().ToInt64();
                long destinationBase = destination.MethodHandle.GetFunctionPointer().ToInt64();

                // Native source address
                byte* pointerRawSource = (byte*)sourceBase;

                // Pointer to insert jump address into native code
                long* pointerRawAddress = (long*)(pointerRawSource + 0x02);

                // Insert 64-bit absolute jump into native code (address in rax)
                // mov rax, immediate64
                // jmp [rax]
                *(pointerRawSource + 0x00) = 0x48;
                *(pointerRawSource + 0x01) = 0xB8;
                *pointerRawAddress = destinationBase; // ( Pointer_Raw_Source + 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 )
                *(pointerRawSource + 0x0A) = 0xFF;
                *(pointerRawSource + 0x0B) = 0xE0;

            }
            else
            {
                // 32-bit systems use 32-bit relative offset and jump
                // 5 byte destructive

                // Get function pointers
                int sourceBase = source.MethodHandle.GetFunctionPointer().ToInt32();
                int destinationBase = destination.MethodHandle.GetFunctionPointer().ToInt32();

                // Native source address
                byte* pointerRawSource = (byte*)sourceBase;

                // Pointer to insert jump address into native code
                int* pointerRawAddress = (int*)(pointerRawSource + 1);

                // Jump offset (less instruction size)
                int offset = (destinationBase - sourceBase) - 5;

                // Insert 32-bit relative jump into native code
                *pointerRawSource = 0xE9;
                *pointerRawAddress = offset;
            }

            // done!
            return true;
        }

    }
}