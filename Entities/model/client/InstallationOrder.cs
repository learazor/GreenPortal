using System.ComponentModel.DataAnnotations;
using Entities.model.portal;
using GreenPortal.util;

namespace GreenPortal.model;

public class InstallationOrder
{
    [Key]
    public Guid Guid { get; set; } = Guid.NewGuid();
    public string CompanyCode { get; set; }
    public double InstallationCost { get; set; }
    public double TransportationCost { get; set; }
    public double TotalCost { get; set; }
    public int Time { get; set; }
    public double Output { get; set; }
    private OrderStatus _status { get; set; } = OrderStatus.OFFERED;
    public OrderStatus Status
    {
        get => _status;
        set
        {
            if (_status != value)
            {
                _status = value;
                NotifyObservers();
            }
        }
    }
    public string ClientEmail { get; set; }
    
    private readonly List<IOrderObserver> _observers = new();

    public void AddObserver(IOrderObserver observer)
    {
        _observers.Add(observer);
    }

    public void RemoveObserver(IOrderObserver observer)
    {
        _observers.Remove(observer);
    }

    private void NotifyObservers()
    {
        foreach (var observer in _observers)
        {
            observer.Update(this);
        }
    }

    public class Builder
    {
        private readonly InstallationOrder _order;

        public Builder()
        {
            _order = new InstallationOrder();
        }

        public Builder SetCompanyCode(string companyCode)
        {
            _order.CompanyCode = companyCode;
            return this;
        }

        public Builder SetInstallationCost(double installationCost)
        {
            _order.InstallationCost = installationCost;
            return this;
        }

        public Builder SetTransportationCost(double transportationCost)
        {
            _order.TransportationCost = transportationCost;
            return this;
        }

        public Builder SetTotalCost(double totalCost)
        {
            _order.TotalCost = totalCost;
            return this;
        }

        public Builder SetTime(int time)
        {
            _order.Time = time;
            return this;
        }

        public Builder SetOutput(double output)
        {
            _order.Output = output;
            return this;
        }

        public Builder SetStatus(OrderStatus status)
        {
            _order._status = status;
            return this;
        }

        public Builder SetClientEmail(string clientEmail)
        {
            _order.ClientEmail = clientEmail;
            return this;
        }

        public InstallationOrder Build()
        {
            return _order;
        }
    }
}