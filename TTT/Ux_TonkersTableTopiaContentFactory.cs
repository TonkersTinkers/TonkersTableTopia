using UnityEngine;

public static class Ux_TonkersTableTopiaContentFactory
{
    public static GameObject Create(RectTransform parent, Ux_TonkersTableTopiaStandardContentType contentType)
    {
        if (parent == null)
        {
            return null;
        }

        switch (contentType)
        {
            case Ux_TonkersTableTopiaStandardContentType.Button:
                return parent.CreateButtonBellyFlop();

            case Ux_TonkersTableTopiaStandardContentType.Image:
                return parent.CreateImageCheeseburger();

            case Ux_TonkersTableTopiaStandardContentType.RawImage:
                return parent.CreateRawImageNachos();

            case Ux_TonkersTableTopiaStandardContentType.Text:
                return parent.CreateTextDadJokes();

            case Ux_TonkersTableTopiaStandardContentType.Toggle:
                return parent.CreateToggleFlipFlop();

            case Ux_TonkersTableTopiaStandardContentType.Slider:
                return parent.CreateSliderSlipNSlide();

            case Ux_TonkersTableTopiaStandardContentType.Scrollbar:
                return parent.CreateScrollbarScoot();

            case Ux_TonkersTableTopiaStandardContentType.ScrollRect:
                return parent.CreateScrollRectScooter();

            case Ux_TonkersTableTopiaStandardContentType.InputField:
                return parent.CreateInputFieldChattyCathy();

            case Ux_TonkersTableTopiaStandardContentType.Dropdown:
                return parent.CreateDropdownDropItLikeItsHot();

            default:
                return null;
        }
    }
}