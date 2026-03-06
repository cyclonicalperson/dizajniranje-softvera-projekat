using System;

namespace CoWorkingManager.Logika.Servisi
{
	public interface IPretplatnik
	{
		void prijaviSe(IObserver observer);
		void odjaviSe(IObserver observer);
		void notifikacija(string poruka);
	}
}
