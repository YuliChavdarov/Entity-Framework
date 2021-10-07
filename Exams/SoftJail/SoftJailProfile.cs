namespace SoftJail
{
    using AutoMapper;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Globalization;

    public class SoftJailProfile : Profile
    {
        // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE THIS CLASS
        public SoftJailProfile()
        {
            this.CreateMap<PrisonerInputModel, Prisoner>()
                .ForMember(x => x.IncarcerationDate, y => y.MapFrom(s => DateTime.ParseExact(s.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(x => x.ReleaseDate, y => y.MapFrom(s => s.ReleaseDate == null ? (DateTime?)null : DateTime.ParseExact(s.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)));

            this.CreateMap<MailInputModel, Mail>();

            this.CreateMap<OfficerInputModel, Officer>()
                .ForMember(x => x.Position, y => y.MapFrom(s => Enum.Parse<Position>(s.Position)))
                .ForMember(x => x.Weapon, y => y.MapFrom(s => Enum.Parse<Weapon>(s.Weapon)));

            this.CreateMap<OfficerPrisonerInputModel, OfficerPrisoner>();
        }
    }
}
