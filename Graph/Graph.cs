using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Graph
{
    public interface IGraph<T>
    {
        IObservable<IEnumerable<T>> RoutesBetween(T source, T target);
    }

    public class Graph<T> : IGraph<T>
    {
        private readonly Dictionary<T, IEnumerable<T>> _routes;
        
        public Graph(IEnumerable<ILink<T>> links)
        {
            _routes = links.GroupBy(l => l.Source)
                            .ToDictionary(g => g.Key, g => g.Select(l => l.Target));
        }

        public IObservable<IEnumerable<T>> RoutesBetween(T source, T target)
        {
            var routes = new List<IEnumerable<T>>();

            var nextNodes = _routes[source];

            foreach (var nextNode in nextNodes)
            {
                var path = new List<T>();
                path.Add(source);
                
                var fullPath = getFullPath(nextNode, target, path);
                
                routes.Add(fullPath);
            }
            
            return routes.ToObservable();
        }

        private IList<T> getFullPath(T source, T target, IList<T> path)
        {
            var emptyList = new List<T>();
            
            if (path.Contains(source))
            {
                return emptyList;
            }
            
            path.Add(source);

            if (source.Equals(target))
            {
                return path;
            }
            
            var nextNodes = _routes[source];
            foreach (var nextNode in nextNodes)
            {
                var fullPath = getFullPath(nextNode, target, path);

                // If fullPath is not an Empty List
                if (fullPath.Count > 0)
                {
                    return fullPath;
                }
            }
            return emptyList;
        }
    }
}
