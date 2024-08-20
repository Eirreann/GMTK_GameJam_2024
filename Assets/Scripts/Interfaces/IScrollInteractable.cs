using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScrollInteractable
{
    public void OnScrollValue(bool direction);

    public void OnHover(bool state);
}
