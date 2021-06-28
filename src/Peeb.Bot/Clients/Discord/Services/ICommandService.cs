using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Peeb.Bot.Clients.Discord.Services
{
    public interface ICommandService
    {
        event Func<Optional<CommandInfo>, ICommandContext, IResult, Task> CommandExecuted;
        Task<IEnumerable<ModuleInfo>> AddModulesAsync(Assembly assembly, IServiceProvider services);
        Task<IResult> ExecuteAsync(ICommandContext context, int argPos, IServiceProvider services, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception);
        SearchResult Search(ICommandContext context, int argPos);
    }
}
