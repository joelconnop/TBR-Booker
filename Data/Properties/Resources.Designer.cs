﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TBRBooker.Model.Properties {
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
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TBRBooker.Model.Properties.Resources", typeof(Resources).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;html&gt;
        ///    &lt;body&gt;
        ///
        ///        &lt;h1&gt;[title]&lt;/h1&gt;
        ///
        ///        &lt;h3&gt;Customer Details&lt;/h3&gt;
        ///        &lt;table&gt;
        ///            &lt;tr&gt;&lt;td&gt;Name:&lt;/td&gt;&lt;td&gt;[name]&lt;/td&gt;&lt;/tr&gt;
        ///            &lt;tr&gt;&lt;td&gt;Phone:&lt;/td&gt;&lt;td&gt;[phone]&lt;/td&gt;&lt;/tr&gt;
        ///            &lt;tr&gt;&lt;td&gt;Email:&lt;/td&gt;&lt;td&gt;[email]&lt;/td&gt;&lt;/tr&gt;
        ///        &lt;/table&gt;
        ///        &lt;br&gt;
        ///
        ///        &lt;h3&gt;Service Details&lt;/h3&gt;
        ///        &lt;table&gt;
        ///            &lt;tr&gt;&lt;td&gt;Date &amp; Day:&lt;/td&gt;&lt;td&gt;[date]&lt;/td&gt;&lt;/tr&gt;
        ///            &lt;tr&gt;&lt;td&gt;Time of Service:&lt;/td&gt;&lt;td&gt;[time]&lt;/td&gt;&lt;/tr&gt;
        ///            &lt;tr&gt;&lt;td&gt;Address:&lt;/td&gt;&lt;td&gt;[addres [rest of string was truncated]&quot;;.
        /// </summary>
        public static string BookingForm {
            get {
                return ResourceManager.GetString("BookingForm", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;html&gt;
        ///    &lt;body&gt;
        ///
        ///        &lt;h1&gt;[name]&lt;/h1&gt;
        ///        &lt;h2&gt;[start] - [end]&lt;/h2&gt;
        ///		&lt;br&gt;
        ///
        ///		&lt;h3&gt;Sales&lt;/h3&gt;
        ///        &lt;table&gt;
        ///            &lt;tr&gt;&lt;td&gt;Total Sales:&lt;/td&gt;&lt;td&gt;[totalSales]&lt;/td&gt;&lt;/tr&gt;
        ///            &lt;tr&gt;&lt;td&gt;Collected/Banked:&lt;/td&gt;&lt;td&gt;[collected]&lt;/td&gt;&lt;/tr&gt;
        ///            &lt;tr&gt;&lt;td&gt;Outstanding:&lt;/td&gt;&lt;td&gt;[outstanding]&lt;/td&gt;&lt;/tr&gt;
        ///			&lt;tr&gt;&lt;td&gt;Overdue Payments:&lt;/td&gt;&lt;td&gt;[overdue]&lt;/td&gt;&lt;/tr&gt;
        ///			&lt;tr&gt;&lt;td&gt;Bad Debt:&lt;/td&gt;&lt;td&gt;[baddebt]&lt;/td&gt;&lt;/tr&gt;
        ///        &lt;/table&gt;
        ///        &lt;br&gt;
        ///
        ///		&lt;h3&gt;Payments&lt;/h3&gt;
        ///		 &lt;table&gt;
        ///            [rest of string was truncated]&quot;;.
        /// </summary>
        public static string ReportForm {
            get {
                return ResourceManager.GetString("ReportForm", resourceCulture);
            }
        }
    }
}
