﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DumBot.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class BotMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal BotMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DumBot.Resources.BotMessages", typeof(BotMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &amp;#128008; /{0} - random cat gif.
        /// </summary>
        internal static string CatGifCommandDescription {
            get {
                return ResourceManager.GetString("CatGifCommandDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sorry, I didn&apos;t find any cats. &amp;#128575;. Try again later..
        /// </summary>
        internal static string CatsNotFound {
            get {
                return ResourceManager.GetString("CatsNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to City not specified.
        /// </summary>
        internal static string CityNotSpecified {
            get {
                return ResourceManager.GetString("CityNotSpecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to I&apos;m a dumb bot, I can understand only commands.
        /// </summary>
        internal static string DumbBot {
            get {
                return ResourceManager.GetString("DumbBot", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Temperature: {0} °C.
        /// </summary>
        internal static string Forecast_Temperature {
            get {
                return ResourceManager.GetString("Forecast_Temperature", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Time: {0}.
        /// </summary>
        internal static string Forecast_Time {
            get {
                return ResourceManager.GetString("Forecast_Time", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Weather: {0}.
        /// </summary>
        internal static string Forecast_Weather {
            get {
                return ResourceManager.GetString("Forecast_Weather", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wind: {0} m/s.
        /// </summary>
        internal static string Forecast_Wind {
            get {
                return ResourceManager.GetString("Forecast_Wind", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hi.
        /// </summary>
        internal static string Greeting {
            get {
                return ResourceManager.GetString("Greeting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &amp;#128220; /{0} - commands list.
        /// </summary>
        internal static string HelpCommandDescription {
            get {
                return ResourceManager.GetString("HelpCommandDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to get the information, try again later.
        /// </summary>
        internal static string TryAgainLater {
            get {
                return ResourceManager.GetString("TryAgainLater", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown command.
        /// </summary>
        internal static string UnknownCommand {
            get {
                return ResourceManager.GetString("UnknownCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type &quot;/{0}&quot; for commands list.
        /// </summary>
        internal static string UseHelp {
            get {
                return ResourceManager.GetString("UseHelp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &amp;#9728; /{0} city_name - weather forecast for the next 24 hours.
        /// </summary>
        internal static string WeatherCommandDescription {
            get {
                return ResourceManager.GetString("WeatherCommandDescription", resourceCulture);
            }
        }
    }
}
