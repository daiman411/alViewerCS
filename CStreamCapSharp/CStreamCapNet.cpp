#include "stdafx.h"
#include "CStreamCapNet.h"

namespace alAVCapDllSharp
{
	using namespace System;
	using namespace System::Runtime::InteropServices;

	Capture::Capture()
	{
		videoDevice = new CStreamCap(NULL, 0);
		initialised = false;
		swapRGB = false;
		invertY = true;
	}

	bool Capture::Init(IntPtr hWnd, UINT idxVideo, UINT idxAudio)
	{
		if (!videoDevice->InitialCap())
			return 0;

		if (!videoDevice->FindDevices())
			return 0;

		if (!videoDevice->ChooseDevices(idxVideo, idxAudio))
			return 0;

		if (!videoDevice->BuildPreviewGraph((HWND)(hWnd.ToInt32())))
			return 0;

		initialised = true;

		return 1;
	}

	bool Capture::StartCapture()
	{
		if (!videoDevice->StartPreview())
			return 0;

		return 1;
	}

	void Capture::StopCapture()
	{
		videoDevice->StopPreview();
	}

	void Capture::GetPixels(void* data)
	{
		if (!initialised)
			return;

		videoDevice->GrabFrame((unsigned char *)data, swapRGB, invertY);
	}

	void Capture::GetPixels(IntPtr data)
	{
		this->GetPixels(data.ToPointer());
	}

	int Capture::GetWidth()
	{
		return videoDevice->VideoWidth();
	}

	int Capture::GetHeight()
	{
		return videoDevice->VideoHeight();
	}

}