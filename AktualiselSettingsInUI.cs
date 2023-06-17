namespace Project1;

public class AktualiselSettingsInUI
{
    public void AktualisereSetting()
    {
        MainWindow.savepath = IOC.Default.GetService<Settings>().Savelastpaths;
    }
}