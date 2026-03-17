using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjetAuth.Data;
using ProjetAuth.Models;

//Ibrahima 
/*
 * Création du builder de l'app Asp.net 
 * Permet de configurer les services de l'app
 */
var builder = WebApplication.CreateBuilder(args);

//Connexion à la  Base de données

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));

//Gestion de l'authentification ASP.NET Identity

builder.Services.AddDefaultIdentity<AppUser>(options =>
{
    options.Password.RequireUppercase = true;//Oblige ue majuscule
    options.Password.RequiredLength = 8;//OBlige un minimum de 8 caractère
    options.Password.RequireDigit =true; //Oblige un chiffre au moin 

    
})
//Activation des rôles Admin & user 
.AddRoles<IdentityRole>()
//Les données seront stockées dans applicationDbContext
.AddEntityFrameworkStores<ApplicationDbContext>();

/*
 * Permet l'utilisation des contoler , page razor et vue 
 */

builder.Services.AddControllersWithViews();

//Construction de l'appli 
var myApp = builder.Build();

/*
    Permet de créer automatiquement  Les rôles et la gestion des users 
 */
using (var scope = myApp.Services.CreateScope())
{
    //Accès au services d'asp
    var services = scope.ServiceProvider;

    //Permet de gérer les rôles 
    var ManagerRole = services.GetRequiredService<RoleManager<IdentityRole>>();
    //Pemet de gérer les utilisateurs 
    var ManagerUser = services.GetRequiredService<UserManager<AppUser>>();


    //Définition de nos rôles 
    string[] Mesroles = { "Admin", "User" };

    foreach (var role in Mesroles)
    {
        //Verifie  le role existe dejà dans la bdd 
        if (!await ManagerRole.RoleExistsAsync(role))
        {
            //Si le rôle n'existe pas on le crée 
            await ManagerRole.CreateAsync(new IdentityRole(role));
        }
    }

    //Compte administrateur 
    var EmailAdm = "admin@tester.com";
    var PasswordAdm = "Admin12345!";

    //Vérifie si un user a déjà l'email
    var adminUser = await ManagerUser.FindByEmailAsync(PasswordAdm);

    //Si on n'a aucun user admin 
    if (adminUser == null)
    {
        //on crée un new admin 
        var user = new AppUser
        {
            UserName = EmailAdm,
            Email = EmailAdm,
            EmailConfirmed = true
        };


        //Création du suer dans la bdd et son password 

        var result = await ManagerUser.CreateAsync(user, PasswordAdm);


        // Création user réussi 
        if (result.Succeeded)
        {
            //ajout du user avec un role d'admin 
            await ManagerUser.AddToRoleAsync(user, "Admin");
        }
    }
}