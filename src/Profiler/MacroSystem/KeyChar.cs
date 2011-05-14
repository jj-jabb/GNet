using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace GNet.Profiler.MacroSystem
{
    [XmlInclude(typeof(KeyCharDown))]
    [XmlInclude(typeof(KeyCharUp))]
    [XmlInclude(typeof(KeyCharTap))]
    public abstract class KeyChar : StepActionInput
    {
        public KeyChar() { }

        public KeyChar(char character)
        {
            Character = character;
        }

        public override StepActionType Type { get { return StepActionType.Action; } }

        public char Character { get; set; }

        protected abstract string KeyType { get; }

        public override string ToString()
        {
            return "KeyChar " + KeyType + ": " + Character;
        }
    }

    public class KeyCharDown : KeyChar
    {
        public KeyCharDown() { }
        public KeyCharDown(char character) : base(character) { }

        protected override string KeyType { get { return "Down"; } }
    }

    public class KeyCharUp : KeyChar
    {
        public KeyCharUp() { }
        public KeyCharUp(char character) : base(character) { }

        protected override string KeyType { get { return "Up"; } }
    }

    public class KeyCharTap : KeyChar
    {
        public KeyCharTap() { }
        public KeyCharTap(char character) : base(character) { }

        protected override string KeyType { get { return "Tap"; } }
    }
}
