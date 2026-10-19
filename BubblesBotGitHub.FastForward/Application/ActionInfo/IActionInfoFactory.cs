namespace BubblesBotGitHub.FastForward.Application.ActionInfo;

public interface IActionInfoFactory
{
    Task<IActionInfo> Create();
}