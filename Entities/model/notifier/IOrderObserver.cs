using GreenPortal.model;

namespace GreenPortal.util;

public interface IOrderObserver
{
    void Update(InstallationOrder order);
}