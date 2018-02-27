#pragma once
// videoInput.NET.h
#include "CStreamCap.h"

#pragma once

using namespace System;

namespace alAVCapDllSharp
{
	public ref class Capture
	{
	public:
		Capture();

		bool	Init(IntPtr hwnd, UINT idxVideo, UINT idxAudio);
		bool	StartCapture();
		void	StopCapture();

		int		GetWidth();
		int		GetHeight();
		void	GetPixels(void* data);
		void	GetPixels(IntPtr data);

	protected:
		CStreamCap*	videoDevice;

		bool	initialised;
		bool	swapRGB;
		bool	invertY;
	};
}
