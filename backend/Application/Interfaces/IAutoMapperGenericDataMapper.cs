using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAutoMapperGenericDataMapper
    {
        TDestination Map<TSource, TDestination>(TSource source)
         where TDestination : class
         where TSource : class;

        TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
            where TDestination : class
            where TSource : class;

        IQueryable<TDestination> Project<TSource, TDestination>(IQueryable<TSource> sources)
            where TDestination : class
            where TSource : class;

    }
}
