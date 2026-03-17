using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using ProjetAuth.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ProjetAuth.Data;

//Contexte de la base de donnée 
// Identity utilise mon modèle AppUser pour add de nouvelle colonne 
public class ApplicationDbContext :IdentityDbContext<AppUser>
{


    //Constructeur permettant à Asp.net de connecter ma bdd automatiqument 
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

   

   
}
