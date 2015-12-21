abstract public class FSMState <T>   {
	public string Name {
		get {
			return this.GetType().ToString();
		}
	}

	abstract public void Enter (T entity);
		
	abstract public void Execute (T entity);

	abstract public void Exit(T entity);
}
