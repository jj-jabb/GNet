using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.Profiler.MacroSystem
{
    [XmlInclude(typeof(KeyCharDown))]
    [XmlInclude(typeof(KeyCharUp))]
    [XmlInclude(typeof(KeyCharTap))]
    public abstract class KeyChar : StepActionInput
    {
        char character;

        public KeyChar() { }

        public KeyChar(char character)
        {
            Character = character;
        }

        public char Character { get { return character; } set { character = value; SetInputs(character); } }

        protected abstract void SetInputs(char character);
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

        protected override void SetInputs(char character)
        {
            Inputs = new InputWrapper[] { InputSimulator.KeyWrapper(character) };
        }

        protected override string KeyType { get { return "Down"; } }
    }

    public class KeyCharUp : KeyChar
    {
        public KeyCharUp() { }
        public KeyCharUp(char character) : base(character) { }

        protected override void SetInputs(char character)
        {
            Inputs = new InputWrapper[] { InputSimulator.KeyWrapper(character, true) };
        }

        protected override string KeyType { get { return "Up"; } }
    }

    public class KeyCharTap : KeyChar
    {
        public KeyCharTap() { }
        public KeyCharTap(char character) : base(character) { }

        protected override void SetInputs(char character)
        {
            Inputs = new InputWrapper[] { InputSimulator.KeyWrapper(character), InputSimulator.KeyWrapper(character, true) };
        }

        protected override string KeyType { get { return "Tap"; } }
    }
}
