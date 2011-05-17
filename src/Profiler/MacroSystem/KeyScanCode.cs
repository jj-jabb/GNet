using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.Profiler.MacroSystem
{
    [XmlInclude(typeof(KeyScanCodeDown))]
    [XmlInclude(typeof(KeyScanCodeUp))]
    [XmlInclude(typeof(KeyScanCodeTap))]
    public abstract class KeyScanCode : StepActionInput
    {
        ScanCode scanCode;

        public KeyScanCode() { }

        public KeyScanCode(ScanCode scanCode)
        {
            ScanCode = scanCode;
        }

        public ScanCode ScanCode { get { return scanCode; } set { scanCode = value; SetInputs(scanCode); } }

        protected abstract void SetInputs(ScanCode scanCode);
        protected abstract string KeyType { get; }

        public override string ToString()
        {
            return "KeyChar " + KeyType + ": " + ScanCode;
        }
    }

    public class KeyScanCodeDown : KeyScanCode
    {
        public KeyScanCodeDown() { }
        public KeyScanCodeDown(ScanCode scanCode) : base(scanCode) { }

        protected override void SetInputs(ScanCode scanCode)
        {
            Inputs = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode) };
        }

        protected override string KeyType { get { return "Down"; } }
    }

    public class KeyScanCodeUp : KeyScanCode
    {
        public KeyScanCodeUp() { }
        public KeyScanCodeUp(ScanCode scanCode) : base(scanCode) { }

        protected override void SetInputs(ScanCode scanCode)
        {
            Inputs = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode, true) };
        }

        protected override string KeyType { get { return "Up"; } }
    }

    public class KeyScanCodeTap : KeyScanCode
    {
        public KeyScanCodeTap() { }
        public KeyScanCodeTap(ScanCode scanCode) : base(scanCode) { }

        protected override void SetInputs(ScanCode scanCode)
        {
            Inputs = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode), InputSimulator.KeyWrapper(scanCode, true) };
        }

        protected override string KeyType { get { return "Tap"; } }
    }
}
