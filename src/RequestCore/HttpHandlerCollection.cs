using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RequestCore
{
    public class HttpHandlerCollection
    {
        private readonly IDictionary<string, Func<string, string, Task<object>>> _twoArgHandlers = new Dictionary<string, Func<string, string, Task<object>>>();
        private readonly IDictionary<string, Action<string>> _singleArgActions = new Dictionary<string, Action<string>>();
        private readonly IDictionary<string, Func<string, Task<object>>> _singleArgFunction = new Dictionary<string, Func<string, Task<object>>>();

        public Setter this[string routePattern] => new Setter(this, routePattern);

        public bool ContainsRoute(string route)
        {
            if (_singleArgActions.ContainsKey(route) || _singleArgFunction.ContainsKey(route)) return true;

            var firstMatch = _twoArgHandlers.Keys.Select(key => new Regex(key).Match(route)).FirstOrDefault(match => match.Success);
            return (firstMatch != null);
        }

        public async Task<object> InvokeHandler(string route, string requestBody)
        {
            if (_singleArgActions.ContainsKey(route))
            {
                _singleArgActions[route](requestBody);
                return null;
            }
            else if (_singleArgFunction.ContainsKey(route))
            {
                return await _singleArgFunction[route](requestBody);
            }
            else
            {
                var firstMatch = _twoArgHandlers.Keys.Select(key => new Tuple<string, Match>(key, new Regex(key).Match(route))).First(match => match.Item2.Success);
                var handler = _twoArgHandlers[firstMatch.Item1];

                return await handler(requestBody, firstMatch.Item2.Groups[1].Value);
            }
        }

        private void AddHandler(string routeString, Func<string, string, Task<object>> handler)
        {
            _twoArgHandlers[routeString] = handler;
        }

        private void AddHandler(string routeString, Func<string, Task<object>> handler)
        {
            _singleArgFunction[routeString] = handler;
        }

        private void AddHandler(string routeString, Action<string> handler)
        {
            _singleArgActions[routeString] = handler;
        }

        public class Setter
        {
            private readonly HttpHandlerCollection _parent;

            private readonly string _routeString;

            public Setter(HttpHandlerCollection parent, string routeString)
            {
                _parent = parent;
                _routeString = routeString;
            }

            public void With(Action<string> handler)
            {
                _parent.AddHandler(_routeString, handler);
            }

            public void With(Func<string, Task<object>> handler)
            {
                _parent.AddHandler(_routeString, handler);
            }

            public void With(Func<string, string, Task<object>> handler)
            {
                _parent.AddHandler(_routeString, handler);
            }
        }
    }
}