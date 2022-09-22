using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;

public partial class _Default : System.Web.UI.Page
{
    #region Private Member Variables
    private static string UPLOADFOLDER = "Uploads";
    #endregion

    #region Web Methods
    protected void Page_Load(object sender, EventArgs args)
    {
        if (!this.IsPostBack)
        {
            //Reserve a spot in Session for the UploadDetail object
            this.Session["UploadDetail"] = new UploadDetail { IsReady = false };
            LoadUploadedFiles(ref gvNewFiles);
        }
    }
    [System.Web.Services.WebMethod]
    [System.Web.Script.Services.ScriptMethod]
    public static object GetUploadStatus()
    {
        //Get the length of the file on disk and divide that by the length of the stream
        UploadDetail info = (UploadDetail)HttpContext.Current.Session["UploadDetail"];
        if (info != null && info.IsReady)
        {
            int soFar = info.UploadedLength;
            int total = info.ContentLength;
            int percentComplete = (int)Math.Ceiling((double)soFar / (double)total * 100);
            string message = "Uploading...";
            string fileName = string.Format("{0}", info.FileName);
            string downloadBytes = string.Format("{0} of {1} Bytes", soFar, total);
            return new
            {
                percentComplete = percentComplete,
                message = message,
                fileName = fileName,
                downloadBytes = downloadBytes
            };
        }
        //Not ready yet
        return null;
    }
    #endregion

