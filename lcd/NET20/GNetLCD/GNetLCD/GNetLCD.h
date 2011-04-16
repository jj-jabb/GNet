// GNetLCD.h

#pragma once

using namespace System;
using namespace Runtime::InteropServices;

namespace GNet {
	
    //lgLcdConnectContext connectContext;
    //lgLcdOpenByTypeContext openContext;

	//DWORD CALLBACK OnLCDButtonsCallback(int device, DWORD dwButtons, const PVOID pContext)
	//{
	//	return 0;
	//}

	//void LcdOpen()
	//{
	//	DWORD res;

	//	//// initialize the library
	//	res = lgLcdInit();
 //   
	//	//// connect to LCDMon
	//	// set up connection context
	//	lgLcdConnectContext connectContext;
	//	ZeroMemory(&connectContext, sizeof(connectContext));
	//	connectContext.appFriendlyName = _T("simple color sample");
	//	connectContext.isAutostartable = FALSE;
	//	connectContext.isPersistent = FALSE;
	//	// we don't have a configuration screen
	//	connectContext.onConfigure.configCallback = NULL;
	//	connectContext.onConfigure.configContext = NULL;
	//	// the "connection" member will be returned upon return
	//	connectContext.connection = LGLCD_INVALID_CONNECTION;
	//	// and connect
	//	res = lgLcdConnect(&connectContext);

	//	// Let's attempt to open up a color device
	//	lgLcdOpenByTypeContext openContext;
	//	ZeroMemory(&openContext, sizeof(openContext));
	//	openContext.connection = connectContext.connection;
	//	openContext.deviceType = LGLCD_DEVICE_BW;
	//	// we have no softbutton notification callback
	//	openContext.onSoftbuttonsChanged.softbuttonsChangedCallback = OnLCDButtonsCallback;
	//	openContext.onSoftbuttonsChanged.softbuttonsChangedContext = NULL;
	//	// the "device" member will be returned upon return
	//	openContext.device = LGLCD_INVALID_DEVICE;
	//	res = lgLcdOpenByType(&openContext);
	//}

	//void LcdClose()
	//{
	//	DWORD res;

	//	// let's close the device again
	//	res = lgLcdClose(openContext.device);

	//	// and take down the connection
	//	res = lgLcdDisconnect(connectContext.connection);

	//	// and shut down the library
	//	res = lgLcdDeInit();
	//}

	public enum class LcdDeviceType
	{
		LcdDeviceBW = 0x00000001,
		LcdDeviceQVGA = 0x00000002
	};

	public ref class Lcd : IDisposable {
	public:
		Lcd(String^ appFriendlyName, bool isPersistent, bool isAutostartable, LcdDeviceType deviceType)
		{
			this->appFriendlyName = appFriendlyName;
			this->isPersistent = isPersistent;
			this->isAutostartable = isAutostartable;
			this->deviceType = deviceType;

			lgLcdInit();
			Connect();
			Open();
		}

		~Lcd()
		{
			Close();
			Disconnect();
			lgLcdDeInit();
		}

		void BringToFront()
		{
			int result = lgLcdSetAsLCDForegroundApp(openContext->device, 1);

			int x=  result;
		}

	protected:
		delegate int LcdOnSoftButtonsCB(int device, int dwButtons, IntPtr pContext);

		virtual int SoftbuttonsChanged(int a, int b, IntPtr c)
		{
			System::Console::WriteLine("SoftbuttonsChanged");
			return 0;
		}

		void Connect()
		{
			connectContext = new lgLcdConnectContext();
#ifdef UNICODE
			connectContext->appFriendlyName = (LPCWSTR)(Marshal::StringToHGlobalUni(appFriendlyName)).ToPointer();
#else
			connectContext->appFriendlyName = (LPCSTR)(Marshal::StringToHGlobalAnsi(appFriendlyName)).ToPointer();
#endif
			connectContext->isPersistent = isPersistent;
			connectContext->isAutostartable = isAutostartable;
			connectContext->connection = LGLCD_INVALID_CONNECTION;

			lgLcdConnect(connectContext);
		}

		void Disconnect()
		{
			if (connectContext != 0)
			{
				lgLcdDisconnect(connectContext->connection);
				Marshal::FreeHGlobal(IntPtr((void*)connectContext->appFriendlyName));
				delete(connectContext);
				connectContext = 0;
			}
		}

		bool Open()
		{
			if (connectContext == 0 || connectContext->connection == LGLCD_INVALID_CONNECTION)
				return false;

			openContext = new lgLcdOpenByTypeContext();
			openContext->connection = connectContext->connection;
			openContext->deviceType = (int)deviceType;

			onSoftButtonsDelegate = gcnew LcdOnSoftButtonsCB(this, &Lcd::SoftbuttonsChanged);

			openContext->onSoftbuttonsChanged.softbuttonsChangedCallback = (lgLcdOnSoftButtonsCB)(Marshal::GetFunctionPointerForDelegate(onSoftButtonsDelegate)).ToPointer();

			int val = lgLcdOpenByType(openContext);

			switch (deviceType)
			{
				case LcdDeviceType::LcdDeviceBW:
					{
						lgLcdBitmap160x43x1 bmp;
						bmp.hdr.Format = LGLCD_BMP_FORMAT_160x43x1;

						ZeroMemory(&bmp.pixels, sizeof(bmp.pixels));
						val = lgLcdUpdateBitmap(openContext->device, &bmp.hdr, LGLCD_SYNC_UPDATE(LGLCD_PRIORITY_NORMAL));
					}
					break;

				case LcdDeviceType::LcdDeviceQVGA:
					{
						lgLcdBitmapQVGAx32 bmp;
						bmp.hdr.Format = LGLCD_BMP_FORMAT_QVGAx32;

						ZeroMemory(&bmp.pixels, sizeof(bmp.pixels));
						val = lgLcdUpdateBitmap(openContext->device, &bmp.hdr, LGLCD_SYNC_UPDATE(LGLCD_PRIORITY_NORMAL));
					}
					break;
			}

			return true;
		}

		void Close()
		{
			if (openContext != 0)
			{
				lgLcdSetAsLCDForegroundApp(openContext->device, 0);
				// TODO: free any allocated memory from Open
				lgLcdClose(openContext->device);
				delete openContext;
				openContext = 0;
			}
		}

		//void InitBitmap()
		//{
		//	bitmap = new lgLcdBitmap160x43x1();
		//	bitmap->hdr.Format = LGLCD_BMP_FORMAT_160x43x1;
		//}

	protected: 
		String^ appFriendlyName;
		bool isPersistent;
		bool isAutostartable;
		LcdDeviceType deviceType;
		LcdOnSoftButtonsCB^ onSoftButtonsDelegate;

	private:
		lgLcdConnectContext* connectContext;
		lgLcdOpenByTypeContext* openContext;
		//lgLcdSoftbuttonsChangedContext* sbChangedContext;
		//lgLcdBitmap160x43x1* bitmap;
		//BYTE* pixels;
		//lgLcdBitmapHeader* bitmapHeader;
	};
}
