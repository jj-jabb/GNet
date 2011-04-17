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

    public enum class LcdPriority
    {
        NoShow = 0,
        Background = 64,
        Normal = 128,
        Alert = 255
    };
    
	public ref class Lcd : IDisposable {
	public:
	    static int BW_SIZE = LGLCD_BMP_WIDTH * LGLCD_BMP_HEIGHT * LGLCD_BMP_BPP;
        static int QVGA_SIZE = LGLCD_QVGA_BMP_WIDTH * LGLCD_QVGA_BMP_HEIGHT * LGLCD_QVGA_BMP_BPP;

		Lcd(String^ appFriendlyName, bool isPersistent, bool isAutostartable, LcdDeviceType deviceType)
		{
			this->appFriendlyName = appFriendlyName;
			this->isPersistent = isPersistent;
			this->isAutostartable = isAutostartable;
			this->deviceType = deviceType;

            disposed = false;

            connectContext = 0;
            openContext = 0;

			lgLcdInit();
		}

		~Lcd()
		{
            disposed = true;

			Close();
			Disconnect();
			lgLcdDeInit();
		}
        
        property bool IsConnected { bool get() { return connectContext != 0; } }
        property bool IsOpen { bool get() { return openContext != 0; } }

		void BringToFront()
		{
            if (disposed) return;
            if (openContext == 0) return;

			int result = lgLcdSetAsLCDForegroundApp(openContext->device, 1);
		}

		int Connect()
		{
            if (disposed) return -1;
            if (connectContext != 0) return 0;

			connectContext = new lgLcdConnectContext();
#ifdef UNICODE
			connectContext->appFriendlyName = (LPCWSTR)(Marshal::StringToHGlobalUni(appFriendlyName)).ToPointer();
#else
			connectContext->appFriendlyName = (LPCSTR)(Marshal::StringToHGlobalAnsi(appFriendlyName)).ToPointer();
#endif
			connectContext->isPersistent = isPersistent;
			connectContext->isAutostartable = isAutostartable;
			connectContext->connection = LGLCD_INVALID_CONNECTION;

			DWORD result = lgLcdConnect(connectContext);
            if (ERROR_SUCCESS != result)
                connectContext = 0;

            return result;
		}

		bool Disconnect()
		{
            if (disposed) return false;
			if (connectContext == 0) return false;

			lgLcdDisconnect(connectContext->connection);
			Marshal::FreeHGlobal(IntPtr((void*)connectContext->appFriendlyName));
			delete(connectContext);
			connectContext = 0;
		}

		int Open()
		{
            if (disposed) return -1;
            if (openContext != 0) return 0;

			if (connectContext == 0 || connectContext->connection == LGLCD_INVALID_CONNECTION) return -2;

			openContext = new lgLcdOpenByTypeContext();
			openContext->connection = connectContext->connection;
			openContext->deviceType = (int)deviceType;

			LcdOnSoftButtonsCB^ onSoftButtonsDelegate = gcnew LcdOnSoftButtonsCB(this, &Lcd::SoftbuttonsChanged);
            onSoftButtonsGch = GCHandle::Alloc(onSoftButtonsDelegate);
            IntPtr onSoftButtonsIp = Marshal::GetFunctionPointerForDelegate(onSoftButtonsDelegate);

			openContext->onSoftbuttonsChanged.softbuttonsChangedCallback = (lgLcdOnSoftButtonsCB)(onSoftButtonsIp).ToPointer();

			int result = lgLcdOpenByType(openContext);
            
            if (ERROR_SUCCESS != result)
            {
                openContext = 0;
                return result;
            }

			switch (deviceType)
			{
				case LcdDeviceType::LcdDeviceBW:
					{
						lgLcdBitmap160x43x1 bmp;
						bmp.hdr.Format = LGLCD_BMP_FORMAT_160x43x1;

						ZeroMemory(&bmp.pixels, sizeof(bmp.pixels));
						result = lgLcdUpdateBitmap(openContext->device, &bmp.hdr, LGLCD_SYNC_UPDATE(LGLCD_PRIORITY_NORMAL));
					}
					break;

				case LcdDeviceType::LcdDeviceQVGA:
					{
						lgLcdBitmapQVGAx32 bmp;
						bmp.hdr.Format = LGLCD_BMP_FORMAT_QVGAx32;

						ZeroMemory(&bmp.pixels, sizeof(bmp.pixels));
						result = lgLcdUpdateBitmap(openContext->device, &bmp.hdr, LGLCD_SYNC_UPDATE(LGLCD_PRIORITY_NORMAL));
					}
					break;
			}

			return 0;
		}

		bool Close()
		{
            if (disposed) return false;
			if (openContext == 0) return false;

			lgLcdSetAsLCDForegroundApp(openContext->device, 0);
			lgLcdClose(openContext->device);
			delete openContext;
			openContext = 0;
            onSoftButtonsGch.Free();
		}

        int UpdateBitmap(array<System::Byte>^ bytes, LcdPriority priority)
        {
            if (disposed) return -1;

            pin_ptr<BYTE> pbytes = &bytes[0];
            int result = 0;

			switch (deviceType)
			{
				case LcdDeviceType::LcdDeviceBW:
					{
                        if (bytes->Length != BW_SIZE)
                            return -1;

						lgLcdBitmap160x43x1 bmp;
						bmp.hdr.Format = LGLCD_BMP_FORMAT_160x43x1;
                        memcpy(&bmp.pixels, pbytes, BW_SIZE);

						result = lgLcdUpdateBitmap(openContext->device, &bmp.hdr, LGLCD_SYNC_UPDATE((int)priority));
					}
					break;

				case LcdDeviceType::LcdDeviceQVGA:
					{
                        if (bytes->Length != QVGA_SIZE)
                            return -1;

						lgLcdBitmapQVGAx32 bmp;
						bmp.hdr.Format = LGLCD_BMP_FORMAT_QVGAx32;
                        memcpy(&bmp.pixels, pbytes, QVGA_SIZE);

						result = lgLcdUpdateBitmap(openContext->device, &bmp.hdr, LGLCD_SYNC_UPDATE((int)priority));
					}
					break;
			}

            return result;
        }

	protected:
		delegate int LcdOnSoftButtonsCB(int device, int dwButtons, IntPtr pContext);

		virtual int SoftbuttonsChanged(int a, int b, IntPtr c)
		{
			return 0;
		}

	protected: 
		String^ appFriendlyName;
		bool isPersistent;
		bool isAutostartable;
		LcdDeviceType deviceType;
        GCHandle onSoftButtonsGch;

	private:
        bool disposed;
		lgLcdConnectContext* connectContext;
		lgLcdOpenByTypeContext* openContext;
	};
}
