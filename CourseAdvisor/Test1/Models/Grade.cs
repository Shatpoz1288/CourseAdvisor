//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Test1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Grade
    {
        public int Term_ID { get; set; }
        public string Course_ID { get; set; }
        public string Student_email { get; set; }
        public string Grade1 { get; set; }
    
        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
        public virtual Term Term { get; set; }
    }
}