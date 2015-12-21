using UnityEngine;

public class FiniteStateMachine <T>  {
	private T Owner;
	private FSMState<T> _currentState;
	private FSMState<T> _previousState;
	
	public FSMState<T> CurrentState {
		get { return _currentState; }
	}
	public FSMState<T> PreviousState {
		get { return _previousState; }
	}
	
	public void Awake()
	{		
		_currentState = null;
		_previousState = null;
	}
	
	public void Configure(T owner, FSMState<T> InitialState) {
		Owner = owner;
		ChangeState(InitialState);
	}

	public void  Update()
	{
		if (_currentState != null) {
			_currentState.Execute(Owner);
		}
	}
 
	public void  ChangeState(FSMState<T> newState)
	{	
		_previousState = _currentState;
 
		if (_currentState != null)
			_currentState.Exit(Owner);
 
		_currentState = newState;
 
		if (_currentState != null)
			_currentState.Enter(Owner);
	}
}
