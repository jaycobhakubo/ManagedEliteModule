// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.Reflection;
using System.Windows.Forms;
using GTI.Modules.Shared.Properties;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// The common AboutBox that should be used for all Windows Elite modules.
    /// </summary>
    public partial class AboutBox : GradientForm
    {
        #region Member Methods
        private string m_assemblyTitle;
        private string m_assemblyProduct;
        private string m_assemblyVersion;
        private string m_assemblyDescription;
        private bool m_isProductCenter = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the AboutBox class.
        /// </summary>
        public AboutBox()
        {
            InitializeComponent();

            //  Initialize the AboutBox to display the product information from the assembly information.
            //  Change assembly information settings for your application through either:
            //  - Project->Properties->Application->Assembly Information
            //  - AssemblyInfo.cs
            Text = string.Format(Resources.AboutBoxAbout, AssemblyTitle);
            m_productName.Text = AssemblyProduct;
            m_version.Text = string.Format(Resources.Version, AssemblyVersion);
            m_description.Text = AssemblyDescription;

           

        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Raises the form's Load event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Text = string.Format(Resources.AboutBoxAbout, AssemblyTitle);
            m_productName.Text = AssemblyProduct;
            m_version.Text = string.Format(Resources.Version, AssemblyVersion);
            m_description.Text = AssemblyDescription;

            if (m_isProductCenter == true)
            {
                //System.Drawing.Color defaultBackground = System.Drawing.ColorTranslator.FromHtml("#44658D");
                //this.BackColor = defaultBackground;
                //this.ForeColor = System.Drawing.Color.White;
                m_okButton.ForeColor = System.Drawing.Color.Black;
            }
        }
        #endregion

        #region Assembly Attribute Accessors

        public bool isProductCenter
        {
            get {return m_isProductCenter;}
            set { m_isProductCenter = value; }
        }


        /// <summary>
        /// Gets or sets the title of the assembly.
        /// </summary>
        public string AssemblyTitle
        {
            get
            {
                if(m_assemblyTitle != null)
                    return m_assemblyTitle;

                // Get all Title attributes on this assembly.
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);

                // If there is at least one Title attribute.
                if(attributes.Length > 0)
                {
                    // Select the first one.
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];

                    // If it is not an empty string, return it.
                    if(titleAttribute.Title != "")
                        return titleAttribute.Title;
                }

                // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name.
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
            set
            {
                m_assemblyTitle = value;
            }
        }

        /// <summary>
        /// Gets or sets the product of the assembly.
        /// </summary>
        public string AssemblyProduct
        {
            get
            {
                if(m_assemblyProduct != null)
                    return m_assemblyProduct;

                // Get all Product attributes on this assembly.
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);

                // If there aren't any Product attributes, return an empty string.
                if(attributes.Length == 0)
                    return string.Empty;

                // If there is a Product attribute, return its value.
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
            set
            {
                m_assemblyProduct = value;
            }
        }

        /// <summary>
        /// Gets or sets the version of the assembly.
        /// </summary>
        public string AssemblyVersion
        {
            get
            {
                if(m_assemblyVersion != null)
                    return m_assemblyVersion;

                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            set
            {
                m_assemblyVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets the description of the assembly.
        /// </summary>
        public string AssemblyDescription
        {
            get
            {
                if(m_assemblyDescription != null)
                    return m_assemblyDescription;

                // Get all Description attributes on this assembly.
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);

                // If there aren't any Description attributes, return an empty string.
                if(attributes.Length == 0)
                    return string.Empty;

                // If there is a Description attribute, return its value.
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
            set
            { 
                m_assemblyDescription = value; 
            }
        }
        #endregion
    }
}
