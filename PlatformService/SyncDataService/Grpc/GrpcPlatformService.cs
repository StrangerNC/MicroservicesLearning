using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataService.Grpc;

public class GrpcPlatformService(IPlatformRepository repository, IMapper mapper) : GrpcPlatform.GrpcPlatformBase
{
    private readonly IPlatformRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
    {
        var response = new PlatformResponse();
        var platforms = _repository.GetAllPlatforms();
        foreach (var platform in platforms)
        {
            response.Platform.Add(_mapper.Map<GrpcPlatformModel>(platform));
        }

        return Task.FromResult(response);
    }
}