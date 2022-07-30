using Server.Entities;
using Server.Helpers;

namespace Server.Services;

public interface IDmdService
{
    public Dmd GetDmd(Guid id);
    public void PostDmd(IEnumerable<Guid> id,string name);

    public IEnumerable<Dmd> GetAllDmd();
}

public class DmdService: IDmdService
{
    private DataContext context;
    public DmdService(DataContext context)
    {
        this.context = context;
    }

    public Dmd GetDmd(Guid id)
    {
        return context.DMDs.First(x => x.Id == id);
    }

    public void PostDmd(IEnumerable<Guid> ids, string name)
    {
        context.DMDs.Add(new Dmd
        {
            Name = name,
            MeditationsId = ids
        });
    }

    public IEnumerable<Dmd> GetAllDmd()
    {
        return context.DMDs;
    }
}