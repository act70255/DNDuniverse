using DND.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.Interface
{
    public interface IWorldRuleService
    {
        Creature AddExperience(int sourceID, int value);
        Creature Spell(int skill, int name, int target);
    }
}
