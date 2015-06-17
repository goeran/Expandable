using System.Collections.Generic;

namespace Expandable.Internal
{
    internal interface IObjectCreationStrategy<T>
    {
        T CreateObjectFromRow(IList<string> row);
    }
}
