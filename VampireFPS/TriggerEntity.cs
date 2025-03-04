using System;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Rendering;

namespace VampireFPS;

[Serializable]
[EditorCreatedObject]
public class TriggerEntity : SceneEntity
{
	public override void RenderEditorIcon(ISceneState scenestate, BoundingBoxRenderHelper renderhelper, bool highlighted, bool selected, bool sceneoccludedpass)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		g.m_App.m_BEPUDebugDrawer.Draw(((SceneEntity)this).World, g.m_App.sceneState.View, g.m_App.sceneState.Projection);
		((SceneEntity)this).RenderEditorIcon(scenestate, renderhelper, highlighted, selected, sceneoccludedpass);
	}
}
