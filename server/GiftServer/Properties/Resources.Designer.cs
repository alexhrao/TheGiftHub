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
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("GiftServer.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to &lt;html lang=&quot;en&quot; xmlns=&quot;http://www.w3.org/1999/xhtml&quot;&gt;
        ///&lt;head&gt;
        ///    &lt;meta charset=&quot;utf-8&quot; /&gt;
        ///    &lt;title&gt;My Dashboard&lt;/title&gt;
        ///    &lt;style&gt;
        ///        body {
        ///            background-color: #CECECE;
        ///            color: black;
        ///            font-family: &apos;Segoe UI&apos;, Tahoma, Geneva, Verdana, sans-serif;
        ///        }
        ///    &lt;/style&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///    &lt;div class=&quot;container-fluid&quot;&gt;
        ///        &lt;div class=&quot;row&quot;&gt;
        ///            &lt;div class=&quot;col-md-3&quot;&gt;
        ///                &lt;!-- Feed --&gt;
        ///                &lt;div id=&quot;newPanel&quot; class=&quot;pa [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string dashboard {
            get {
                return ResourceManager.GetString("dashboard", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to GiftRegister.
        /// </summary>
        internal static string emailPassword {
            get {
                return ResourceManager.GetString("emailPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///
        ///&lt;html lang=&quot;en&quot; xmlns=&quot;http://www.w3.org/1999/xhtml&quot;&gt;
        ///&lt;head&gt;
        ///    &lt;meta charset=&quot;utf-8&quot; /&gt;
        ///    &lt;meta name=&quot;viewport&quot; content=&quot;width=device-width, initial-scale=1&quot; /&gt;
        ///    &lt;link rel=&quot;stylesheet&quot; href=&quot;https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css&quot;&gt;
        ///    &lt;script src=&quot;https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js&quot;&gt;&lt;/script&gt;
        ///    &lt;script src=&quot;https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js&quot;&gt;&lt;/script&gt;
        ///    &lt;script&gt;
        ///        $(doc [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string header {
            get {
                return ResourceManager.GetString("header", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;html lang=&quot;en&quot; xmlns=&quot;https://www.w3.org/1999/xhtml&quot;&gt;
        ///    &lt;head&gt;
        ///        &lt;title&gt;Gift Registry Login&lt;/title&gt;
        ///        &lt;script&gt;
        ///            $(document).ready(function () {
        ///                $(&apos;#signupSubmit&apos;).hide();
        ///                $(&apos;#secondPass&apos;).keyup(function () {
        ///                    if ($(&apos;#secondPass&apos;).val() === $(&apos;#firstPass&apos;).val() &amp;&amp; $(&apos;#secondPass&apos;).val()) {
        ///                        // We are good to go!
        ///                        $(&apos;#secondPass&apos;).parent().removeClass(&apos;has-error&apos;);
        ///             [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string login {
            get {
                return ResourceManager.GetString("login", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;html lang=&quot;en&quot; xmlns=&quot;http://www.w3.org/1999/xhtml&quot;&gt;
        ///&lt;head&gt;
        ///    &lt;meta charset=&quot;utf-8&quot; /&gt;
        ///    &lt;script&gt;
        ///        $(document).ready(function () {
        ///            $(&apos;.dropdown&apos;).on(&apos;mouseenter mouseleave click tap&apos;, function () {
        ///                $(this).toggleClass(&quot;open&quot;);
        ///            });
        ///            $(&apos;.dropdown-submenu&apos;).on(&apos;mouseenter mouseleave click tap&apos;, function () {
        ///                $(this).toggleClass(&quot;open&quot;);
        ///            });
        ///        });
        ///    &lt;/script&gt;
        ///    &lt;style&gt;
        ///        body {
        ///            f [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string navigationBar {
            get {
                return ResourceManager.GetString("navigationBar", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;html lang=&quot;en&quot; xmlns=&quot;http://www.w3.org/1999/xhtml&quot;&gt;
        ///&lt;head&gt;
        ///    &lt;meta charset=&quot;utf-8&quot; /&gt;
        ///    &lt;title&gt;Reset Password for Gift Registry&lt;/title&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///    &lt;div class=&quot;container-fluid&quot;&gt;
        ///        &lt;div class=&quot;text-center&quot;&gt;
        ///            &lt;form id=&quot;resetForm&quot; method=&quot;post&quot;&gt;
        ///                &lt;div class=&quot;input-group&quot;&gt;
        ///                    &lt;span class=&quot;input-group-addon&quot;&gt;&lt;i class=&quot;glyphicon glyphicon-lock&quot;&gt;&lt;/i&gt;&lt;/span&gt;
        ///                    &lt;input name=&quot;password&quot; type=&quot;password&quot; class=&quot;form-control&quot; id=&quot;fi [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string resetPassword {
            get {
                return ResourceManager.GetString("resetPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to http://localhost:60001.
        /// </summary>
        internal static string URL {
            get {
                return ResourceManager.GetString("URL", resourceCulture);
            }
        }
    }
}
