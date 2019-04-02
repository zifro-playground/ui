using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PM
{
	public class VarTooltip : UITooltip
	{
		public override GameObject prefab => UISingleton.instance.varTooltipPrefab;
		public override Vector2 offset => new Vector2(10, tooltipRect ? -tooltipRect.sizeDelta.y - 20 : -20);

		protected Text tooltipHeader;
		public string header;
		public Color textColor;

		protected override void FetchTextReferences()
		{
			tooltipText = tooltipRect.Find("Value").GetComponent<Text>();
			tooltipHeader = tooltipRect.Find("Name").GetComponent<Text>();
		}

		protected override void ResizeToFit()
		{
			var parent = tooltipRect.parent as RectTransform;

			while (GetPreferredTextHeight() > tooltipText.rectTransform.rect.height)
			{
				tooltipRect.sizeDelta += SIZE_INCREMENT;
				tooltipRect.sizeDelta = new Vector2(
					Mathf.Min(tooltipRect.sizeDelta.x + SIZE_INCREMENT.x, parent.sizeDelta.x),
					Mathf.Min(tooltipRect.sizeDelta.y + SIZE_INCREMENT.y, parent.sizeDelta.y)
				);
				if (Vector2.Distance(tooltipRect.sizeDelta, parent.sizeDelta) <= Vector2.kEpsilon)
				{
					break;
				}
			}

			tooltipRect.sizeDelta = new Vector2(
				Mathf.Min(Mathf.Max(tooltipRect.sizeDelta.x, tooltipHeader.preferredWidth + 40), parent.sizeDelta.x),
				tooltipText.preferredHeight + 46
			);
		}

		private float GetPreferredTextHeight()
		{
			Canvas.ForceUpdateCanvases();
			return tooltipText.preferredHeight;
		}

		public override void ApplyTooltipTextChange()
		{
			if (!initialized)
			{
				Init();
			}

			tooltipHeader.text = header ?? "";
			tooltipText.color = textColor;
			base.ApplyTooltipTextChange();
		}
	}
}