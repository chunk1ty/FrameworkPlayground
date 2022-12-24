using Microsoft.AspNetCore.Mvc;
using SamuraiApp.Data;
using SamuraiApp.Domain;

namespace SamuraiApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : Controller
{
    private const int SamuraiCount = 10000;

    private readonly SamuraiContext _samuraiContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HomeController> _logger;

    public HomeController(SamuraiContext samuraiContext, IServiceProvider serviceProvider, ILogger<HomeController> logger)
    {
        _samuraiContext = samuraiContext;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create()
    {
        for (int i = 0; i < SamuraiCount; i++)
        {
            var samurai = new Samurai() {Name = "Ank"};
            _samuraiContext.Samurais.Add(samurai);
            await _samuraiContext.SaveChangesAsync();
        }
      
        return Ok();
    }

    [HttpPost("Create1")]
    public async Task<IActionResult> Create1()
    {
        for (int i = 0; i < SamuraiCount; i++)
        {
            var samurai = new Samurai() { Name = "Ank" };
            _samuraiContext.Samurais.Add(samurai);
        }

        await _samuraiContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("Create2")]
    public async Task<IActionResult> Create2()
    {
        for (int i = 0; i < SamuraiCount; i++)
        {
            using var childScope = _serviceProvider.CreateScope();
            var samuraiContext = childScope.ServiceProvider.GetRequiredService<SamuraiContext>();
            // _logger.LogInformation($"SamuraiContext hashcode: [{samuraiContext.GetHashCode()}]");
            var samurai = new Samurai() { Name = "Ank" };
            samuraiContext.Samurais.Add(samurai);
            await samuraiContext.SaveChangesAsync();
        }


        return Ok();
    }
}