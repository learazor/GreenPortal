using GreenPortal.model;

namespace GreenPortal.util;

public class EmailNotifier : IOrderObserver
{
    public void Update(InstallationOrder order)
    {
        Console.WriteLine($"Email sent to {order.ClientEmail}: Your order status has been updated to {order.Status}");
        //TODO send email here
    }
}