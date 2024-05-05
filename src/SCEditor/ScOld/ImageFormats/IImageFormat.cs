using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SCEditor.ScOld.ImageFormats;

public interface IImageFormat
{
    // static abstract void ReadImage(ImageEncoding encoding, bool isKtx, int width, int height, BinaryReader br);
    static abstract void ReadColor(ReadOnlySpan<byte> span, out int color);
    static abstract void WriteColor(Span<byte> span, int color);
    static abstract int PixelSize { get; }
    static abstract string Name { get; }
}

public class ImageFormatRgba8888 : IImageFormat
{
    public static void ReadColor(ReadOnlySpan<byte> span, out int color)
    {
        int rawValue = Unsafe.As<byte, int>(ref MemoryMarshal.GetReference(span));
        byte r = (byte) (rawValue >> 0);
        byte g = (byte) (rawValue >> 8);
        byte b = (byte) (rawValue >> 16);
        byte a = (byte) (rawValue >> 24);
        
        color = (a << 24) | (r << 16) | (g << 8) | (b << 0);
    }

    public static void WriteColor(Span<byte> span, int color)
    {
        byte r = (byte) (color >> 16);
        byte g = (byte) (color >> 8);
        byte b = (byte) (color >> 0);
        byte a = (byte) (color >> 24);
        if (a == 0)
        {
            r = 0;
            g = 0;
            b = 0;
        }

        int rawValue = (a << 24) | (r << 16) | (g << 8) | (b << 0);
        
        Unsafe.As<byte, int>(ref MemoryMarshal.GetReference(span)) = rawValue;
    }
    
    public static int PixelSize => 4;

    public static string Name => "RGBA8888";
}

// Rgb565
public class ImageFormatRgb565 : IImageFormat
{
    public static void ReadColor(ReadOnlySpan<byte> span, out int color)
    {
        ushort rawValue = Unsafe.As<byte, ushort>(ref MemoryMarshal.GetReference(span));
        int r = ((rawValue >> 11) & 0x1F) << 3;
        int g = ((rawValue >> 5) & 0x3F) << 2;
        int b = (rawValue & 0x1F) << 3;
        
        color = (r << 16) | (g << 8) | (b << 0) | (0xFF << 24);
    }

    public static void WriteColor(Span<byte> span, int color)
    {
        byte r = (byte) (color >> 16);
        byte g = (byte) (color >> 8);
        byte b = (byte) (color >> 0);
        byte a = (byte) (color >> 24);
        if (a == 0)
        {
            r = 0;
            g = 0;
            b = 0;
        }
        
        ushort rawValue = (ushort) (((r & 0xF8) << 8) | ((g & 0xFC) << 3) | ((b & 0xF8) >> 3));
        Unsafe.As<byte, ushort>(ref MemoryMarshal.GetReference(span)) = rawValue;
    }
    
    public static int PixelSize => 2;

    public static string Name => "RGB565";
}

// Rgba4444
public class ImageFormatRgba4444 : IImageFormat
{
    public static void ReadColor(ReadOnlySpan<byte> span, out int color)
    {
        ushort rawValue = Unsafe.As<byte, ushort>(ref MemoryMarshal.GetReference(span));
        int r = ((rawValue >> 12) & 0xF) << 4;
        int g = ((rawValue >> 8) & 0xF) << 4;
        int b = ((rawValue >> 4) & 0xF) << 4;
        int a = (rawValue & 0xF) << 4;
        
        color = (a << 24) | (r << 16) | (g << 8) | (b << 0);
    }

    public static void WriteColor(Span<byte> span, int color)
    {
        byte r = (byte) (color >> 16);
        byte g = (byte) (color >> 8);
        byte b = (byte) (color >> 0);
        byte a = (byte) (color >> 24);
        if (a == 0)
        {
            r = 0;
            g = 0;
            b = 0;
        }
        
        ushort rawValue = (ushort) (((a & 0xF0) << 8) | ((r & 0xF0) << 4) | ((g & 0xF0) << 0) | ((b & 0xF0) >> 4));
        Unsafe.As<byte, ushort>(ref MemoryMarshal.GetReference(span)) = rawValue;
    }
    
