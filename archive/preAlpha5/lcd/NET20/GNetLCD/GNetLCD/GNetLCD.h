// GNetLCD.h

#pragma once

using namespace System;
using namespace System::Drawing;
using namespace System::Drawing::Imaging;
using namespace System::Drawing::Text;
using namespace System::Runtime::InteropServices;

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

            switch (deviceType)
            {
                case LcdDeviceType::LcdDeviceBW:
                    bitmap = gcnew Bitmap(LGLCD_BMP_WIDTH, LGLCD_BMP_HEIGHT);
                    break;

                case LcdDeviceType::LcdDeviceQVGA:
                    bitmap = gcnew Bitmap(LGLCD_QVGA_BMP_WIDTH, LGLCD_QVGA_BMP_HEIGHT);
                    break;
            }

            graphics = Graphics::FromImage(bitmap);
            graphics->TextRenderingHint = TextRenderingHint::SingleBitPerPixelGridFit;
            graphics->Clear(Color::Black);

			lgLcdInit();
		}

		~Lcd()
		{
            disposed = true;
            
            delete bitmap;
            delete graphics;

			Close();
			Disconnect();
			lgLcdDeInit();
		}
        
        property bool IsConnected { bool get() { return connectContext != 0; } }
        property bool IsOpen { bool get() { return openContext != 0; } }
        property Bitmap^ LcdBitmap { Bitmap^ get() { return bitmap; } }
        property Graphics^ LcdGraphics { Graphics^ get() { return graphics; } }

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

        int UpdateBitmap(LcdPriority priority)
        {
            if (disposed) return -1;

            int result = 0;

            BitmapData^ bitdata =  bitmap->LockBits(
                Drawing::Rectangle(0, 0, bitmap->Width, bitmap->Height),
                ImageLockMode::ReadOnly,
                //deviceType == LcdDeviceType::LcdDeviceBW ? 
                PixelFormat::Format32bppRgb
                );

            if (deviceType == LcdDeviceType::LcdDeviceBW) {
				lgLcdBitmap160x43x1 bmp;
				bmp.hdr.Format = LGLCD_BMP_FORMAT_160x43x1;

                for (int y = 0; y < bitdata->Height; y++) {
                    BYTE* row = (BYTE*) (bitdata->Scan0).ToPointer() + (y * bitdata->Stride);
                    for (int x = 0; x < bitdata->Width; x++) {

                        BYTE* p = &row[x*4];
                        BYTE val = p[0] | p[1] | p[2];
                        bmp.pixels[(y * bitdata->Width) + x] =
                            (p[0] | p[1] | p[2]) < 0x80 ?
                                0x00 : 0xff;
                    }
                }

				result = lgLcdUpdateBitmap(openContext->device, &bmp.hdr, LGLCD_SYNC_UPDATE((int)priority));
            } else {
				lgLcdBitmapQVGAx32 bmp;
				bmp.hdr.Format = LGLCD_BMP_FORMAT_QVGAx32;

                // don't have a color device to test this (e.g. G19)
                /*
                for (int y = 0; y < bitdata->Height; y++) {
                    byte* row = (byte*) bitdata->Scan0 + (y * bitdata->Stride);
                    for (int x = 0; x < bitdata->Width; x++) {
                        byte* p = row[x];
                        bmp.pixels[(y * bitdata->Width) + x] = row[x];
                    }
                }
                */

				result = lgLcdUpdateBitmap(openContext->device, &bmp.hdr, LGLCD_SYNC_UPDATE((int)priority));
            }

            bitmap->UnlockBits(bitdata);

            return result;
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

        Graphics^ graphics;
        Bitmap^ bitmap;

	private:
        bool disposed;
		lgLcdConnectContext* connectContext;
		lgLcdOpenByTypeContext* openContext;
	};
}
