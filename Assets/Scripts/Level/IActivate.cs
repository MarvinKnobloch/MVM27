using System;
using UnityEngine;

public interface IActivate
{
    public void SetRequirement();
    public void Activate();
    public void Deactivate();
}
