using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetAuth.Data;
using ProjetAuth.Models;
using ProjetAuth.Services;

namespace ProjetAuth.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _db;

    public AccountController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Register(string? returnUrl = null)
    {
        return View(new RegisterViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var normalizedEmail = model.Email.Trim().ToLowerInvariant();
        var exists = await _db.AppUsers.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
        if (exists)
        {
            ModelState.AddModelError(nameof(RegisterViewModel.Email), "Ce courriel est déjà utilisé.");
            return View(model);
        }

        var (hashBase64, saltBase64) = PasswordHashing.Hash(model.Password);
        var user = new AppUser
        {
            Email = model.Email.Trim(),
            PasswordHash = hashBase64,
            PasswordSalt = saltBase64
        };

        _db.AppUsers.Add(user);
        await _db.SaveChangesAsync();

        await SignInAsync(user, isPersistent: false);

        var returnUrl = model.ReturnUrl;
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var normalizedEmail = model.Email.Trim().ToLowerInvariant();
        var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
        if (user is null || !PasswordHashing.Verify(model.Password, user.PasswordSalt, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Courriel ou mot de passe incorrect.");
            return View(model);
        }

        await SignInAsync(user, model.RememberMe);

        var returnUrl = model.ReturnUrl;
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInAsync(AppUser user, bool isPersistent)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = isPersistent }
        );
    }
}

