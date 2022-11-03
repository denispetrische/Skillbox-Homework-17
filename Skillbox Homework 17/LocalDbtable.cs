using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Skillbox_Homework_17
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
