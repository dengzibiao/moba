using System;

public class WalkHandleBase
{
    public WalkHandleBase ( WalkHandleMgr mgr )
    {
        m_Parent = mgr;
        m_Parent.AddHandleBase( this );
    }

    public void RegistHandle (UInt32 mID , WalkHandleMgr.Handle hHandle )
    {
        if ( m_Parent != null )
            m_Parent.RegistHandle( mID , hHandle );
    }

    virtual public void RegistAllHandle ()
    {
    }

    private WalkHandleMgr m_Parent;
}
