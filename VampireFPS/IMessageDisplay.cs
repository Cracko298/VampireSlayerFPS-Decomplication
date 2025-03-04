using Microsoft.Xna.Framework;

namespace VampireFPS;

internal interface IMessageDisplay : IDrawable, IUpdateable
{
	void ShowMessage(string message, params object[] parameters);
}
