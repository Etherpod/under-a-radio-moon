using System;
using UnityEngine;

namespace Jam5Project;

public class ErnestonianText : NomaiWallText
{
    [SerializeField]
    private bool _alwaysAllowFocus = false;
    [SerializeField]
    private bool _startWithAllLinesVisible = true;
    [SerializeField]
    private bool _startHidden = false;

    public override void LateInitialize()
    {
        if (_startHidden)
        {
            HideTextOnStart();
            HideImmediate();
        }
        
        base.LateInitialize();
        
        if (_startWithAllLinesVisible) {
            foreach (var line in _textLines)
            {
                line.SetUnreadState(true);
            }
        }

        string text = OWUtilities.RemoveByteOrderMark(_nomaiTextAsset);
        TranslatorTextBuilder.AddTranslation(text);
    }

    public override bool CheckAllowFocus(float focusDistance, Vector3 focusDirection)
    {
        if (_alwaysAllowFocus)
        {
            return focusDistance <= _interactRange;
        }

        return base.CheckAllowFocus(focusDistance, focusDirection);
    }
}
