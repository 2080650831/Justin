﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Justin.FrameWork.WinForm.Helper
{
    public class FileHelper
    {
        public static void OverWrite(string fileName,string content)
        {
            StreamWriter sw = new StreamWriter(fileName);
            sw.Write(content);
            sw.Close();
        }

        [DllImport("Shell32.dll")]
        static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, ref   SHFILEINFO psfi, uint cbFileInfo, uint uFlags);
        struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            public char szDisplayName;
            public char szTypeName;
        }
        /// <summary>
        /// 从文件扩展名得到文件关联图标
        /// </summary>
        /// <param name="fileName">文件名或文件扩展名</param>
        /// <param name="smallIcon">是否是获取小图标，否则是大图标</param>
        /// <returns>图标</returns>
        static public Icon GetFileIcon(string fileName, bool smallIcon)
        {
            SHFILEINFO fi = new SHFILEINFO();
            Icon ic = null;
            //SHGFI_ICON + SHGFI_USEFILEATTRIBUTES + SmallIcon   
            int iTotal = (int)SHGetFileInfo(fileName, 100, ref fi, 0, (uint)(smallIcon ? 273 : 272));
            if (iTotal > 0)
            {
                ic = Icon.FromHandle(fi.hIcon);
            }
            return ic;
        }
    }
}
