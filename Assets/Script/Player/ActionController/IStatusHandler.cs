using System;
public interface IStatusHandler
{
	bool ChangeStatusTo (STATUS next);
}