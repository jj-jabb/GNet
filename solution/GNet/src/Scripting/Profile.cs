using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using GNet.Hid;

namespace GNet.Scripting
{
    public class GenericProfile : Profile
    {
        public GenericProfile() { }
        public GenericProfile(string filepath) : base(filepath) { }

        public override ScriptLanguage Language
        {
            get { return ScriptLanguage.Undefined; }
        }

        protected override string HeaderLineStart
        {
            get { return "#"; }
        }
    }

    public class LuaProfile : Profile
    {
        public LuaProfile() { }
        public LuaProfile(string filepath) : base(filepath) { }

        public override ScriptLanguage Language
        {
            get { return ScriptLanguage.Lua; }
        }

        protected override string HeaderLineStart
        {
            get { return "--="; }
        }
    }

    public class BooProfile : Profile
    {
        public BooProfile() { }
        public BooProfile(string filepath) : base(filepath) { }

        public override ScriptLanguage Language
        {
            get { return ScriptLanguage.Boo; }
        }

        protected override string HeaderLineStart
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

        Dictionary<string, string> configuration;
        int headerLineCount;
        private string p;

        public Profile(string filepath)
            : this()
        {
            Filepath = filepath;
        }

        public Profile()
        {
            Executables = new List<string>();
            configuration = new Dictionary<string, string>();
        }

        public string Filepath { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DeviceType Device { get; set; }
        public bool Lock { get; set; }
        public List<string> Executables { get; set; }
        public HookOptions KeyboardHook { get; set; }
        public HookOptions MouseHook { get; set; }
        public bool IsEnabled { get; set; }

        public string Contents { get; set; }
        public int HeaderLineCount { get; set; }

        public abstract ScriptLanguage Language { get; }
        protected abstract string HeaderLineStart { get; }

        public void ReadFile()
        {
            ReadHeader();

            using (var reader = File.OpenText(Filepath))
            {
                Contents = reader.ReadToEnd();
            }
        }

        public int ReadHeader()
        {
            using (var reader = File.OpenText(Filepath))
            {
                return ReadHeader(reader);
            }
        }

        public int ReadHeader(StreamReader reader)
        {
            string line;
            string key;
            string value;

            headerLineCount = 0;

            #region validate inputs

            if (HeaderLineStart == null)
                throw new ArgumentNullException("LineStart is not defined.");

            if (Filepath == null)
                throw new ArgumentNullException("Filepath cannot be null.");

            if (!File.Exists(Filepath))
                throw new FileNotFoundException("File '" + Filepath + "' does not exist.");

            #endregion

            #region read header

            var lineStartLength = HeaderLineStart.Length;
            var configSplit = new char[] { ':' };
            string[] split;

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                headerLineCount++;

                if (!line.StartsWith(HeaderLineStart))
                    break;

                line = line.Substring(lineStartLength);
                split = line.Split(configSplit, 2, StringSplitOptions.None);
                if (split.Length == 2)
                {
                    key = split[0].Trim();
                    value = split[1].Trim();
                    if (key.Length > 0 && value.Length > 0)
                        configuration[key] = value;
                }
            }

            configuration["Filepath"] = Filepath;

            #endregion

            foreach (var k in configuration.Keys)
            {
                value = configuration[k];

                #region parse config values

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
                            configuration[k] = "G13";
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
                            configuration[k] = "None";
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
                            configuration[k] = "None";
                        }
                        break;

                    case "IsEnabled":
                        IsEnabled = value.ToLower() == "true" ? true : false;
                        break;

                }

                #endregion
            }

            return headerLineCount;
        }

        public void Save()
        {
            using (var fs = File.CreateText(Filepath))
            {
                WriteProperty(fs, "Name", Name);
                WriteProperty(fs, "Filepath", Filepath);
                WriteProperty(fs, "Description", Description);
                WriteProperty(fs, "Device", Device.ToString());
                WriteProperty(fs, "Lock", Lock.ToString());

                StringBuilder executables = new StringBuilder();
                foreach (var exec in Executables)
                    executables.Append("\"").Append(exec).Append("\" ");

                WriteProperty(fs, "Executables", executables.ToString());

                WriteProperty(fs, "KeyboardHook", KeyboardHook.ToString());
                WriteProperty(fs, "MouseHook", MouseHook.ToString());
                WriteProperty(fs, "IsEnabled", IsEnabled.ToString());

                string line;
                bool inHeader = true;
                using (var sr = new StringReader(Contents))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (inHeader)
                        {
                            if (!line.StartsWith(HeaderLineStart))
                                inHeader = false;
                        }

                        if (!inHeader)
                            fs.WriteLine(line);
                    }
                }

                //fs.WriteLine(Contents);
            }
        }

        void WriteProperty(StreamWriter fs, string name, string value)
        {
            fs.Write(HeaderLineStart);
            fs.Write(" ");
            fs.Write(name);
            fs.Write(": ");
            fs.WriteLine(value);
        }
    }
}
