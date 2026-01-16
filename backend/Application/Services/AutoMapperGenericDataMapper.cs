using Application.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AutoMapperGenericDataMapper : IAutoMapperGenericDataMapper
    {
        private readonly IMapper _mapper;
        public AutoMapperGenericDataMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
        public TDestination Map<TSource, TDestination>(TSource source)
            where TSource : class
            where TDestination : class
        {
            return _mapper.Map<TSource, TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
            where TSource : class
            where TDestination : class
        {
            return _mapper.Map<TSource, TDestination>(source, destination);
        }

        public IQueryable<TDestination> Project<TSource, TDestination>(IQueryable<TSource> sources)
            where TSource : class
            where TDestination : class
        {
            return _mapper.ProjectTo<TDestination>(sources);
        }
    }
}
