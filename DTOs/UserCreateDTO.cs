﻿namespace TransactionsAPI.DTOs
{
    public class UserCreateDTO
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
    }
}
