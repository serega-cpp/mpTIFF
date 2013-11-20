using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

public class MyMPTiffApp
{
    [STAThread]
	public static int Main()
	{
        MyMPTiffApp app = new MyMPTiffApp();
        app.DoGdiPlus();
        return 0;
	}

    protected void DoGdiPlus()
    {
        //// Get source files names
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Title = "Specify source files for multi-pages TIFF (at least 2 files)";
        openFileDialog.Filter = "TIFF files (*.tif)|*.tif|All files (*.*)|*.*";
        openFileDialog.Multiselect = true;
        if (openFileDialog.ShowDialog() != DialogResult.OK) 
            return;

        string[] srcFileNames = openFileDialog.FileNames;
        if (srcFileNames.Length < 2) {
            MessageBox.Show("At least 2 files required for conversion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        //// Get destanation file name
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Title = "Specify destination file name";
        saveFileDialog.Filter = "TIFF files (*.tif)|*.tif|All files (*.*)|*.*";
        if (saveFileDialog.ShowDialog() != DialogResult.OK) 
            return;
        
        string destFileName = saveFileDialog.FileName;

        //// Create multipage TIFF file
        ImageCodecInfo tiffCodecInfo = GetEncoderInfo("image/tiff");
        EncoderParameters tiffEncoderParams = new EncoderParameters(1);
        
        // Save first page
        Image gim = Image.FromFile(srcFileNames[0]);
        tiffEncoderParams.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
        gim.Save(destFileName, tiffCodecInfo, tiffEncoderParams);
        
        // Add and Save other pages
        tiffEncoderParams.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
        for (int i = 1; i < srcFileNames.Length; i++) {
            Image img = Image.FromFile(srcFileNames[i]);
            gim.SaveAdd(img, tiffEncoderParams);
        }

        // Finalize file
        tiffEncoderParams.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.Flush);
        gim.SaveAdd(tiffEncoderParams);

        MessageBox.Show("File " + destFileName + " was successfully created!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private static ImageCodecInfo GetEncoderInfo(String mimeType)
    {
        ImageCodecInfo[] encoders;
        encoders = ImageCodecInfo.GetImageEncoders();
        for (int j = 0; j < encoders.Length; ++j) {
            if (encoders[j].MimeType == mimeType)
                return encoders[j];
        }

        return null;
    }
};
