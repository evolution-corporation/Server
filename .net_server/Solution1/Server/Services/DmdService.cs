using Server.Entities;
using Server.Helpers;

namespace Server.Services;

public interface IDmdService
{
    public Dmd GetDmd(int id);
    public void PostDmd(List<int> id,string name);

    public IEnumerable<Dmd> GetAllDmd();
}

public class DmdService: IDmdService
{
    private DataContext context;
    public DmdService(DataContext context)
    {
        this.context = context;
    }

    public Dmd GetDmd(int id)
    {
        return context.DMDs.AsQueryable().First(x => x.Id == id);
    }

    public void PostDmd(List<int> ids, string name)
    {
        context.DMDs.Add(new Dmd
        {
            Name = name,
            MeditationsId = ids
        });
        context.SaveChanges();
    }

    public IEnumerable<Dmd> GetAllDmd()
    {
        return context.DMDs.AsQueryable().ToArray();
    }
}