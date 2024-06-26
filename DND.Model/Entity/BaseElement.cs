﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Model.Entity
{
    public enum ActionType
    {
        Attack,
        Heal,
    }
    public enum ElementType
    {
        Physical,
        Fire,
        Water,
        Earth,
        Wind,
        Light,
        Dark,
    }

    public enum EffectType
    {
        Damage,
        Heal,
        Buff,
        Debuff,
    }

    public class BaseElement : BaseEntity
    {
        public virtual string Name { get; set; }
    }

    public class StatusValue : BaseElement
    {
        public StatusValue(string name = "", int value = 0)
        {
            Name = name;
            Value = value;
        }

        public int Value { get; set; } = 0;
    }

    public class StatusVolume : StatusValue
    {
        public StatusVolume(string name = "", int value = 0, int maxValue = 10)
        {
            Name = name;
            Value = value;
            MaxValue = maxValue;
        }
        public int MaxValue { get; set; } = 0;
        public int Add(int value)
        {
            var result = Value + value - MaxValue;
            if (Value + value > MaxValue)
            {
                Value = MaxValue;
            }
            else
            {
                Value += value;
            }
            return result;
        }
        public int Sub(int value)
        {
            var result = Value - value;
            if (Value - value < 0)
            {
                Value = 0;
            }
            else
            {
                Value -= value;
            }
            return result;
        }
        public bool CheckSub(int value) => Value - value >= 0;
    }

    public class Position : BaseElement
    {
        public Position(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
    }
}
