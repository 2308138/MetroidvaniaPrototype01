using UnityEngine;

public abstract class BossState : ScriptableObject
{
    protected BossController boss;

    public virtual void Initialize(BossController controller) => boss = controller;
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();

    public virtual void OnParried() { }
}