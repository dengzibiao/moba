using System;

public class CHandleBase 
{
	public CHandleBase (CHandleMgr mgr)
	{
		m_Parent = mgr;
		m_Parent.AddHandleBase(this);
	}
	
	public void	RegistHandle( UInt32 mID, CHandleMgr.Handle hHandle)
	{
		if ( m_Parent != null )
			m_Parent.RegistHandle( mID, hHandle );
    }
			
	virtual public void	   RegistAllHandle()
	{ 
	}
	
	private CHandleMgr m_Parent;
}

