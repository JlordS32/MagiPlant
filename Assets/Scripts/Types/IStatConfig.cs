using System;

public interface IStatConfig<TEnum> where TEnum : Enum
{
    float GetValue(TEnum stat, int level = 1);
}
