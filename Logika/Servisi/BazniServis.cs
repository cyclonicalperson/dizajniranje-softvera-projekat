using CoWorkingManager.Logika.Servisi;
using System.Collections.Generic;

namespace CoWorkingManager.Logika.Servisi
{
	public abstract class BazniServis : IPretplatnik
	{
		private readonly List<IObserver> _posmatraci = new();

		public void prijaviSe(IObserver posmatrac) => _posmatraci.Add(posmatrac);
		public void odjaviSe(IObserver posmatrac) => _posmatraci.Remove(posmatrac);
		public void notifikacija(string poruka)
		{
			foreach (var posmatrac in _posmatraci)
				posmatrac.Update(poruka);
		}
	}
}
