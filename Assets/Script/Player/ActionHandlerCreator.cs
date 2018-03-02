using System;

public class ActionHandlerCreator
{
    public static IActionHandler CreateCommonActionHandler(IActorController actionController, IStatusHandler statusHandler, MobaObjectID type)
    {
        return new CommonPlayerActionHandler(actionController, statusHandler);
    }
}