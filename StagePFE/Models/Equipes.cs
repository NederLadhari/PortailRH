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
    
    public partial class Equipes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Equipes()
        {
            this.Employe = new HashSet<Employe>();
        }
    
        public int Id { get; set; }
        public Nullable<int> IdChef { get; set; }
        public Nullable<int> IdMembre { get; set; }
        public string Departement { get; set; }
        public Nullable<int> IdDep { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employe> Employe { get; set; }
        public virtual Departements Departements { get; set; }
    }
}
