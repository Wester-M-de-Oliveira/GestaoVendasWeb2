﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GestaoVendasWeb2.Models;

namespace GestaoVendasWeb2.Dtos
{
    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