    public static int PixelSize => 2;

    public static string Name => "RGBA4444";
}

// Rgba5551
public class ImageFormatRgba5551 : IImageFormat
{
    public static void ReadColor(ReadOnlySpan<byte> span, out int color)
    {
        ushort rawValue = Unsafe.As<byte, ushort>(ref MemoryMarshal.GetReference(span));
        int r = (((rawValue >> 11) & 0x1F) + 15) / 31;
        int g = (((rawValue >> 6) & 0x1F) + 15) / 31;
        int b = (((rawValue >> 1) & 0x1F) + 15) / 31;
        int a = (rawValue & 0x1) * 255;

        color = (a << 24) | (r << 16) | (g << 8) | (b << 0);
    }

    public static void WriteColor(Span<byte> span, int color)
    {
        byte r = (byte)(color >> 16);
        byte g = (byte)(color >> 8);
        byte b = (byte)(color >> 0);
        byte a = (byte)(color >> 24);
        if (a == 0)
        {
            r = 0;
            g = 0;
            b = 0;
        }

        byte r5 = (byte)(r * 31 / 255);
        byte g5 = (byte)(g * 31 / 255);
        byte b5 = (byte)(b * 31 / 255);
        byte a1 = (byte)(a > 0 ? 1 : 0);

        ushort rawValue = (ushort)((a1 << 0) | (r5 << 11) | (g5 << 6) | (b5 << 1));
        Unsafe.As<byte, ushort>(ref MemoryMarshal.GetReference(span)) = rawValue;
    }
    
    public static int PixelSize => 2;
    public static string Name => "RGBA5551";
}

// Lum8
public class ImageFormatLum8 : IImageFormat
{
    public static void ReadColor(ReadOnlySpan<byte> span, out int color)
    {
        byte rawValue = Unsafe.As<byte, byte>(ref MemoryMarshal.GetReference(span));
        int r = rawValue;
        int g = rawValue;
        int b = rawValue;
        int a = 255;

        color = (a << 24) | (r << 16) | (g << 8) | (b << 0);
    }

    public static void WriteColor(Span<byte> span, int color)
    {
        byte r = (byte)(color >> 16);
        byte g = (byte)(color >> 8);
        byte b = (byte)(color >> 0);
        byte a = (byte)(color >> 24);
        if (a == 0)
        {
            r = 0;
            g = 0;
            b = 0;
        }

        byte rawValue = (byte)((r + g + b) / 3);
        Unsafe.As<byte, byte>(ref MemoryMarshal.GetReference(span)) = rawValue;
    }
    
    public static int PixelSize => 1;
    public static string Name => "LUM8";
}

// LumA88
public class ImageFormatLumA88 : IImageFormat
{
    public static void ReadColor(ReadOnlySpan<byte> span, out int color)
    {
        byte rawValue = Unsafe.As<byte, byte>(ref MemoryMarshal.GetReference(span));
        int r = rawValue;
        int g = rawValue;
        int b = rawValue;
        int a = Unsafe.As<byte, byte>(ref MemoryMarshal.GetReference(span.Slice(1)));

        color = (a << 24) | (r << 16) | (g << 8) | (b << 0);
    }

    public static void WriteColor(Span<byte> span, int color)
    {
        byte r = (byte)(color >> 16);
        byte g = (byte)(color >> 8);
        byte b = (byte)(color >> 0);
        byte a = (byte)(color >> 24);
        if (a == 0)
        {
            r = 0;
            g = 0;
            b = 0;
        }

        byte rawValue = (byte)((r + g + b) / 3);
        Unsafe.As<byte, byte>(ref MemoryMarshal.GetReference(span)) = rawValue;
        Unsafe.As<byte, byte>(ref MemoryMarshal.GetReference(span.Slice(1))) = a;
    }
    
    public static int PixelSize => 2;
    public static string Name => "LUMA88";
}