using VampireCommandFramework;

namespace AuraFarming;

public class Commands
{
    [Command("AuraT1", shortHand: "at1", description: "Applies thwe T1 Auras", adminOnly: true)]
    public void ApplyAuraT1(ChatCommandContext ctx)
    {
        ctx.Reply("TODO: Applying T1 Auras...");
    }

    [Command("AuraT2", shortHand: "at2", description: "Applies thwe T2 Auras", adminOnly: true)]
    public void ApplyAuraT2(ChatCommandContext ctx)
    {
        ctx.Reply("TODO: Applying T2 Auras...");
    }

    [Command("AuraT3", shortHand: "at3", description: "Applies thwe T3 Auras", adminOnly: true)]
    public void ApplyAuraT3(ChatCommandContext ctx)
    {
        ctx.Reply("TODO: Applying T3 Auras...");
    }

    [Command("setandgetbuffinfo", shortHand: "sbi", adminOnly: true)]
    public void SetAndGetBuffInfoCommand(ChatCommandContext ctx, int buffId)
    {
        D.GiveBuffAndGetBuffInfo(ctx, buffId);
        ctx.Reply("complete");
    }
    [Command("buffinfo", shortHand: "bi", adminOnly: true)]
    public void GetBuffInfoCommand(ChatCommandContext ctx, int buffId)
    {
        D.GetBuffInfo(ctx, buffId);
        ctx.Reply("complete");
    }


    // TODO: add remove commands for admins
}