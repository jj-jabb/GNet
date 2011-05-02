using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

using GNet;

namespace GNet.Scripting
{
    public class ProfileManager : IDisposable
    {
        static ProfileManager current;
        public static ProfileManager Current
        {
            get
            {
                if (current == null)
                    current = new ProfileManager();
                return current;
            }
        }

        public static string Basepath
        {
            get { return Path.GetDirectoryName(Application.ExecutablePath) + ".\\Profiles\\"; }
        }

        public static void DisposeCurrent()
        {
            current.Dispose();
            current = null;
        }

        bool disposed;

        IDeviceScript script;
        LuaScript luaScript;
        
        public List<Profile> profiles;
        public Dictionary<string, List<Profile>> profilesByExePath;
        public Dictionary<string, List<Profile>> profilesByName;

        ProfileManager()
        {
            LoadProfiles();

            luaScript = new LuaScript();
            luaScript.Started += new EventHandler(luaScript_Started);
            luaScript.Stopped += new EventHandler(luaScript_Stopped);
            luaScript.ScriptError += new G13Script.ScriptErrorHandler(luaScript_ScriptError);

            IsRunForExeEnabled = true;
            WMI.Current.EventSystemForeground += new EventSystemForegroundHandler(Current_EventSystemForeground);
        }

        public bool AutoRunning { get; private set; }
        public Profile AutoProfile { get; private set; }

        void luaScript_ScriptError(Exception ex)
        {
            Stop();
        }

        void luaScript_Started(object sender, EventArgs e)
        {
            if (ScriptStarted != null)
                ScriptStarted(this, EventArgs.Empty);
        }

        void luaScript_Stopped(object sender, EventArgs e)
        {
            if (ScriptStopped != null)
                ScriptStopped(this, EventArgs.Empty);
        }

        public ProfileManager SetProfile(Profile profile)
        {
            if (script != null)
            {
                if (script.Profile == profile)
                    return this;

                if (script.IsRunning)
                    script.Stop();
            }

            switch (profile.Header.Language)
            {
                case ScriptLanguage.Lua:
                    script = luaScript;
                    script.Profile = profile;
                    break;
            }

            return this;
        }

        public void Start()
        {
            if (script == null)
                return;

            if (script.IsRunning)
                script.Stop();

            script.Start();
        }

        public void Stop()
        {
            if (script != null && script.IsRunning)
            {
                script.Stop();
                AutoRunning = false;
                AutoProfile = null;
            }
        }

        public event EventHandler ScriptStarted;
        public event EventHandler ScriptStopped;

        void Current_EventSystemForeground(int processId, string processName, string filePath)
        {
            System.Diagnostics.Debug.WriteLine("Current_EventSystemForeground: " + filePath);
            if (!IsRunForExeEnabled)
                return;

            List<Profile> profileList;
            bool profileFound = false;

            if (profilesByExePath.TryGetValue(filePath, out profileList) && profileList.Count > 0 && profileList[0].Header.IsEnabled)
            {
                profileFound = true;

                if (profileList[0] != AutoProfile)
                {
                    profileList[0].Load();
                    SetProfile(profileList[0]);
                    AutoRunning = true;
                    AutoProfile = profileList[0];
                    Start();
                }
            }

            if (!profileFound)
                Stop();
        }

        public bool IsRunForExeEnabled { get; set; }

        public List<Profile> LoadProfiles()
        {
            Profile profile;
            List<Profile> profileList;

            profiles = new List<Profile>();
            profilesByExePath = new Dictionary<string, List<Profile>>();
            profilesByName = new Dictionary<string, List<Profile>>();

            var profilePath = ProfileManager.Basepath;

            foreach (var headerFile in Directory.GetFiles(profilePath))
                if (headerFile.EndsWith(".header"))
                {
                    profile = new Profile(headerFile);
                    profiles.Add(profile);

                    if (profile.Header.Executable != null)
                    {
                        if (profilesByExePath.TryGetValue(profile.Header.Executable, out profileList))
                            profileList.Add(profile);
                        else
                        {
                            profileList = new List<Profile>();
                            profileList.Add(profile);
                            profilesByExePath[profile.Header.Executable] = profileList;
                        }
                    }

                    if (profilesByName.TryGetValue(profile.Header.Name, out profileList))
                        profileList.Add(profile);
                    else
                    {
                        profileList = new List<Profile>();
                        profileList.Add(profile);
                        profilesByName[profile.Header.Name] = profileList;
                    }
                }

            return profiles;
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;
            
            Stop();

            WMI.Current.EventSystemForeground -= new EventSystemForegroundHandler(Current_EventSystemForeground);
            WMI.DisposeCurrent();
        }
    }
}
