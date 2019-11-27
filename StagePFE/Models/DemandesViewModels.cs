using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StagePFE.Models
{

    public enum TypeConge
    {
        [Description("CongeAnnuel")]
        CongeAnnuel,
        [Description("Maternité")]
        Maternite,
        [Description("Maladie")]
        Maladie
    }

    public class CongesViewModels
    {


       
        [DataType(DataType.Date)]
        
        [Display(Name = "Date de début")]
        public DateTime? DateDebut { get; set; }

        
        [DataType(DataType.Date)]
        [Display(Name = "Date de fin")]
        public DateTime? DateFin { get; set; }

        [Required]
        [Display(Name = "Type")]
        public int Type { get; set; }

        [Required]
        [Display(Name = "Motif")]
        public string Motif { get; set; }

        [Display(Name = "Approbation Chef de projet")]
        public string ApprobationChef { get; set; }

        [Display(Name = "Approbation du Gérant")]
        public string ApprobationGerant { get; set; }

        [Display(Name = "Nom")]
        public string Nom { get; set; }

        [Display(Name = "Prenom")]
        public string Prenom { get; set; }

        [Display(Name = "Nombre de jours")]
        public int Nombre { get; set; }

        [Display(Name = "nom type")]
        public string TtpeNom { get; set; }

        [Display(Name = "idtype")]
        public int IDtype { get; set; }


        [Required]
        [Display(Name = "Date de début")]
        public string Departement { get; set; }


        [Required]
        [Display(Name = "Date de début")]
        public int IdMembre { get; set; }


        [Display(Name = "nom type")]
        public string IdChef { get; set; }

        [Display(Name = "id dep")]
        public int IdDep { get; set; }

        public virtual ICollection<Employe> TeamMate { get; set; }
        public virtual ICollection<Demandes> historique { get; set; }



        //TypeEnum

        [Display(Name = "The label for my dropdown list")]
        public virtual Nullable<TypeConge> NomType { get; set; }
        public virtual Nullable<int> TypeId
        {
            get { return (Nullable<int>)NomType; }
            set { NomType = (Nullable<TypeConge>)value; }
        }




        //ListeDemande

        [Display(Name ="Liste")]
        public IEnumerable<Demandes> Demande { get; set; }

        [Display(Name = "Liste")]
        public IEnumerable<Employe> Employe { get; set; }

        //ListeEquipe
        [Display(Name = "Listee")]
        public IEnumerable<Equipes> Team { get; set; }

        public IEnumerable<Types> TypeConge { get; set; }

    }
}