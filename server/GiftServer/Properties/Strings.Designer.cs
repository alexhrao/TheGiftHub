﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GiftServer.Properties {
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
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("GiftServer.Properties.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to &lt;p&gt;&lt;strong&gt;Uh-Oh...&lt;/strong&gt; Looks like that code expired. &lt;a data-toggle=&quot;modal&quot; href=&quot;#resetPassword&quot;&gt;Reset your Password&lt;/a&gt;&lt;/p&gt;.
        /// </summary>
        internal static string codeExpired {
            get {
                return ResourceManager.GetString("codeExpired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;&lt;strong&gt;Uh-Oh...&lt;/strong&gt; Looks like you already made an account! Sign in instead&lt;/p&gt;.
        /// </summary>
        internal static string duplicateUser {
            get {
                return ResourceManager.GetString("duplicateUser", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Gift List.
        /// </summary>
        internal static string giftList {
            get {
                return ResourceManager.GetString("giftList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;&lt;strong&gt;Uh-Oh...&lt;/strong&gt; Looks like we didn&apos;t recognize that Username/Password pair. Try again or &lt;a data-toggle=&quot;modal&quot; href=&quot;#resetPassword&quot;&gt;Reset your Password&lt;/a&gt;&lt;/p&gt;.
        /// </summary>
        internal static string invalidCredentials {
            get {
                return ResourceManager.GetString("invalidCredentials", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;&lt;strong&gt;Password Reset&lt;/strong&gt; Please login below with your new password&lt;/p&gt;.
        /// </summary>
        internal static string newLogin {
            get {
                return ResourceManager.GetString("newLogin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;&lt;strong&gt;Recovery email sent&lt;/strong&gt; - check your inbox&lt;/p&gt;.
        /// </summary>
        internal static string recoveryEmailSent {
            get {
                return ResourceManager.GetString("recoveryEmailSent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;&lt;strong&gt;Success!&lt;/strong&gt; Please login below&lt;/p&gt;.
        /// </summary>
        internal static string signupSuccess {
            get {
                return ResourceManager.GetString("signupSuccess", resourceCulture);
            }
        }
    }
}
