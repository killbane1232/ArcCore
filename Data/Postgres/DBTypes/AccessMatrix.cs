using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("access_matrix")]
    [PrimaryKey(nameof(ParamId), nameof(AccessTypeId))]
    public class AccessMatrix
    {
        [Column("param_id"), ForeignKey("param_id")]
        public long ParamId { get; set; }
        public virtual AccessParameter? Param { get; set; }
        [Column("access_type_id"), ForeignKey("access_type_id")]
        public long AccessTypeId { get; set; }
        public virtual AccessType? AccessType { get; set; }
    }
}
