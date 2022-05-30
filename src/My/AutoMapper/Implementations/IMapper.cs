namespace AutoMapper.Implementations;

internal interface IMapper
{
    TTo Map<TFrom, TTo>(TFrom from) 
        where TTo : new();
}
