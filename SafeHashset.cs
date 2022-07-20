using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;


/// <summary>
/// This hashset can be used in foreach loops without risk of exception because the set changes mid loop.
/// loop checks out before starting, then checks in when finished. Any change to the loop mus happen following WaitForReady.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SafeHashset<T> : HashSet<T>
{
	public bool Busy { private set; get; }

	public async Task CheckOut()
	{
		await WaitForReady();
		Busy = true;
	}

	public void CheckIn() => Busy = false;



	public async Task WaitForReady()
	{
		while (Busy)
			await Task.Delay(10);
	}

	public async Task SafeAdd(T item)
	{
		await WaitForReady();
		Add(item);
	}

	public async Task SafeRemove(T item)
	{
		await WaitForReady();
		Remove(item);
	}
}
