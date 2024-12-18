﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
namespace Arcam.Data.ClickHouse.DBTypes
{
    [Table("test_strategy_tp"), PrimaryKey("TestStrategyId", "Date")]
    public class TestStrategyTP
    {
        [Column("test_strategy_id"), ForeignKey("test_strategy_id")]
        public long TestStrategyId { get; set; }
        [NotMapped]
        public virtual TestStrategy TestStrategy { get; set; }
        [Column("is_open")]
        public bool IsOpen { get; set; }
        [Column("is_long")]
        public bool IsLong { get; set; }
        [Column("price")]
        public double Price { get; set; }
        [Column("vallet_percent")]
        public double ValletPercent { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }
    }
}
