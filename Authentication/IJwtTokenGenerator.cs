using TransactionsAPI.Models;

namespace TransactionsAPI.Authentication
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
