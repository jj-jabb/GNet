using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

            WMI.Current.EventSystemForeground += new EventSystemForegroundHandler(Current_EventSystemForeground);
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

            switch (profile.Language)
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
                script.Stop();
        }

        public event EventHandler ScriptStarted;
        public event EventHandler ScriptStopped;

        void Current_EventSystemForeground(int processId, string processName, string filePath)
        {
            if (!IsRunForExeEnabled)
                return;

            List<Profile> profileList;
            if (profilesByExePath.TryGetValue(filePath, out profileList) && profileList.Count > 0 && profileList[0].IsEnabled)
            {
                profileList[0].ReadFile();
                SetProfile(profileList[0]);
                Start();
            }
        }

        public bool IsRunForExeEnabled { get; set; }

        public void LoadProfiles()
        {
            Profile profile;
            List<Profile> profileList;

            profiles = new List<Profile>();
            profilesByExePath = new Dictionary<string, List<Profile>>();
            profilesByName = new Dictionary<string, List<Profile>>();

            var profilePath = ".\\Profiles\\";
            foreach (var path in Directory.GetDirectories(profilePath))
                foreach (var file in Directory.GetFiles(path))
                    if (!Path.GetFileName(file).StartsWith("_"))
                    {
                        profiles.Add(profile = Profile.GetProfile(file));
                        profile.ReadHeader();

                        foreach (var exec in profile.Executables)
                            if (profilesByExePath.TryGetValue(exec, out profileList))
                                profileList.Add(profile);
                            else
                            {
                                profileList = new List<Profile>();
                                profileList.Add(profile);
                                profilesByExePath[exec] = profileList;
                            }

                        if (profilesByName.TryGetValue(profile.Name, out profileList))
                            profileList.Add(profile);
                        else
                        {
                            profileList = new List<Profile>();
                            profileList.Add(profile);
                            profilesByName[profile.Name] = profileList;
                        }
                    }

            System.Diagnostics.Debug.WriteLine("");
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
