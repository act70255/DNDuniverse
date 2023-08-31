using DND.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.Service.Interface
{
    public interface ICreatureService
    {
        Creature AddSkills(int creatureID, int[] skillIDs);
    }
}
