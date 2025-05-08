using VampireCommandFramework;

namespace AuraFarming;

public class Commands
{
    [Command("AuraT1", shortHand: "at1", description: "Applies thwe T1 Auras", adminOnly: true)]
    public void ApplyAuraT1(ChatCommandContext ctx)
    {
        Aura.TryRemoveAllAuras(ctx.Event.SenderCharacterEntity);
        foreach (var aura in Aura.aurasT1)
        {
            Aura.ApplyAura(ctx.Event.SenderCharacterEntity, ctx.Event.SenderUserEntity, aura);
        }
        ctx.Reply("Applied T1 Auras!");
    }

    [Command("AuraT2", shortHand: "at2", description: "Applies thwe T2 Auras", adminOnly: true)]
    public void ApplyAuraT2(ChatCommandContext ctx)
    {
        Aura.TryRemoveAllAuras(ctx.Event.SenderCharacterEntity);
        foreach (var aura in Aura.aurasT2)
        {
            Aura.ApplyAura(ctx.Event.SenderCharacterEntity, ctx.Event.SenderUserEntity, aura);
        }
        ctx.Reply("Applied T2 Auras!");
    }

    [Command("AuraT3", shortHand: "at3", description: "Applies thwe T3 Auras", adminOnly: true)]
    public void ApplyAuraT3(ChatCommandContext ctx)
    {
        Aura.TryRemoveAllAuras(ctx.Event.SenderCharacterEntity);
        foreach (var aura in Aura.aurasT3)
        {
            Aura.ApplyAura(ctx.Event.SenderCharacterEntity, ctx.Event.SenderUserEntity, aura);
        }
        ctx.Reply("Applied T3 Auras!");
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