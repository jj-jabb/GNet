using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GNet.Scripting
{
    public class ProfileHeader
    {
        public string Headerpath { get; set; }
        public string Filepath { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DeviceType Device { get; set; }
        public bool Lock { get; set; }
        public string Executable { get; set; }
        public HookOptions KeyboardHook { get; set; }
        public HookOptions MouseHook { get; set; }
        public ScriptLanguage Language { get; set; }
        public bool IsEnabled { get; set; }

        public static ProfileHeader Load(string headerPath)
        {
            var path = Path.Combine(ProfileManager.Basepath, headerPath);
            if (File.Exists(path))
            {
                XmlSerializer xmlser = new XmlSerializer(typeof(ProfileHeader));
                using (var fs = File.OpenRead(path))
                {
                    var header = xmlser.Deserialize(fs) as ProfileHeader;
                    header.Headerpath = Path.GetFileName(path);
                    return header;
                }
            }

            throw new FileNotFoundException("Could not find header file " + path, path);
        }

        public static void Save(ProfileHeader header)
        {
            XmlSerializer xmlser = new XmlSerializer(typeof(ProfileHeader));
            var path = Path.Combine(ProfileManager.Basepath, header.Headerpath);
            using (var fs = File.CreateText(path))
            {
                xmlser.Serialize(fs, header);
            }
        }

        public void Save()
        {
            Save(this);
        }
    }

    public class Profile
    {
        ProfileHeader header;
        public ProfileHeader Header
        {
            get { return header; }
            set
            {
                if (value == null)
                    header = new ProfileHeader();
                else
                    header = value;
            }
        }
        public string Contents { get; set; }

        public Profile()
        {
            header = new ProfileHeader();
        }

        public Profile(string headerPath)
        {
            Header = ProfileHeader.Load(headerPath);
        }

        public Profile(ProfileHeader header)
        {
            Header = header;
        }

        public Profile Load()
        {
            if (Header.Filepath == null)
                return this;

            var path = ProfileManager.Basepath + header.Filepath;
            if (!File.Exists(path))
                return this;
                
            using (var fs = File.OpenText(path))
            {
                Contents = fs.ReadToEnd();
            }

            return this;
        }

        public Profile Save()
        {
            if (Header.Filepath == null)
                return this;

            var path = ProfileManager.Basepath + header.Filepath;

            using (var fs = File.CreateText(path))
            {
                fs.Write(Contents);
            }

            return this;
        }
    }
}
