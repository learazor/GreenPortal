using EfcRepositories;
using Entities.model.portal;
using Entities.model.user;
using GreenPortal.model;
using GreenPortal.session;
using Microsoft.AspNetCore.Identity;

namespace GreenPortal.controllers;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("orders")]
public class OrderController : ControllerBase
{
    private readonly OrderRepository _orderRepository;
    private readonly CompanyRepository _companyRepository;
    private readonly UserManager<User> _userManager;
    
    public OrderController(OrderRepository orderRepository, UserManager<User> userManager, CompanyRepository companyRepository)
    {
        _orderRepository = orderRepository;
        _userManager = userManager;
        _companyRepository = companyRepository;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromQuery] Guid installationOfferId)
    {
        var user = await _userManager.GetUserAsync(User);
        var type = "Client";
        if (CheckUser(user, type, out var unauthorized)) return unauthorized;
        
        var offers = HttpContext.Session.GetObjectFromJson<List<InstallationOrder>>("Offers"); //use this to prepare order based on offer id
        var order = offers.FirstOrDefault(offer => offer.Guid == installationOfferId);
        if (order == null)
        {
            return NotFound("InstallationOffer not found.");
        }

        order.Status = OrderStatus.ORDERED;
        
        try
        {
            await _orderRepository.AddOrder(order);
            return Ok(new { Message = "Order created successfully.", OrderId = order.Guid });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpPost("accept-order")]
    public async Task<IActionResult> AcceptOrder([FromQuery] Guid installationOfferId)
    {
        return await ChangeStatus(installationOfferId, OrderStatus.ACCEPTED);
    }
    
    [HttpPost("cancel-order")]
    public async Task<IActionResult> CancelOrder([FromQuery] Guid installationOfferId)
    {
        return await ChangeStatus(installationOfferId, OrderStatus.CANCELED);
    }
    
    [HttpGet("company")]
    public async Task<IActionResult> GetCompanyOrders()
    {
        var user = await _userManager.GetUserAsync(User);
        if (CheckUser(user, "Company", out var unauthorized)) return unauthorized;

        if (user is CompanyUser companyUser)
        {
            var orders = await _orderRepository.GetOrdersByCompanyCodeAsync(companyUser.CompanyCode);
            return Ok(orders);
        }

        return Forbid("You are not authorized to view company orders.");
    }
    
    [HttpGet("client")]
    public async Task<IActionResult> GetClientOrders()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || user.AccountType != "Client")
        {
            return Unauthorized("You must be logged in as a Client to access this endpoint.");
        }

        try
        {
            var orders = await _orderRepository.GetOrdersByClientEmail(user.Email);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }


    private async Task<IActionResult> ChangeStatus(Guid installationOfferId, OrderStatus canceled)
    {
        var user = await _userManager.GetUserAsync(User);
        var type = "Company";
        if (CheckUser(user, type, out var unauthorized)) return unauthorized;
        if (CheckCompany(installationOfferId, user, out var acceptOrder)) return acceptOrder;
        
        await _orderRepository.UpdateOrderStatus(installationOfferId, canceled);

        return Ok(new
        {
            Message = "Order status updated successfully.",
            OrderId = installationOfferId,
            NewStatus = canceled
        });
    }

    private bool CheckUser(User? user, string type, out IActionResult unauthorized)
    {
        if (user == null || user.AccountType != type )
        {
            unauthorized = Unauthorized("You must be logged in to access this endpoint. This endpoint is restricted to "+type+" users only.");
            return true;
        }
        unauthorized = null;
        return false;
    }

    private bool CheckCompany(Guid installationOfferId, User? user, out IActionResult acceptOrder)
    {
        var order = _orderRepository.GetOrderByIdAsync(installationOfferId);
        if (user is not CompanyUser companyUser)
        {
            acceptOrder = Forbid("Only company users can perform this action.");
            return true;
        }
        
        if (order.Result?.CompanyCode != companyUser.CompanyCode)
        {
            acceptOrder = Forbid("You cannot accept an order that belongs to another company.");
            return true;
        }
        acceptOrder = null;
        return false;
    }
}
