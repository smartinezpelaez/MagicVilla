using AutoMapper;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;

namespace MagicVilla_API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            //Hay dos formas de mappear 
            //Forma 1
            CreateMap<Villa, VillaDto>();  
            CreateMap<VillaDto, Villa>();

            //Forma 2
            CreateMap<Villa, VillaCreateDto>().ReverseMap();

            CreateMap<Villa, VillaUpdateDto>().ReverseMap();

            CreateMap<NumeroVilla, NumeroVillaDto>().ReverseMap();

            CreateMap<NumeroVilla, NumeroVillaCrearDto>().ReverseMap();

            CreateMap<NumeroVilla, NumeroVillaUpdateDto>().ReverseMap();

        }

    }
}
