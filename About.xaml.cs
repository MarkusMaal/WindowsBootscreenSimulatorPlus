using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;

namespace WindowsBootscreenSimulatorPlus
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {
        #region Assembly Attribute Accessors

        public static string AssemblyVersion
        {
            get
            {
                var assemblyVer = Assembly.GetExecutingAssembly().GetName().Version;
                var majorVer = assemblyVer?.Major;
                var minorVer = assemblyVer?.Minor;
                return string.Format("{0}.{1}{2}", majorVer, minorVer, (majorVer > 0 ? "" : (int.Parse(minorVer.ToString()?[0].ToString() ?? "0") < 2 ? " (pre-alpha)" : (int.Parse(minorVer.ToString()?[0].ToString() ?? "0") < 5 ? " (alpha)" : " (beta)")))) ?? "N/A";
            }
        }

        public static string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyTitleAttribute)attributes[0]).Title;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
        public About()
        {
            InitializeComponent();
        }
    }
}
