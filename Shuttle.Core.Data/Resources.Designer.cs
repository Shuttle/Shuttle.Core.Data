﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Shuttle.Core.Data {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Shuttle.Core.Data.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to There is already a `DatabaseContextScope`.  Nested scopes are not supported..
        /// </summary>
        public static string AmbientScopeException {
            get {
                return ResourceManager.GetString("AmbientScopeException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [hidden].
        /// </summary>
        public static string ConnectionStringHiddenValue {
            get {
                return ResourceManager.GetString("ConnectionStringHiddenValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Coluld not find a connection string named &apos;{0}&apos;..
        /// </summary>
        public static string ConnectionStringMissingException {
            get {
                return ResourceManager.GetString("ConnectionStringMissingException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connection string options named &apos;{0}&apos; has no connection string..
        /// </summary>
        public static string ConnectionStringOptionMissingException {
            get {
                return ResourceManager.GetString("ConnectionStringOptionMissingException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DatabaseContextFactoryOptions have not been configured.  Cannot call &apos;Create()&apos; directly..
        /// </summary>
        public static string DatabaseContextFactoryOptionsException {
            get {
                return ResourceManager.GetString("DatabaseContextFactoryOptionsException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Creating DatabaseContext for connection name &apos;{0}&apos; timed out after &apos;{1}&apos;..
        /// </summary>
        public static string DatabaseContextFactoryTimeoutException {
            get {
                return ResourceManager.GetString("DatabaseContextFactoryTimeoutException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Attempt to retrieve non-existent database context with key &apos;{0}&apos; for name &apos;{1}&apos;..
        /// </summary>
        public static string DatabaseContextKeyNotFoundException {
            get {
                return ResourceManager.GetString("DatabaseContextKeyNotFoundException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There is no active database context.  Use the database context factory to create a context..
        /// </summary>
        public static string DatabaseContextMissing {
            get {
                return ResourceManager.GetString("DatabaseContextMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Attempt to retrieve non-existent database context with name &apos;{0}&apos;..
        /// </summary>
        public static string DatabaseContextNameNotFoundException {
            get {
                return ResourceManager.GetString("DatabaseContextNameNotFoundException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not create a connection from provider factory &apos;{0}&apos;..
        /// </summary>
        public static string DbProviderFactoryCreateConnectionException {
            get {
                return ResourceManager.GetString("DbProviderFactoryCreateConnectionException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There is no `DbType` mapping for system type `{0}`..
        /// </summary>
        public static string DbTypeMappingException {
            get {
                return ResourceManager.GetString("DbTypeMappingException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A database context with name &apos;{0}&apos; has already been created..
        /// </summary>
        public static string DuplicateDatabaseContextException {
            get {
                return ResourceManager.GetString("DuplicateDatabaseContextException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not retrieve the value for property `{0}` from `parameters`..
        /// </summary>
        public static string DynamicGetValueException {
            get {
                return ResourceManager.GetString("DynamicGetValueException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find embedded resource script for name &apos;{0}&apos; at path &apos;{1}&apos;..
        /// </summary>
        public static string EmbeddedScriptMissingException {
            get {
                return ResourceManager.GetString("EmbeddedScriptMissingException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connection settings named &apos;{0}&apos; has no provider name..
        /// </summary>
        public static string ProviderNameMissingException {
            get {
                return ResourceManager.GetString("ProviderNameMissingException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find a record for &apos;{0}&apos; with id &apos;{1}&apos;..
        /// </summary>
        public static string RecordNotFoundException {
            get {
                return ResourceManager.GetString("RecordNotFoundException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The IScriptProviderConfiguration instance returned null for the ResourceAssembly.  No file could be found which is why an embedded resource is required..
        /// </summary>
        public static string ResourceAssemblyMissingException {
            get {
                return ResourceManager.GetString("ResourceAssemblyMissingException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Directory &apos;{0}&apos; was searched (recursively) for script file &apos;{1}&apos;.  Exactly 1 file must exist in the directory structure but {2} were found..
        /// </summary>
        public static string ScriptCountException {
            get {
                return ResourceManager.GetString("ScriptCountException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Timeout may not be less than 0..
        /// </summary>
        public static string TimeoutException {
            get {
                return ResourceManager.GetString("TimeoutException", resourceCulture);
            }
        }
    }
}
