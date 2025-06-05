using CommandsService.Models;

namespace CommandsService.Data;

public class CommandRepository(AppDbContext context) : ICommandRepository
{
    private readonly AppDbContext _context = context;

    public bool SaveChanges()
    {
        return _context.SaveChanges() > 0;
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms.ToList();
    }

    public void CreatePlatform(Platform platform)
    {
        if (platform == null) throw new ArgumentNullException(nameof(platform));

        _context.Platforms.Add(platform);
    }

    public bool PlatformExists(int platformId)
    {
        return _context.Platforms.Any(p => p.Id == platformId);
    }

    public bool ExternalPlatformExists(int externalId)
    {
        return _context.Platforms.Any(p => p.ExternalId == externalId);
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        return _context.Commands.Where(c => c.PlatformId == platformId).OrderBy(c => c.Platform.Name);
    }

    public Command? GetCommand(int platformId, int commandId)
    {
        return _context.Commands.FirstOrDefault(c => c.Id == commandId && c.PlatformId == platformId);
    }

    public void CreateCommand(int platformId, Command command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        command.PlatformId = platformId;
        _context.Commands.Add(command);
    }
}