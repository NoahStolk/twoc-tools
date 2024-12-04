using Silk.NET.OpenGL;
using System.Numerics;

namespace TwocTools.App.Rendering;

internal sealed class SceneFramebuffer(GL gl, LineRenderer lineRenderer)
{
	private Vector2 _cachedFramebufferSize;
	private uint _framebufferId;

	public uint FramebufferTextureId { get; private set; }

	public unsafe void Initialize(Vector2 framebufferSize)
	{
		if (framebufferSize.X < 1)
			framebufferSize.X = 1;
		if (framebufferSize.Y < 1)
			framebufferSize.Y = 1;

		if (_cachedFramebufferSize == framebufferSize)
			return;

		if (_framebufferId != 0)
			gl.DeleteFramebuffer(_framebufferId);

		if (FramebufferTextureId != 0)
			gl.DeleteTexture(FramebufferTextureId);

		_framebufferId = gl.GenFramebuffer();
		gl.BindFramebuffer(FramebufferTarget.Framebuffer, _framebufferId);

		FramebufferTextureId = gl.GenTexture();
		gl.BindTexture(TextureTarget.Texture2D, FramebufferTextureId);
		gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, (uint)framebufferSize.X, (uint)framebufferSize.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, null);
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
		gl.TexParameterI(TextureTarget.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
		gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, FramebufferTextureId, 0);

		uint rbo = gl.GenRenderbuffer();
		gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);

		gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent24, (uint)framebufferSize.X, (uint)framebufferSize.Y);
		gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rbo);

		GLEnum framebufferStatus = gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
		if (framebufferStatus != GLEnum.FramebufferComplete)
		{
			GLEnum error = gl.GetError();
			throw new InvalidOperationException($"Framebuffer for scene is not complete. Framebuffer status: {framebufferStatus} Last error: {error}");
		}

		gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		gl.DeleteRenderbuffer(rbo);

		_cachedFramebufferSize = framebufferSize;
	}

	public unsafe void RenderFramebuffer(Vector2 size)
	{
		if (_framebufferId == 0 || FramebufferTextureId == 0 || size.X < 1 || size.Y < 1)
			return;

		gl.BindFramebuffer(FramebufferTarget.Framebuffer, _framebufferId);

		// Keep track of the original viewport, so we can restore it later.
		Span<int> originalViewport = stackalloc int[4];
		gl.GetInteger(GLEnum.Viewport, originalViewport);
		gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);

		gl.ClearColor(0.3f, 0.3f, 0.3f, 0);
		gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		gl.Enable(EnableCap.DepthTest);
		gl.Enable(EnableCap.Blend);
		gl.Enable(EnableCap.CullFace);
		gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		lineRenderer.Render();

		gl.Viewport(originalViewport[0], originalViewport[1], (uint)originalViewport[2], (uint)originalViewport[3]);
		gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}
}
