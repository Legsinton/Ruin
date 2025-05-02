using System;
using Unity.Behavior;

[BlackboardEnum]
public enum State
{
	Idle,
	Patrol,
	Searching,
	Chase,
	Attack
}
