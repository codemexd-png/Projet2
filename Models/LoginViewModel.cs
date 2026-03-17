using System.ComponentModel.DataAnnotations;

namespace ProjetAuth.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Le courriel est obligatoire.")]
    [EmailAddress(ErrorMessage = "Format de courriel invalide.")]
    [Display(Name = "Courriel")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mot de passe")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Se souvenir de moi")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

