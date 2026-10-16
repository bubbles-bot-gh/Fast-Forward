using BubblesBotGitHub.FastForward.Application.ActionInfo;

namespace BubblesBotGitHub.FastForward.Application.Interfaces;

public interface IActionInfoFactory
{
    Task<IActionInfo> Create();
}