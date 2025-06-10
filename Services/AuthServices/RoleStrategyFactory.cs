using Microsoft.AspNetCore.Mvc;

namespace PetCare.Services.AuthServices
{
    public class RoleStrategyFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public RoleStrategyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IRoleStrategy CreateStrategy(string roleName)
        {
            return roleName switch
            {
                "Administrador" => _serviceProvider.GetRequiredService<AdminStrategy>(),
                "Cuidador" => _serviceProvider.GetRequiredService<CuidadorStrategy>(),
                "Cliente" => _serviceProvider.GetRequiredService<ClienteStrategy>(),
                _ => throw new NotImplementedException($"No strategy implemented for role: {roleName}")
            };
        }
    }
}
