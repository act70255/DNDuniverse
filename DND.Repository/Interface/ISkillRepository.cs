using DND.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Repository.Interface
{
    public interface ISkillRepository
    {
        Skill Get(int id);
        IEnumerable<Skill> GetAll();
    }
}
