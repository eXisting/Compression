using System;
using System.IO;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using Zstd.Extern;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    
    // [DllImport("__Internal")]
    // private static extern int CountLettersInString([MarshalAs(UnmanagedType.LPWStr)]string str);

    void Start()
    {
        
    }
    
    unsafe void SimpleCCode()
    {
        try
        {
            var src = File.ReadAllBytes(Application.streamingAssetsPath + "/dickens");
            var cctx = ExternMethods.ZSTD_createCCtx();
            var dctx = ExternMethods.ZSTD_createDCtx();

            var dest = new byte[ExternMethods.ZSTD_compressBound((nuint)src.Length)];
            var uncompressed = new byte[src.Length];
            fixed (byte* dstPtr = dest)
            fixed (byte* srcPtr = src)
            fixed (byte* uncompressedPtr = uncompressed)
            {
                var compressedLength = ExternMethods.ZSTD_compressCCtx(cctx, (IntPtr)dstPtr, (nuint)dest.Length, (IntPtr) srcPtr, (nuint)src.Length,
                    (int)ExternMethods.ZSTD_cParameter.ZSTD_c_compressionLevel);

                var decompressedLength = ExternMethods.ZSTD_decompressDCtx(dctx, (IntPtr)uncompressedPtr, (nuint) uncompressed.Length, (IntPtr)dstPtr, compressedLength);
                Debug.Log($"{compressedLength} {decompressedLength} {src.Length}");
                label.text = $"{compressedLength} {decompressedLength} {src.Length}";
            }
                ;
            ExternMethods.ZSTD_freeCCtx(cctx);
            ExternMethods.ZSTD_freeDCtx(dctx);
            
        }
        catch (Exception e)
        {
            label.text = e.Message;
            Debug.LogError(e);
        }
    }
}
