using System;

namespace Project1;

public sealed class IOC
{
    public static IOC Default { get; } = new();

    public void InitializeServices(IServiceProvider serviceProvider) =>
        iocProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    public T? GetService<T>() where T : class
    {
        if (iocProvider == null)
            throw new Exception("Service provider is not initialized.");

        return (T?)iocProvider.GetService(typeof(T));
    }

    private IServiceProvider iocProvider;
}
