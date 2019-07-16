// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using TheArtOfDev.HtmlRenderer.Demo.Common;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using TheArtOfDev.HtmlRenderer.WinForms;
using PdfSharp;
using PdfSharp.Drawing;
using Saxon.Api;

namespace TheArtOfDev.HtmlRenderer.Demo.WinForms
{
	public partial class DemoForm : Form
	{
		#region Fields/Consts

		/// <summary>
		/// the private font used for the demo
		/// </summary>
		private readonly PrivateFontCollection _privateFont = new PrivateFontCollection();

		#endregion


		/// <summary>
		/// Init.
		/// </summary>
		public DemoForm()
		{
			SamplesLoader.Init(HtmlRenderingHelper.IsRunningOnMono() ? "Mono" : "WinForms", typeof(HtmlRender).Assembly.GetName().Version.ToString());

			InitializeComponent();

			Icon = GetIcon();
			_openSampleFormTSB.Image = Common.Properties.Resources.form;
			_showIEViewTSSB.Image = Common.Properties.Resources.browser;
			_openInExternalViewTSB.Image = Common.Properties.Resources.chrome;
			_useGeneratedHtmlTSB.Image = Common.Properties.Resources.code;
			_generateImageSTB.Image = Common.Properties.Resources.image;
			_generatePdfTSB.Image = Common.Properties.Resources.pdf;
			_runPerformanceTSB.Image = Common.Properties.Resources.stopwatch;

			StartPosition = FormStartPosition.CenterScreen;
			var size = Screen.GetWorkingArea(Point.Empty);
			Size = new Size((int)(size.Width * 0.7), (int)(size.Height * 0.8));

			LoadCustomFonts();

			_showIEViewTSSB.Enabled = !HtmlRenderingHelper.IsRunningOnMono();
			_generatePdfTSB.Enabled = !HtmlRenderingHelper.IsRunningOnMono();
		}

		/// <summary>
		/// Load custom fonts to be used by renderer HTMLs
		/// </summary>
		private void LoadCustomFonts()
		{
			// load custom font font into private fonts collection
			var file = Path.GetTempFileName();
			File.WriteAllBytes(file, Resources.CustomFont);
			_privateFont.AddFontFile(file);

			// add the fonts to renderer
			foreach (var fontFamily in _privateFont.Families)
				HtmlRender.AddFontFamily(fontFamily);
		}

		/// <summary>
		/// Get icon for the demo.
		/// </summary>
		internal static Icon GetIcon()
		{
			var stream = typeof(DemoForm).Assembly.GetManifestResourceStream("TheArtOfDev.HtmlRenderer.Demo.WinForms.html.ico");
			return stream != null ? new Icon(stream) : null;
		}

		private void OnOpenSampleForm_Click(object sender, EventArgs e)
		{
			using (var f = new SampleForm())
			{
				f.ShowDialog();
			}
		}

		/// <summary>
		/// Toggle if to show split view of HtmlPanel and WinForms WebBrowser control.
		/// </summary>
		private void OnShowIEView_ButtonClick(object sender, EventArgs e)
		{
			_showIEViewTSSB.Checked = !_showIEViewTSSB.Checked;
			_mainControl.ShowWebBrowserView(_showIEViewTSSB.Checked);
		}

		/// <summary>
		/// Open the current html is external process - the default user browser.
		/// </summary>
		private void OnOpenInExternalView_Click(object sender, EventArgs e)
		{
			var tmpFile = Path.ChangeExtension(Path.GetTempFileName(), ".htm");
			File.WriteAllText(tmpFile, _mainControl.GetHtml());
			Process.Start(tmpFile);
		}

		/// <summary>
		/// Toggle the use generated html button state.
		/// </summary>
		private void OnUseGeneratedHtml_Click(object sender, EventArgs e)
		{
			_useGeneratedHtmlTSB.Checked = !_useGeneratedHtmlTSB.Checked;
			_mainControl.UseGeneratedHtml = _useGeneratedHtmlTSB.Checked;
			_mainControl.UpdateWebBrowserHtml();
		}

		/// <summary>
		/// Open generate image form for the current html.
		/// </summary>
		private void OnGenerateImage_Click(object sender, EventArgs e)
		{
			using (var f = new GenerateImageForm(_mainControl.GetHtml()))
			{
				f.ShowDialog();
			}
		}

