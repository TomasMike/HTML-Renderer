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

using PdfSharp;
using PdfSharp.Drawing;

namespace TheArtOfDev.HtmlRenderer.PdfSharp
{
	/// <summary>
	/// The settings for generating PDF using <see cref="PdfGenerator"/>
	/// </summary>
	public sealed class PdfGenerateConfig
	{
		#region Fields/Consts

		/// <summary>
		/// the page size to use for each page in the generated pdf
		/// </summary>
		private PageSize _pageSize;

		/// <summary>
		/// if the page size is undefined this allow you to set manually the page size
		/// </summary>
		private XSize _xsize;

		/// <summary>
		/// the orientation of each page of the generated pdf
		/// </summary>
		private PageOrientation _pageOrientation;

		/// <summary>
		/// the top margin between the page start and the text
		/// </summary>
		private int _marginTop;

		/// <summary>
		/// the bottom margin between the page end and the text
		/// </summary>
		private int _marginBottom;

		/// <summary>
		/// the left margin between the page start and the text
		/// </summary>
		private int _marginLeft;

		/// <summary>
		/// the right margin between the page end and the text
		/// </summary>
		private int _marginRight;

		/// <summary>
		/// Whether to display page numbers in footer area
		/// </summary>
		private bool _enablePageNumbering;

		/// <summary>
		/// Where to display the page number
		/// </summary>
		private PageNumberLocation _pageNumberLocation;

		/// <summary>
		/// Font which will be used in the page numbers text
		/// </summary>
		private XFont _pageNumbersFont;

		/// <summary>
		/// Pattern from which to generate the page numbers text. Use {0} for current page and {1} for number of total pages, e.g. "{0}/{1}" will generate "1/21" 
		/// </summary>
		private string _pageNumbersPattern;
		/// <summary>
		/// Margin from the bottom of the page edge 
		/// </summary>
		private double _pageNumbersMarginBottom;
		/// <summary>
		/// Margin from the left/right of the page edge (when the page number is on the left size, it will be the left margin and vice versa.
		/// If the number is in the middle, this value will be ignored.
		/// </summary>
		private double _pageNumbersLeftRightMargin;
		#endregion


		/// <summary>
		/// the page size to use for each page in the generated pdf
		/// </summary>
		public PageSize PageSize
		{
			get { return _pageSize; }
			set { _pageSize = value; }
		}

		/// <summary>
		/// if the page size is undefined this allow you to set manually the page size
		/// </summary>
		public XSize ManualPageSize {
			get { return _xsize; }
			set { _xsize = value; }
		}

		/// <summary>
		/// the orientation of each page of the generated pdf
		/// </summary>
		public PageOrientation PageOrientation
		{
			get { return _pageOrientation; }
			set { _pageOrientation = value; }
		}

		/// <summary>
		/// the top margin between the page start and the text
		/// </summary>
		public int MarginTop
		{
			get { return _marginTop; }
			set
			{
				if (value > -1)
					_marginTop = value;
			}
		}

		/// <summary>
		/// the bottom margin between the page end and the text
		/// </summary>
		public int MarginBottom
		{
			get { return _marginBottom; }
			set
			{
				if (value > -1)
					_marginBottom = value;
			}
		}

		/// <summary>
		/// the left margin between the page start and the text
		/// </summary>
		public int MarginLeft
		{
			get { return _marginLeft; }
			set
			{
				if (value > -1)
					_marginLeft = value;
			}
		}

		/// <summary>
		/// the right margin between the page end and the text
		/// </summary>
		public int MarginRight
		{
			get { return _marginRight; }
			set
			{
				if (value > -1)
					_marginRight = value;
			}
		}

		/// <summary>
		/// Whether to display page numbers in footer area
		/// </summary>
		public bool EnablePageNumbering
		{
			get { return _enablePageNumbering; }
			set { _enablePageNumbering = value; }
		}

		/// <summary>
		/// Whether to display on the left side of the page or not(display it on the right side)
		/// </summary>
		public PageNumberLocation PageNumberLocation
		{
			get { return _pageNumberLocation; }
			set { _pageNumberLocation = value; }
		}

		/// <summary>
		/// Font which will be used in the page numbers text
		/// </summary>
		public XFont PageNumbersFont
		{
			get { return _pageNumbersFont ?? new XFont("Times New Roman", 11); }
			set { _pageNumbersFont = value; }
		}

		/// <summary>
		/// Pattern from which to generate the page numbers text. Use {0} for current page and {1} for number of total pages, e.g. "{0}/{1}" will generate "1/21" 
		/// </summary>
		public string PageNumbersPattern
		{
			get
			{
				return _pageNumbersPattern ?? "{0}/{1}";
			}
			set { _pageNumbersPattern = value; }
		}



		//Margin from the bottom of the page edge 
		public double PageNumbersMarginBottom
		{
			get { return _pageNumbersMarginBottom; }
			set { _pageNumbersMarginBottom = value; }
		}

		//Margin from the left/right of the page edge (when the page number is on the left size, it will be the left margin and vice versa.
		//If the number is in the middle, this value will be ignored.
		public double PageNumbersLeftRightMargin
		{
			get { return _pageNumbersLeftRightMargin; }
			set { _pageNumbersLeftRightMargin = value; }
		}



		/// <summary>
		/// Set all 4 margins to the given value.
		/// </summary>
		/// <param name="value"></param>
		public void SetMargins(int value)
		{
			if (value > -1)
				_marginBottom = _marginLeft = _marginTop = _marginRight = value;
		}

		// The international definitions are:
		//   1 inch == 25.4 mm
		//   1 inch == 72 point

		/// <summary>
		/// Convert the units passed in milimiters to the units used in PdfSharp
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static XSize MilimitersToUnits(double width, double height) {
			return new XSize(width / 25.4 * 72, height / 25.4 * 72);
		}

		/// <summary>
		/// Convert the units passed in inches to the units used in PdfSharp
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static XSize InchesToUnits(double width, double height) {
			return new XSize(width * 72, height * 72);
		}
	}

	public enum PageNumberLocation
	{
		Right,Left,Middle
	}
}