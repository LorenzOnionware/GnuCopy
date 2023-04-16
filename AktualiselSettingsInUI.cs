using Project1.Viewmodels;

namespace Project1;

public class AktualiselSettingsInUI
{
    public void AktualisereSetting()
    {
        MainWindow.savepath = IOC.Default.GetService<Settings>().Savelastpaths;
        var setting = IOC.Default.GetService<Settings>().Listingart;
            switch (setting)
            {
                case false:
                    MainWindow.Listing = false;
                    MainViewmodel.Default.Presetlistenable  = true;
                    break;
                case true:
                    MainWindow.Listing = true;
                    MainViewmodel.Default.Presetlistenable  = true;
                    break;
                default:
                    MainWindow.Listing = null;
                    MainViewmodel.Default.Presetlistenable = false;
                    break;
            }

    }
}