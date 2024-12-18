﻿using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
namespace Arcam.Data.DataBase.DBTypes
{
    [Table("platform")]
    public class Platform
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("url")]
        public string Url { get; set; }
        [Column("classname")]
        public string ClassName { get; set; }
    }
}
