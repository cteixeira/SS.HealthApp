﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SS.HealthApp.Core.Resources {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SS.HealthApp.Core.Resources.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to Attached is the PDF file corresponding to the requested document.&lt;br/&gt;&lt;br/&gt;.
        /// </summary>
        internal static string Local_AccountSendDocumentBody {
            get {
                return ResourceManager.GetString("Local_AccountSendDocumentBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Digital Customer | Financial Document.
        /// </summary>
        internal static string Local_AccountSendDocumentSubject {
            get {
                return ResourceManager.GetString("Local_AccountSendDocumentSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DigitalCustomer@simplesolutions.pt.
        /// </summary>
        internal static string Local_EmailSender {
            get {
                return ResourceManager.GetString("Local_EmailSender", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Attached is the PDF file corresponding to the requested presence statement.&lt;br/&gt;&lt;br/&gt;.
        /// </summary>
        internal static string Local_PresenceDeclarationSendFileBody {
            get {
                return ResourceManager.GetString("Local_PresenceDeclarationSendFileBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Digital Customer | Declaration of Presence.
        /// </summary>
        internal static string Local_PresenceDeclarationSendFileSubject {
            get {
                return ResourceManager.GetString("Local_PresenceDeclarationSendFileSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Em anexo encontra-se o ficheiro PDF correspondente ao seu documento.&lt;br/&gt;&lt;br/&gt;.
        /// </summary>
        internal static string SAMS_AccountSendDocumentBody {
            get {
                return ResourceManager.GetString("SAMS_AccountSendDocumentBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SAMS | Documento Financeiro.
        /// </summary>
        internal static string SAMS_AccountSendDocumentSubject {
            get {
                return ResourceManager.GetString("SAMS_AccountSendDocumentSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SAMSinfo@sams.sbsi.pt.
        /// </summary>
        internal static string SAMS_EmailSender {
            get {
                return ResourceManager.GetString("SAMS_EmailSender", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exmo(a). Sr(a).&lt;br/&gt;&lt;br/&gt;Desde já agradecemos a sua preferência pelos nossos serviços.&lt;br/&gt;&lt;br/&gt;Em anexo encontra-se o ficheiro PDF correspondente à sua declaração.&lt;br/&gt;&lt;br/&gt;Qualquer questão contacte-nos 217 499 999.&lt;br/&gt;&lt;br/&gt;&lt;img src=&quot;cid:LogoSAMS&quot;&gt;.
        /// </summary>
        internal static string SAMS_PresenceDeclarationSendFileBody {
            get {
                return ResourceManager.GetString("SAMS_PresenceDeclarationSendFileBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SAMS | Declaração de Presença.
        /// </summary>
        internal static string SAMS_PresenceDeclarationSendFileSubject {
            get {
                return ResourceManager.GetString("SAMS_PresenceDeclarationSendFileSubject", resourceCulture);
            }
        }
    }
}