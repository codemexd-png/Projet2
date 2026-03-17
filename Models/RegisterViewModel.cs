using System.ComponentModel.DataAnnotations;

namespace ProjetAuth.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Le courriel est obligatoire.")]
    [EmailAddress(ErrorMessage = "Format de courriel invalide.")]
    [Display(Name = "Courriel")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
    [StringLength(100, ErrorMessage = "Le mot de passe doit avoir au moins {2} caractères.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Mot de passe")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmation est obligatoire.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Les mots de passe ne correspondent pas.")]
    [Display(Name = "Confirmer le mot de passe")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}

