using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using GNet.Profiler.MacroSystem;

namespace GNet.Profiler
{
    public class Profile
    {
        // TODO:
        // implement AutoRelease
        [XmlIgnore]
        public string Description { get; set; }
        [XmlIgnore]
        public string Script { get; set; }

        public string Name { get; set; }

        public DeviceType Device { get; set; }
        public bool Lock { get; set; }
        public string Executable { get; set; }
        public HookOptions KeyboardHook { get; set; }
        public HookOptions MouseHook { get; set; }
        public ScriptLanguage Language { get; set; }
        public bool IsEnabled { get; set; }
        public List<Macro> Macros { get; set; }
        public List<InputAssignment> InputAssignments { get; set; }

        public bool ShouldSerializeLock() { return Lock; }
        public bool ShouldSerializeExecutable() { return Executable != null; }
        public bool ShouldSerializeKeyboardHook() { return KeyboardHook != HookOptions.None; }
        public bool ShouldSerializeMouseHook() { return MouseHook != HookOptions.None; }
        public bool ShouldSerializeIsEnabled() { return !IsEnabled; }
        public bool ShouldSerializeMacros() { return Macros != null && Macros.Count > 0; }
        public bool ShouldSerializeInputAssignments() { return InputAssignments != null && InputAssignments.Count > 0; }

        [XmlElement("DescriptionCData")]
        public XmlCDataSection DescriptionCData
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                return doc.CreateCDataSection(Description);
            }
            set
            {
                Description = value.Value;
            }
        }

        [XmlElement("ScriptCData")]
        public XmlCDataSection ScriptCData
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                return doc.CreateCDataSection(Script);
            }
            set
            {
                Script = value.Value;
            }
        }
    }
}
