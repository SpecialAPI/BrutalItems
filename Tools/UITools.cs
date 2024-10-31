using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Tools
{
    public static class UITools
    {
        public static void DoJumpingText(string text, Vector3 worldPosition, float heightOffGround, Color? color = null)
        {
            var root = GameUIRoot.Instance;

            if(root == null)
                return;

            if (root.m_inactiveDamageLabels.Count == 0)
            {
                GameObject gameObject = (GameObject)Object.Instantiate(BraveResources.Load("DamagePopupLabel"), root.transform);
                root.m_inactiveDamageLabels.Add(gameObject.GetComponent<dfLabel>());
            }

            var label = root.m_inactiveDamageLabels[0];
            root.m_inactiveDamageLabels.RemoveAt(0);

            label.gameObject.SetActive(true);
            label.Text = text;
            label.Color = color ?? Color.white;
            label.Opacity = 1f;

            label.transform.position = dfFollowObject.ConvertWorldSpaces(worldPosition, GameManager.Instance.MainCameraController.Camera, root.Manager.RenderCamera).WithZ(0f);
            label.transform.position = label.transform.position.QuantizeFloor(label.PixelsToUnits() / (Pixelator.Instance.ScaleTileScale / Pixelator.Instance.CurrentTileScale));
            label.StartCoroutine(root.HandleDamageNumberCR(worldPosition, worldPosition.y - heightOffGround, label));
        }
    }
}
