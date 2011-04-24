using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using GNet.Hid;

namespace GNet.Scripting
{
    public class GenericProfile : Profile
    {
        public GenericProfile(string filepath) : base(filepath) { }

        public override ScriptLanguage Language
        {
            get { return ScriptLanguage.Undefined; }
        }

        protected override string LineStart
        {
            get { return "#"; }
        }
    }

    public class LuaProfile : Profile
    {
        public LuaProfile(string filepath) : base(filepath) { }

        public override ScriptLanguage Language
        {
            get { return ScriptLanguage.Lua; }
        }

        protected override string LineStart
        {
            get { return "--="; }
        }
    }

    public class BooProfile : Profile
    {
        public BooProfile(string filepath) : base(filepath) { }

        public override ScriptLanguage Language
        {
            get { return ScriptLanguage.Boo; }
        }

        protected override string LineStart
        {
            get { return "#="; }
        }
    }

    public abstract class Profile
    {
        public static Profile GetProfile(string filepath)
        {
            var extension = Path.GetExtension(filepath).ToLower();
            switch (extension)
            {
                case ".lua":
                    return new LuaProfile(filepath);

                case ".boo":
                    return new BooProfile(filepath);

                default:
                    return new GenericProfile(filepath);
            }
        }

        public string Filepath { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DeviceType Device { get; set; }
        public bool Lock { get; set; }
        public List<string> Executables { get; set; }
        public HookOptions KeyboardHook { get; set; }
        public HookOptions MouseHook { get; set; }

        public Dictionary<string, string> Configuration { get; private set; }

        public string Contents { get; set; }

        public Profile(string filepath)
        {
            Filepath = filepath;
            Executables = new List<string>();
            Configuration = new Dictionary<string, string>();
        }

        public abstract ScriptLanguage Language { get; }
        protected abstract string LineStart { get; }

        public void ParseHeader()
        {
            string line;
            string key;
            string value;

            if(LineStart == null)
                throw new ArgumentNullException("LineStart is not defined.");

            if (Filepath == null)
                throw new ArgumentNullException("Filepath cannot be null.");

            if (!File.Exists(Filepath))
                throw new FileNotFoundException("File '" + Filepath + "' does not exist.");

            using (var reader = File.OpenText(Filepath))
            {
                var lineStartLength = LineStart.Length;
                var configSplit = new char[] { ':' };
                string[] split;

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();

                    if (!line.StartsWith(LineStart))
                        break;

                    line = line.Substring(lineStartLength);
                    split = line.Split(configSplit, 2, StringSplitOptions.None);
                    if (split.Length == 2)
                    {
                        key = split[0].Trim();
                        value = split[1].Trim();
                        if (key.Length > 0 && value.Length > 0)
                            Configuration[key] = value;
                    }
                }

                Configuration["Filepath"] = Filepath;
            }

            foreach (var k in Configuration.Keys)
            {
                value = Configuration[k];

                switch (k)
                {
                    case "Name":
                        Name = value;
                        break;

                    case "Description":
                        Description = value;
                        break;

                    case "Device":
                        try
                        {
                            Device = (DeviceType)Enum.Parse(typeof(DeviceType), value);
                        }
                        catch
                        {
                            Device = DeviceType.G13;
                            Configuration[k] = "G13";
                        }
                        break;

                    case "Lock":
                        Lock = value.ToLower() == "true" ? true : false;
                        break;

                    case "Executables":
                        int start = 0;
                        int len;
                        bool onStart = true;
                        for (int i = 0; i < value.Length; i++)
                        {
                            if (value[i] == '"')
                            {
                                if (onStart)
                                {
                                    start = i;
                                    onStart = false;
                                }
                                else
                                {
                                    len = i - start - 1;
                                    if (len > 0)
                                        Executables.Add(value.Substring(start + 1, len));
                                    onStart = true;
                                }
                            }
                        }

                        break;

                    case "KeyboardHook":
                        try
                        {
                            KeyboardHook = (HookOptions)Enum.Parse(typeof(HookOptions), value);
                        }
                        catch
                        {
                            KeyboardHook = HookOptions.None;
                            Configuration[k] = "None";
                        }
                        break;

                    case "MouseHook":
                        try
                        {
                            MouseHook = (HookOptions)Enum.Parse(typeof(HookOptions), value);
                        }
                        catch
                        {
                            MouseHook = HookOptions.None;
                            Configuration[k] = "None";
                        }
                        break;
                }
            }
        }
    }
}
