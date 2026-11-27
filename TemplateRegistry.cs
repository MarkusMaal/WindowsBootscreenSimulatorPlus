namespace WindowsBootscreenSimulatorPlus
{
    public class TemplateRegistry
    {
        public static List<BootscreenTemplate> Templates { get; set; } = [];

        public static void InitDefaultTemplates()
        {
            Templates.Clear();

            Templates.Add(new BootscreenTemplate(BootscreenTemplate.Simulator.WindowsNT, "Windows NT 4"));

            var newTemplate = new BootscreenTemplate(BootscreenTemplate.Simulator.Windows9x, "Windows 98");
            newTemplate.SetValue("Graphic", Properties.Resources.Windows_98_4_10_1998_Boot);
            newTemplate.SetValue("Slit", Properties.Resources.slit);
            Templates.Add(newTemplate);

            newTemplate = new BootscreenTemplate(BootscreenTemplate.Simulator.Windows9x, "Windows ME");
            newTemplate.SetValue("Graphic", Properties.Resources.Windows_ME_4_90_3000_Boot);
            newTemplate.SetValue("Slit", Properties.Resources.slit);
            Templates.Add(newTemplate);

            newTemplate = new BootscreenTemplate(BootscreenTemplate.Simulator.Windows2000, "Windows 2000");
            newTemplate.SetValue("Graphic", Properties.Resources.Windows_2000_Pro);
            newTemplate.SetValue("Slit", Properties.Resources.slit2k);
            newTemplate.SetValue("GraphicServer", Properties.Resources.Windows_2000_Server);
            newTemplate.SetValue("SlitServer", Properties.Resources.slit2kserver);
            Templates.Add(newTemplate);

            newTemplate = new BootscreenTemplate(BootscreenTemplate.Simulator.WindowsXPVista, "Windows XP");
            newTemplate.SetValue("Graphic", Properties.Resources.Windows_XP_SP2_Boot);
            newTemplate.SetValue("Throbber", Properties.Resources.xp_blue_throbber);
            Templates.Add(newTemplate);

            newTemplate = new BootscreenTemplate(BootscreenTemplate.Simulator.WindowsXPVista, "Windows Vista");
            newTemplate.SetValue("Graphic", Properties.Resources.WindowsVista_RTM_Boot);
            newTemplate.SetValue("Throbber", Properties.Resources.vista_golden_throbber);
            Templates.Add(newTemplate);

            Templates.Add(new BootscreenTemplate(BootscreenTemplate.Simulator.Windows7, "Windows 7"));

            Templates.Add(new BootscreenTemplate(BootscreenTemplate.Simulator.WindowsMetro, "Windows 8.x/10/early 11"));
        }

        public static string[] GetTemplates() => [.. Templates.Select(p => p.GetStringValue("FriendlyName"))];

        public static void Simulate(int idx) => Templates[idx].Show();
    }
}
