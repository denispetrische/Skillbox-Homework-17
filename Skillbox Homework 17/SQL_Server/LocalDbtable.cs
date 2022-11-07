using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Skillbox_Homework_17.SQL_Server
{
    public partial class LocalDbtable
    {
        [Key]
        public int Id { get; set; }
        public string Фамилия { get; set; }
        public string Имя { get; set; }
        public string Отчество { get; set; }
        public string НомерТелефона { get; set; }
        public string Email { get; set; }
    }
}
