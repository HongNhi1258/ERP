using Infragistics.Win;
using Infragistics.Win.FormattedLinkLabel;
using System.Drawing;

namespace DaiCo.ERPProject
{
  public class SuggestTextDrawFilter : IUIElementDrawFilter
  {
    public bool DrawElement(DrawPhase drawPhase, ref UIElementDrawParams drawParams)
    {
      if (drawParams.Font.Bold)
      {
        drawParams.AppearanceData.FontData.Bold = DefaultableBoolean.False;
        drawParams.AppearanceData.ForeColor = Color.Blue;
      }
      return false;
    }

    public DrawPhase GetPhasesToFilter(ref UIElementDrawParams drawParams)
    {
      if (drawParams.Element is TextSectionUIElement && drawParams.Element.Parent is FormattedTextUIElement)
        return DrawPhase.BeforeDrawElement;
      return DrawPhase.None;
    }
  }

}
