using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using GNet.PInvoke;
using GNet.Profiler.MacroSystem;

namespace GNet.Profiler
{
    public class InputAssignment
    {
        public G13Keys? Key { get; set; }
        public JoystickDirection? Joystick { get; set; }
        public double? MinJoystickAngle { get; set; }
        public double? MaxJoystickAngle { get; set; }

        //public ScanCode? ScanCode { get; set; }
        //public char? Character { get; set; }
        public string MacroName { get; set; }

        public bool ShouldSerializeKey() { return Key != null; }
        public bool ShouldSerializeJoystick() { return Joystick != null; }
        public bool ShouldSerializeMinJoystickAngle() { return MinJoystickAngle != null; }
        public bool ShouldSerializeMaxJoystickAngle() { return MaxJoystickAngle != null; }
        //public bool ShouldSerializeScanCode() { return ScanCode != null; }
        //public bool ShouldSerializeCharacter() { return Character != null; }
        public bool ShouldSerializeMacroName() { return MacroName != null; }
    }
}
