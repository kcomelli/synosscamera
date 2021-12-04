using AutoMapper;
using synosscamera.core.Model.Dto.Camera;
using synosscamera.station.Model.ApiInfo;

namespace synosscamera.api.Infrastructure
{
    /// <summary>
    /// Automapper profile
    /// </summary>
    public class MapperProfile : Profile
    {
        /// <summary>
        /// Constructor of the class
        /// </summary>
        public MapperProfile()
        {
            Configure();
        }

        private void Configure()
        {
            ConfigureCamera();
        }

        private void ConfigureCamera()
        {
            CreateMap<CameraInfo, CameraDetails>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));
            CreateMap<CameraSpecialInfo, CameraDetailsSpecialized>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
                .ForMember(dest => dest.RecordingStatus, opt => opt.MapFrom(src => (int)src.RecordingStatus))
                .ForMember(dest => dest.Ip, opt => opt.MapFrom(src => src.Host));
            CreateMap<synosscamera.station.Model.ApiInfo.CameraDetailInfo, synosscamera.core.Model.Dto.Camera.CameraDetailInfo>();
        }
    }
}
