// GNetLCD.h

#pragma once

using namespace System;
using namespace Runtime::InteropServices;

namespace GNet {
		
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
			return 1;
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

			return true;
		}

		void Close()
		{
			// TODO: free any allocated memory from Open
			lgLcdClose(openContext->device);
		}

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
		lgLcdBitmap* bitmap;
		lgLcdBitmapHeader* bitmapHeader;
	};
}
