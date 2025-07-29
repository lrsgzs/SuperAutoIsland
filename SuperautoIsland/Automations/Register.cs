using ClassIsland.Core;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Commands;
using ClassIsland.Core.Extensions.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SuperAutoIsland.Automations;

public class Register
{
    public static void Claim(IServiceCollection services)
    {
        services.AddHostedService<AutomationHandler>();
    }
}