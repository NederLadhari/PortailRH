//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StagePFE.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Demandes
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> DateDebut { get; set; }
        public Nullable<System.DateTime> DateFin { get; set; }
        public string Motif { get; set; }
        public string ApprobationChef { get; set; }
        public string ApprobationGerant { get; set; }
        public Nullable<System.DateTime> DateDemande { get; set; }
        public Nullable<int> EmployeID { get; set; }
        public Nullable<int> TypeID { get; set; }
        public Nullable<int> NombreJours { get; set; }
        public string Etat { get; set; }
    
        public virtual Employe Employe { get; set; }
        public virtual Types Types { get; set; }
    }
}
