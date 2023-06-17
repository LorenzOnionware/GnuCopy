using System;
using Avalonia.Controls;


namespace Project1
{
    sealed class WindowClosingService 
    {
        public event EventHandler Closed;

        public void Close()
        {
            App.MainWindow.Close();
        }

        public void RegisterWindow(Window window)
        {
            window.Closed += (o, e) => Closed?.Invoke(this, EventArgs.Empty);
        }
    }
}