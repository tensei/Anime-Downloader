﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Anime_Downloader.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\dummy\\*exe")]
        public string TorrentClient {
            get {
                return ((string)(this["TorrentClient"]));
            }
            set {
                this["TorrentClient"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\dummy")]
        public string Torrent_Files {
            get {
                return ((string)(this["Torrent_Files"]));
            }
            set {
                this["Torrent_Files"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\dummy")]
        public string OngoingFolder {
            get {
                return ((string)(this["OngoingFolder"]));
            }
            set {
                this["OngoingFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int RefreshCounter {
            get {
                return ((int)(this["RefreshCounter"]));
            }
            set {
                this["RefreshCounter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int RefreshTimer {
            get {
                return ((int)(this["RefreshTimer"]));
            }
            set {
                this["RefreshTimer"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Status: Starting...")]
        public string StatusLabel {
            get {
                return ((string)(this["StatusLabel"]));
            }
            set {
                this["StatusLabel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60")]
        public int RefreshWaitTime {
            get {
                return ((int)(this["RefreshWaitTime"]));
            }
            set {
                this["RefreshWaitTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string Setting {
            get {
                return ((string)(this["Setting"]));
            }
            set {
                this["Setting"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string animeitems {
            get {
                return ((string)(this["animeitems"]));
            }
            set {
                this["animeitems"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\r\n        <?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n        <ArrayOfString xmlns:x" +
            "si=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001" +
            "/XMLSchema\">\r\n        <string>dummy</string>\r\n        </ArrayOfString>\r\n      ")]
        public global::System.Collections.Specialized.StringCollection Listbox {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["Listbox"]));
            }
            set {
                this["Listbox"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\r\n        <?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n        <ArrayOfAnyType xmlns:" +
            "xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/200" +
            "1/XMLSchema\" />\r\n      ")]
        public global::System.Collections.ArrayList ListboxItems {
            get {
                return ((global::System.Collections.ArrayList)(this["ListboxItems"]));
            }
            set {
                this["ListboxItems"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\r\n        <?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n        <ArrayOfString xmlns:x" +
            "si=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001" +
            "/XMLSchema\" />\r\n      ")]
        public global::System.Collections.Specialized.StringCollection Groups {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["Groups"]));
            }
            set {
                this["Groups"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Debug shit")]
        public string Debug {
            get {
                return ((string)(this["Debug"]));
            }
            set {
                this["Debug"] = value;
            }
        }
    }
}