    #region Web Callbacks
    protected void gvNewFiles_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "eventMouseOver(this)");
            e.Row.Attributes.Add("onmouseout", "eventMouseOut(this)");
        }
    }
    protected void gvNewFiles_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "deleteFile":
                DeleteFile(e.CommandArgument.ToString());
                LoadUploadedFiles(ref gvNewFiles);
                break;
            case "downloadFile":
                string strFolder = "Uploads";
                string filePath = Path.Combine(strFolder, e.CommandArgument.ToString());
                DownloadFile(filePath);
                break;
        }
    }
    protected void hdRefereshGrid_ValueChanged(object sender, EventArgs e)
    {
        LoadUploadedFiles(ref gvNewFiles);
    }
    #endregion

    #region Support Methods
    public void LoadUploadedFiles(ref GridView gv)
    {
        DataTable dtFiles = GetFilesInDirectory(HttpContext.Current.Server.MapPath(UPLOADFOLDER));
        gv.DataSource = dtFiles;
        gv.DataBind();
        if (dtFiles != null && dtFiles.Rows.Count > 0)
        {
            double totalSize = Convert.ToDouble(dtFiles.Compute("SUM(Size)", ""));
            if (totalSize > 0) lblTotalSize.Text = CalculateFileSize(totalSize);
        }
    }
    public DataTable GetFilesInDirectory(string sourcePath)
    {
        System.Data.DataTable dtFiles = new System.Data.DataTable();
        if ((Directory.Exists(sourcePath)))
        {
            dtFiles.Columns.Add(new System.Data.DataColumn("Name"));
            dtFiles.Columns.Add(new System.Data.DataColumn("Size"));
            dtFiles.Columns["Size"].DataType = typeof(double);
            dtFiles.Columns.Add(new System.Data.DataColumn("ConvertedSize"));
            DirectoryInfo dir = new DirectoryInfo(sourcePath);
            foreach (FileInfo files in dir.GetFiles("*.*"))
            {
                System.Data.DataRow drFile = dtFiles.NewRow();
                drFile["Name"] = files.Name;
                drFile["Size"] = files.Length;
                drFile["ConvertedSize"] = CalculateFileSize(files.Length);
                dtFiles.Rows.Add(drFile);
            }
        }
        return dtFiles;
    }
    public void DownloadFile(string filePath)
    {
        if (File.Exists(Server.MapPath(filePath)))
        {
            string strFileName = Path.GetFileName(filePath).Replace(" ", "%20");
           // Response.ContentType = "application/octet-stream";
           // Response.AddHeader("Content-Disposition", "attachment; filename=" + strFileName);
           // Response.Clear();
           // Response.WriteFile(Server.MapPath(filePath));
          //  Response.End();

            string strRootRelativePathName =
                string.Format(filePath, strFileName);

            // string strPathName =
            // Server.MapPath("C:\\Users\\Bala\\SPFX\\Uploads\\Temp\\Testfile.mp4");

            // **************************************************

            System.IO.Stream oStream = null;

            try
            {
                // Open the file
                oStream =
                    new System.IO.FileStream
                        (path: Server.MapPath(filePath),
                        mode: System.IO.FileMode.Open,
                        share: System.IO.FileShare.Read,
                        access: System.IO.FileAccess.Read);

                // **************************************************
                Response.Buffer = false;

                // Setting the unknown [ContentType]
                // will display the saving dialog for the user
                Response.ContentType = "application/octet-stream";

                // With setting the file name,
                // in the saving dialog, user will see
                // the [strFileName] name instead of [download]!
                Response.AddHeader("Content-Disposition", "attachment; filename=" + strFileName);

                long lngFileLength = oStream.Length;

                // Notify user (client) the total file length
                Response.AddHeader("Content-Length", lngFileLength.ToString());
                // **************************************************

                // Total bytes that should be read
                long lngDataToRead = lngFileLength;

                // Read the bytes of file
                while (lngDataToRead > 0)
                {
                    // The below code is just for testing! So we commented it!
                    //System.Threading.Thread.Sleep(200);

                    // Verify that the client is connected or not?
                    if (Response.IsClientConnected)
                    {
                        // 8KB
                        int intBufferSize = 8 * 1024;

                        // Create buffer for reading [intBufferSize] bytes from file
                        byte[] bytBuffers =
                            new System.Byte[intBufferSize];

                        // Read the data and put it in the buffer.
                        int intTheBytesThatReallyHasBeenReadFromTheStream =
                            oStream.Read(buffer: bytBuffers, offset: 0, count: intBufferSize);

                        // Write the data from buffer to the current output stream.
                        Response.OutputStream.Write
                            (buffer: bytBuffers, offset: 0,
                            count: intTheBytesThatReallyHasBeenReadFromTheStream);

                        // Flush (Send) the data to output
                        // (Don't buffer in server's RAM!)
                        Response.Flush();

                        lngDataToRead =
                            lngDataToRead - intTheBytesThatReallyHasBeenReadFromTheStream;
                    }
                    else
                    {
                        // Prevent infinite loop if user disconnected!
                        lngDataToRead = -1;
                    }
                }
            }
            catch { }
            finally
            {
                if (oStream != null)
                {
                    //Close the file.
                    oStream.Close();
                    oStream.Dispose();
                    oStream = null;
                }
                Response.Close();
            }
        }
    }
    public string DeleteFile(string FileName)
    {
        string strMessage = "";
        try
        {
            string strPath = Path.Combine(UPLOADFOLDER, FileName);
            if (File.Exists(Server.MapPath(strPath)) == true)
            {
                File.Delete(Server.MapPath(strPath));
                strMessage = "File Deleted";
            }
            else
                strMessage = "File Not Found";
        }
        catch (Exception ex)
        {
            strMessage = ex.Message;
        }
        return strMessage;
    }
    public string CalculateFileSize(double FileInBytes)
    {
        string strSize = "00";
        if (FileInBytes < 1024)
            strSize = FileInBytes + " B";//Byte
        else if (FileInBytes > 1024 & FileInBytes < 1048576)
            strSize = Math.Round((FileInBytes / 1024), 2) + " KB";//Kilobyte
        else if (FileInBytes > 1048576 & FileInBytes < 107341824)
            strSize = Math.Round((FileInBytes / 1024) / 1024, 2) + " MB";//Megabyte
        else if (FileInBytes > 107341824 & FileInBytes < 1099511627776)
            strSize = Math.Round(((FileInBytes / 1024) / 1024) / 1024, 2) + " GB";//Gigabyte
        else
            strSize = Math.Round((((FileInBytes / 1024) / 1024) / 1024) / 1024, 2) + " TB";//Terabyte
        return strSize;
    }
    #endregion
}