		/// <summary>
		/// Create PDF using PdfSharp project, save to file and open that file.
		/// </summary>
		private void OnGeneratePdf_Click(object sender, EventArgs e)
		{

			string outPDFPath = @"C:\Users\tmi\Downloads\obnova_kulturnych_pamiatok.1.0 (2)TMI (2).pdf";
			string htmlstring = generateHtml();
			if (!string.IsNullOrEmpty(htmlstring))
			{
				PdfGenerateConfig x = new PdfGenerateConfig();
				x.EnablePageNumbering = true;
				x.PageNumbersFont = new XFont("Times New Roman", 11);
				x.PageNumbersPattern = "{0}/{1}";
				x.PageNumberLocation = PageNumberLocation.Middle;
				x.PageNumbersMarginBottom = 20;
				x.PageNumbersLeftRightMargin = 10;
				x.PageSize = PageSize.A4;
				x.SetMargins(30);
				var pdf = PdfGenerator.GeneratePdf(htmlstring, x, null, null, null);

				using (FileStream fs = new FileStream(outPDFPath, FileMode.Create))
				{
					pdf.Save(fs, true);
				}

				System.Diagnostics.Process.Start(outPDFPath);
			}
		}

		private string generateHtml()
		{
			string outPDFPath = @"C:\Users\tmi\Downloads\obnova_kulturnych_pamiatok.1.0 (2)TMI (2).pdf";

			//typ 1
			string xsltPath =
				@"C:\Projects\Formulare.definicie\MKU\obnova_kulturnych_pamiatok\1.0\UPVS\obnova_kulturnych_pamiatok.1.0.pdf.xsl";
			string xslNamespace = "http://schemas.gov.sk/form/obnova_kulturnych_pamiatok/1.0";

			//typ 2
			xsltPath = @"C:\Projects\Formulare.definicie\MKU\ziva_kultura\1.0\UPVS\ziva_kultura.1.0.pdf.xsl";
			xslNamespace =
				"http://schemas.gov.sk/form/neformalne_vzdelavanie_v_oblasti_kultury_osob_so_zdravotnym_postihnutim/1.0";
			string xmlPath = string.Empty;
			List<Tuple<string, DateTime>> q = new List<Tuple<string, DateTime>>();

			foreach (var v in Directory.GetFiles(@"C:\Users\tmi\Downloads\", "*export*"))
			{
				q.Add(new Tuple<string, DateTime>(v, File.GetCreationTime(v)));
			}

			q.RemoveAll(f => !f.Item1.EndsWith("xml"));
			xmlPath = q.OrderBy(a => a.Item2).Last().Item1;

			XmlDocument xml = new XmlDocument();
			xml.Load(File.OpenRead(xmlPath));
			var nsmgr = new XmlNamespaceManager(xml.NameTable);
			nsmgr.AddNamespace("f",xslNamespace );
			string x = "";
			using (MemoryStream outstream = new MemoryStream())
			{
				using (FileStream fs1 = new FileStream(xsltPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					using (FileStream fsxml =
						new FileStream(xmlPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						TransformXMLToHTML(fsxml, fs1, outstream);
					}
				}

				x = Encoding.UTF8.GetString(outstream.ToArray());
			}

			return x;
		}

		public static string TransformXMLToHTML(Stream input, Stream transformXsl, Stream output)
		{
			try
			{
				Processor xsltProcessor = new Processor();
				XsltCompiler xsltCompiler = xsltProcessor.NewXsltCompiler();
				XsltExecutable xsltExecutable = xsltCompiler.Compile(transformXsl);

				var xsltTransformer = xsltExecutable.Load();
				XmlTextReader modelFileXML = new XmlTextReader(input);
				XdmNode xdmNode = xsltProcessor.NewDocumentBuilder().Build(modelFileXML);
				xsltTransformer.InitialContextNode = xdmNode;

				Serializer serializer = xsltProcessor.NewSerializer();
				serializer.SetOutputStream(output);

				xsltTransformer.Run(serializer);
				return "";

			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message + "\r\n" + e.StackTrace);
				return string.Empty;
			}
		}

		/// <summary>
		/// Execute performance test by setting all sample HTMLs in a loop.
		/// </summary>
		private void OnRunPerformance_Click(object sender, EventArgs e)
		{
			_mainControl.UpdateLock = true;
			_toolStrip.Enabled = false;
			Application.DoEvents();

			var msg = DemoUtils.RunSamplesPerformanceTest(html =>
			{
				_mainControl.SetHtml(html);
				Application.DoEvents(); // so paint will be called
			});

			Clipboard.SetDataObject(msg);
			MessageBox.Show(msg, "Test run results");

			_mainControl.UpdateLock = false;
			_toolStrip.Enabled = true;
		}
	}
}