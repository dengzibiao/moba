internal class CommonSummon : BaseStatus
{
    private int index;

    public CommonSummon(int index)
    {
        this.index = index;
    }

    public override bool OnEnter(STATUS last)
    {
        fightTouch.DoSummon(index, null);
        return true;
    }

    public override STATUS GetNextStatus()
    {
        return STATUS.IDLE;
    }
}