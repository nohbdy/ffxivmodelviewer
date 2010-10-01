using System.Collections.Generic;

namespace DatDigger
{
    public interface INavigable
    {
        string DisplayName { get; }
        INavigable Parent { get; }
        List<INavigable> Children { get; }
    }
}
