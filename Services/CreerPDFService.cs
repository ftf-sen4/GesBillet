using Syncfusion.Pdf;
using Syncfusion.Drawing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Gestions_des_Titres_de_Transport.Models;

namespace Gestions_des_Titres_de_Transport.Services
{
    public class CreerPDFService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public CreerPDFService(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public MemoryStream CreatePDF()
        {
            PdfDocument document = new PdfDocument();
            document.PageSettings.Size = PdfPageSize.A4;
            document.PageSettings.Margins.Top = 0;
            document.PageSettings.Margins.Bottom = 0;
            document.PageSettings.Margins.Left = 0;
            document.PageSettings.Margins.Right = 0;


            PdfPage page = document.Pages.Add();

            PdfGraphics graphics = page.Graphics;

            PdfPen pen = new PdfPen(PdfBrushes.Black, 1);

            RectangleF bounds = new RectangleF(15, 15, 565, 785);

            graphics.DrawRectangle(pen,bounds);



            PdfStandardFont texte9 = new PdfStandardFont(PdfFontFamily.Helvetica, 9, PdfFontStyle.Bold);
            PdfStandardFont texte14 = new PdfStandardFont(PdfFontFamily.Helvetica, 14);
            PdfStandardFont texte16 = new PdfStandardFont(PdfFontFamily.Helvetica, 16);
            PdfStandardFont texte24 = new PdfStandardFont(PdfFontFamily.Helvetica, 24);

            graphics.DrawString("BON DE PASSAGE\nen compte", texte16, PdfBrushes.Black,new PointF(228, 30));
            graphics.DrawString("N° 00000000001", texte16, PdfBrushes.Black,new PointF(425, 29));
            graphics.DrawString("68 Avenue de la Libération\nB.P 1172 Lomé Togo\nTél. : 00228 22 21 42 44\nE-mail : boadsiege@boad.org", texte9, PdfBrushes.Black,new PointF(76, 34));

            FileStream imageStream = new FileStream(_hostingEnvironment.WebRootPath + "//logo.png", FileMode.Open, FileAccess.Read);
            PdfBitmap image = new PdfBitmap(imageStream);

            graphics.DrawImage(image, new PointF(25, 25), new SizeF(46, 64));



            MemoryStream stream = new MemoryStream();

            document.Save(stream);
            //Closing the PDF document.
            document.Close(true);
            return stream;
        }
  //      public MemoryStream ImprimerBonDePassage(ImprimerBonModel Model)
  //      {
  //          var NomSignataire = Model.NomSignataire.ToUpper();
  //          var DateDeSaisie = Model.DateSaisie.Value.ToString("dd/MM/yyyy");
  //          var IDBonDePassage = Model.IDBonDePassage.ToString("D11");
  //          var FonctionSignataire = Model.FonctionSignataire.ToUpper();

		//	PdfDocument document = new PdfDocument();
		//	document.PageSettings.Size = PdfPageSize.A4;

		//	document.PageSettings.Margins.Top = 0;
		//	document.PageSettings.Margins.Bottom = 0;
		//	document.PageSettings.Margins.Left = 0;
		//	document.PageSettings.Margins.Right = 0;

		//	PdfPage page = document.Pages.Add();
		//	PdfGraphics graphics = page.Graphics;
		//	PdfPen pen = new PdfPen(PdfBrushes.Black, 1);

		//	PdfColor BOADGray = new PdfColor(68, 68, 68);
  //          PdfSolidBrush BOADGrayBrush = new PdfSolidBrush(BOADGray);

		//	PdfColor BOADLightGray = new PdfColor(239, 239, 239);
  //          PdfSolidBrush BOADLightGrayBrush = new PdfSolidBrush(BOADLightGray);

		//	PdfColor BOADRed = new PdfColor(173, 23, 44);
		//	PdfSolidBrush BOADRedBrush = new PdfSolidBrush(BOADRed);

		//	PdfColor BOADBlack = new PdfColor(0, 0, 0);
		//	PdfSolidBrush BOADBlackBrush = new PdfSolidBrush(BOADBlack);


		//	PdfStandardFont Texte9Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 9, PdfFontStyle.Regular);
		//	PdfStandardFont Texte10Bold = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
		//	PdfStandardFont Texte10Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Regular);
		//	PdfStandardFont Texte12Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Regular);
		//	PdfStandardFont Texte13Bold = new PdfStandardFont(PdfFontFamily.Helvetica, 13, PdfFontStyle.Bold);
		//	PdfStandardFont Texte13Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 13, PdfFontStyle.Regular);
		//	PdfStandardFont Texte13RegularUnderlined = new PdfStandardFont(PdfFontFamily.Helvetica, 13, PdfFontStyle.Underline);
		//	PdfStandardFont Texte14Bold = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
		//	PdfStandardFont Texte16Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Regular);


  //          graphics.DrawString("BON DE PASSAGE EN COMPTE", Texte13Bold, BOADGrayBrush, new PointF(30, 30));

		//	graphics.DrawString($"N° {IDBonDePassage}", Texte16Regular, BOADRedBrush, new PointF(30, 56));

		//	FileStream imageStream = new FileStream(_hostingEnvironment.WebRootPath + "//logo.png", FileMode.Open, FileAccess.Read);
		//	PdfBitmap LogoBOAD = new PdfBitmap(imageStream);
		//	graphics.DrawImage(LogoBOAD, new PointF(398, 34), new SizeF(46, 64));


		//	graphics.DrawString("68 Avenue de la Libération\nB.P 1172 Lomé Togo\nTél. : 00228 22 21 42 44\nE-mail : boadsiege@boad.org", Texte9Regular, BOADGrayBrush, new PointF(447, 38));
		//	graphics.DrawString("Joindre l'original de ce bon à la facture correspondante", Texte10Bold, BOADGrayBrush, new PointF(30, 808));


		//	graphics.DrawString($"Nous prions {Model.LibelleAgence} de bien vouloir DELIVRER contre le présent bon\npour le compte de la BOAD {Model.NombreDeBillets} titre(s) de transport au nom de :", Texte13Regular, BOADBlackBrush, new PointF(30, 146));

  //          PointF CurseurListePosition = new PointF(30, 204);

  //          var Num = 1;
  //          foreach (var item in Model.ListeLigne)
  //          {
  //              graphics.DrawString($"{Num}. {item.NomPassager}", Texte13Bold, BOADBlackBrush, CurseurListePosition);
		//		CurseurListePosition.Y += 18;
		//		Num++;
		//	}

  //          //CurseurListePosition.Y = 66;
  //          CurseurListePosition.Y = 342;
		//	graphics.DrawString("Pour les parcours respectifs suivants :", Texte13Regular, BOADBlackBrush, CurseurListePosition);
		//	CurseurListePosition.Y += 46;


  //          PdfGrid Grille = new PdfGrid();
  //          Grille.Style.Font = Texte14Bold;

  //          Grille.Columns.Add(6);
  //          Grille.Columns[0].Width = 20;
  //          Grille.Columns[1].Width = 70;
  //          Grille.Columns[2].Width = 70;
		//	Grille.Columns[3].Width = 70;
  //          Grille.Columns[4].Width = 230;
  //          Grille.Columns[5].Width = 65;

  //          Grille.Headers.Add(1);
		//	PdfStringFormat StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
  //          PdfGridRow Header = Grille.Headers[0];

  //          Header.Cells[0].Value = "N°";
  //          Header.Cells[0].StringFormat.LineAlignment = PdfVerticalAlignment.Middle;
  //          Header.Cells[1].Value = "Date Départ";
  //          Header.Cells[1].StringFormat = StringFormat;

		//	Header.Cells[2].Value = "Date Retour";
		//	Header.Cells[2].StringFormat = StringFormat;

		//	Header.Cells[3].Value = "Classe";
		//	Header.Cells[3].StringFormat = StringFormat;
		//	Header.Cells[4].Value = "Itinéraires";
		//	Header.Cells[4].StringFormat = StringFormat;
		//	Header.Cells[5].Value = "Montant";
		//	Header.Cells[5].StringFormat = StringFormat;

  //          PdfGridCellStyle StyleCellule = new PdfGridCellStyle();
  //          StyleCellule.Borders.All = PdfPens.Transparent;
  //          StyleCellule.TextBrush = BOADBlackBrush;
  //          StyleCellule.BackgroundBrush = new PdfSolidBrush(Color.White);

  //          for (int j = 0; j < Header.Cells.Count; j++)
  //          {
  //              PdfGridCell Cellule = Header.Cells[j];
  //              Cellule.Style = StyleCellule;
  //          }

		//	Num = 1;
		//	foreach (var item in Model.ListeLigne)
		//	{
		//		PdfGridRow Ligne = Grille.Rows.Add();
		//		Ligne.Cells[0].Value = Num.ToString();
		//		Ligne.Cells[0].StringFormat.LineAlignment = PdfVerticalAlignment.Middle;

		//		Ligne.Cells[1].Value = item.DateDepart.Value.ToString("dd/MM/yyyy");
		//		Ligne.Cells[1].StringFormat = StringFormat;

		//		Ligne.Cells[2].Value = item.DateDeRetour.Value.ToString("dd/MM/yyyy");
		//		Ligne.Cells[2].StringFormat = StringFormat;

		//		Ligne.Cells[3].Value = item.Classe.ToUpper();
		//		Ligne.Cells[3].StringFormat = StringFormat;

		//		Ligne.Cells[4].Value = item.Routing.ToUpper();
		//		Ligne.Cells[4].StringFormat = StringFormat;

		//		Ligne.Cells[5].Value = item.MontantBillet.Value.ToString("N0");
		//		Ligne.Cells[5].StringFormat = StringFormat;
		//		Num++;
		//	}

		//	Grille.ApplyBuiltinStyle(PdfGridBuiltinStyle.PlainTable3);
  //          PdfGridStyle GrilleStyle = new PdfGridStyle();
  //          GrilleStyle.CellPadding = new PdfPaddings(5,5,10,10);
  //          Grille.Style = GrilleStyle;


  //          PdfGridLayoutFormat GrilleLayoutFormat = new PdfGridLayoutFormat();
  //          GrilleLayoutFormat.Layout = PdfLayoutType.Paginate;
  //          Grille.Draw(page, CurseurListePosition.X, CurseurListePosition.Y, page.Size.Width, GrilleLayoutFormat);

		//	PointF DatePosition = new PointF(30, 672);
		//	graphics.DrawString($"Fait le : {DateDeSaisie}", Texte12Regular, BOADGrayBrush, DatePosition);
  //          DatePosition.Y += 28;

		//	graphics.DrawString("SIGNATAIRE", Texte13RegularUnderlined, BOADGrayBrush, DatePosition);
		//	DatePosition.Y += 28;

		//	graphics.DrawString(NomSignataire, Texte14Bold, BOADGrayBrush, DatePosition);
		//	DatePosition.Y += 19;

		//	graphics.DrawString(FonctionSignataire, Texte10Regular, BOADGrayBrush, DatePosition);
		//	DatePosition.Y += 19;

		//	MemoryStream stream = new MemoryStream();
		//	document.Save(stream);
		//	document.Close(true);
		//	return stream;
		//}

		public MemoryStream ImprimerBonDePassageTest(ImprimerBonModel Model)
		{
			var DateDeSaisie = Model.DateSaisie.Value.ToString("dd/MM/yyyy");
			var IDBonDePassage = Model.IDBonDePassage.ToString("D11");

			PdfDocument document = new PdfDocument();
			document.PageSettings.Size = PdfPageSize.A4;

			document.PageSettings.Margins.Top = 0;
			document.PageSettings.Margins.Left = 0;
			document.PageSettings.Margins.Right = 0;
			document.PageSettings.Margins.Bottom = 0;

			PdfPage page = document.Pages.Add();
			PdfGraphics graphics = page.Graphics;

			PdfColor BOADGray = new PdfColor(68, 68, 68);
			PdfSolidBrush BOADGrayBrush = new PdfSolidBrush(BOADGray);
			PdfColor BOADRed = new PdfColor(173, 23, 44);
			PdfSolidBrush BOADRedBrush = new PdfSolidBrush(BOADRed);
			PdfColor BOADBlack = new PdfColor(0, 0, 0);
			PdfSolidBrush BOADBlackBrush = new PdfSolidBrush(BOADBlack);

			PdfStandardFont Texte9Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 9, PdfFontStyle.Regular);
			PdfStandardFont Texte10Bold = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
			PdfStandardFont Texte10Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Regular);
			PdfStandardFont Texte12Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Regular);
			PdfStandardFont Texte13Bold = new PdfStandardFont(PdfFontFamily.Helvetica, 13, PdfFontStyle.Bold);
			PdfStandardFont Texte13Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 13, PdfFontStyle.Regular);
			PdfStandardFont Texte13RegularUnderlined = new PdfStandardFont(PdfFontFamily.Helvetica, 13, PdfFontStyle.Underline);
			PdfStandardFont Texte14Bold = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
			PdfStandardFont Texte16Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Regular);
			PdfStandardFont Texte10RegularUnderlined = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Underline);

			graphics.DrawString("BON DE PASSAGE EN COMPTE", Texte13Bold, BOADGrayBrush, new PointF(30, 30));
			graphics.DrawString($"N° {IDBonDePassage}", Texte16Regular, BOADRedBrush, new PointF(30, 56));

			FileStream imageStream = new FileStream(_hostingEnvironment.WebRootPath + "//logo.png", FileMode.Open, FileAccess.Read);
			PdfBitmap LogoBOAD = new PdfBitmap(imageStream);
			graphics.DrawImage(LogoBOAD, new PointF(398, 34), new SizeF(46, 64));


			graphics.DrawString("68 Avenue de la Libération\nB.P 1172 Lomé Togo\nTél. : 00228 22 21 42 44\nE-mail : boadsiege@boad.org", Texte9Regular, BOADGrayBrush, new PointF(447, 38));
			graphics.DrawString("Joindre l'original de ce bon à la facture correspondante", Texte10Bold, BOADGrayBrush, new PointF(30, 808));
			graphics.DrawString($"Nous prions {Model.LibelleAgence} de bien vouloir DELIVRER contre le présent bon pour le \n compte de la BOAD {Model.NombreDeBillets} titre(s) de transport au nom de :", Texte10Regular, BOADBlackBrush, new PointF(30, 118));

			PointF CurseurListePosition = new PointF(30, 160);
			
			var Num = 1;
			foreach (var item in Model.ListeLigne)
			{
				graphics.DrawString($"{Num}. {item.NomPassager}", Texte10Bold, BOADBlackBrush, CurseurListePosition);
				CurseurListePosition.Y += 14;
				Num++;
			}

			CurseurListePosition.Y = 256;
			graphics.DrawString("Pour les parcours respectifs suivants :", Texte10Regular, BOADBlackBrush, CurseurListePosition);
			CurseurListePosition.Y += 16;

			PdfGrid Grille = new PdfGrid();
			Grille.Style.Font = Texte14Bold;

			Grille.Columns.Add(5);
			Grille.Columns[0].Width = 20;
			Grille.Columns[1].Width = 70;
			Grille.Columns[2].Width = 70;
			Grille.Columns[3].Width = 70;
			Grille.Columns[4].Width = 305;

			Grille.Headers.Add(1);
			PdfStringFormat StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			PdfGridRow Header = Grille.Headers[0];

			Header.Cells[0].Value = "N°";
			Header.Cells[0].StringFormat.LineAlignment = PdfVerticalAlignment.Middle;

			Header.Cells[1].Value = "Date Départ";
			Header.Cells[1].StringFormat = StringFormat;

			Header.Cells[2].Value = "Date Retour";
			Header.Cells[2].StringFormat = StringFormat;

			Header.Cells[3].Value = "Classe";
			Header.Cells[3].StringFormat = StringFormat;
			Header.Cells[4].Value = "Itinéraires";
			Header.Cells[4].StringFormat = StringFormat;

			PdfGridCellStyle StyleCellule = new PdfGridCellStyle();
			StyleCellule.Borders.All = PdfPens.Transparent;
			StyleCellule.TextBrush = BOADBlackBrush;
			StyleCellule.BackgroundBrush = new PdfSolidBrush(Color.White);

			for (int j = 0; j < Header.Cells.Count; j++)
			{
				PdfGridCell Cellule = Header.Cells[j];
				Cellule.Style = StyleCellule;
			}

			Num = 1;
			foreach (var item in Model.ListeLigne)
			{
				PdfGridRow Ligne = Grille.Rows.Add();
				Ligne.Cells[0].Value = Num.ToString();
				Ligne.Cells[0].StringFormat.LineAlignment = PdfVerticalAlignment.Middle;

				Ligne.Cells[1].Value = item.DateDepart.Value.ToString("dd/MM/yyyy");
				Ligne.Cells[1].StringFormat = StringFormat;

				Ligne.Cells[2].Value = item.DateDeRetour.Value.ToString("dd/MM/yyyy");
				Ligne.Cells[2].StringFormat = StringFormat;

				Ligne.Cells[3].Value = item.Classe.ToUpper();
				Ligne.Cells[3].StringFormat = StringFormat;

				Ligne.Cells[4].Value = item.Routing.ToUpper();
				Ligne.Cells[4].StringFormat = StringFormat;

				Num++;
			}

			Grille.ApplyBuiltinStyle(PdfGridBuiltinStyle.PlainTable3);
			PdfGridStyle GrilleStyle = new PdfGridStyle();
			GrilleStyle.CellPadding = new PdfPaddings(5, 5, 10, 10);
			Grille.Style = GrilleStyle;


			PdfGridLayoutFormat GrilleLayoutFormat = new PdfGridLayoutFormat();
			GrilleLayoutFormat.Layout = PdfLayoutType.Paginate;
			Grille.Draw(page, CurseurListePosition.X, CurseurListePosition.Y, page.Size.Width, GrilleLayoutFormat);

			PointF DatePosition = new PointF(30, 734);
			graphics.DrawString($"Fait le : {DateDeSaisie}", Texte10Regular, BOADGrayBrush, DatePosition);
			DatePosition.Y += 19;

			graphics.DrawString("SIGNATURE", Texte10RegularUnderlined, BOADGrayBrush, DatePosition);
			graphics.DrawString("COUT TOTAL :", Texte10RegularUnderlined, BOADGrayBrush, 349, 719);


			PdfStringFormat StringFormat2 = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);

			PdfGrid GrilleAgence = new PdfGrid();

			GrilleAgence.Columns.Add(5);
			GrilleAgence.Columns[0].Width = 20;
			GrilleAgence.Columns[1].Width = 186;
			GrilleAgence.Columns[2].Width = 108;
			GrilleAgence.Columns[3].Width = 118;
			GrilleAgence.Columns[4].Width = 100;


			GrilleAgence.Headers.Add(1);
			PdfGridRow HeaderAgence = GrilleAgence.Headers[0];

			HeaderAgence.Cells[0].Value = "N°";
			HeaderAgence.Cells[0].StringFormat.LineAlignment = PdfVerticalAlignment.Middle;

			HeaderAgence.Cells[1].Value = "DOCUMENT EMIS";
			HeaderAgence.Cells[1].StringFormat.LineAlignment = PdfVerticalAlignment.Middle;

			HeaderAgence.Cells[2].Value = "EMIS LE";
			HeaderAgence.Cells[2].StringFormat = StringFormat2;

			HeaderAgence.Cells[3].Value = "TARIF";
			HeaderAgence.Cells[3].StringFormat = StringFormat2;

			HeaderAgence.Cells[4].Value = "TOTAL";
			HeaderAgence.Cells[4].StringFormat = StringFormat2;

			PdfGridCellStyle StyleCellule2 = new PdfGridCellStyle();
			StyleCellule2.Borders.All = PdfPens.Transparent;
			StyleCellule2.TextBrush = BOADBlackBrush;
			StyleCellule2.BackgroundBrush = new PdfSolidBrush(Color.DarkRed);

			for (int j = 0; j < HeaderAgence.Cells.Count; j++)
			{
				PdfGridCell Cellule = HeaderAgence.Cells[j];
				Cellule.Style = StyleCellule;
			}

			var Num2 = 1;
			foreach (var item in Model.ListeLigne)
			{
				PdfGridRow Ligne = GrilleAgence.Rows.Add();
				Ligne.Cells[0].Value = Num2.ToString();
				Ligne.Cells[0].StringFormat.LineAlignment = PdfVerticalAlignment.Middle;

				Num2++;
			}

			GrilleAgence.ApplyBuiltinStyle(PdfGridBuiltinStyle.PlainTable1);
			PdfGridStyle GrilleAgenceStyle = new PdfGridStyle();
			GrilleAgenceStyle.CellPadding = new PdfPaddings(5, 5, 10, 10);
			GrilleAgence.Style = GrilleAgenceStyle;

			graphics.DrawString("A remplir par l'Agence de Voyage :", Texte10Regular, BOADBlackBrush, 30, 480);

			PdfGridLayoutFormat GrilleAgenceLayoutFormat = new PdfGridLayoutFormat();
			GrilleAgenceLayoutFormat.Layout = PdfLayoutType.Paginate;
			GrilleAgence.Draw(page, 30, 511, page.Size.Width, GrilleAgenceLayoutFormat);


			MemoryStream stream = new MemoryStream();
			document.Save(stream);
			document.Close(true);
			return stream;
		}
		public MemoryStream ImprimerBEReglement(ImprimerBERModel Model)
        {
			PdfDocument document = new PdfDocument();
			document.PageSettings.Size = PdfPageSize.A4;

			document.PageSettings.Margins.Top = 0;
			document.PageSettings.Margins.Bottom = 0;
			document.PageSettings.Margins.Left = 0;
			document.PageSettings.Margins.Right = 0;

			PdfPage page = document.Pages.Add();
			PdfGraphics graphics = page.Graphics;
			PdfPen pen = new PdfPen(PdfBrushes.Black, 1);

			PdfColor BOADGray = new PdfColor(68, 68, 68);
			PdfSolidBrush BOADGrayBrush = new PdfSolidBrush(BOADGray);

			PdfColor BOADLightGray = new PdfColor(239, 239, 239);
			PdfSolidBrush BOADLightGrayBrush = new PdfSolidBrush(BOADLightGray);

			PdfColor BOADRed = new PdfColor(173, 23, 44);
			PdfSolidBrush BOADRedBrush = new PdfSolidBrush(BOADRed);

			PdfColor BOADBlack = new PdfColor(0, 0, 0);
			PdfSolidBrush BOADBlackBrush = new PdfSolidBrush(BOADBlack);

			PdfStandardFont Texte8Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 8, PdfFontStyle.Regular);

			PdfStandardFont Texte9Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 9, PdfFontStyle.Regular);
			PdfStandardFont Texte10Bold = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
			PdfStandardFont Texte10Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Regular);
			PdfStandardFont Texte12Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Regular);
			PdfStandardFont Texte13Bold = new PdfStandardFont(PdfFontFamily.Helvetica, 13, PdfFontStyle.Bold);
			PdfStandardFont Texte13Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 13, PdfFontStyle.Regular);
			PdfStandardFont Texte13RegularUnderlined = new PdfStandardFont(PdfFontFamily.Helvetica, 13, PdfFontStyle.Underline);
			PdfStandardFont Texte14Bold = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
			PdfStandardFont Texte16Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Regular);
			PdfStandardFont Texte20Bold = new PdfStandardFont(PdfFontFamily.Helvetica, 20, PdfFontStyle.Bold);


			FileStream imageStream = new FileStream(_hostingEnvironment.WebRootPath + "/images/logo_big.png", FileMode.Open, FileAccess.Read);
			PdfBitmap LogoBOAD = new PdfBitmap(imageStream);
			graphics.DrawImage(LogoBOAD, new PointF(40, 40), new SizeF(70, 101));

			graphics.DrawString("BORDEREAU D’ENVOI", Texte20Bold, BOADGrayBrush, new PointF(163, 104));
			graphics.DrawString("Monsieur le Directeur du DFI", Texte13Regular, BOADGrayBrush, new PointF(290, 142));
			graphics.DrawString($"Référence : DAG/DVPD-{DateTime.Now.ToString("yyyy")}l", Texte13Regular, BOADGrayBrush, new PointF(373, 192));
			graphics.DrawString("Lomé, le 13/11/2023", Texte13Regular, BOADGrayBrush, new PointF(60, 192));

			graphics.DrawString("Signature", Texte12Regular, BOADGrayBrush, new PointF(331, 646));
			graphics.DrawString("Komlan Norbert MENSAH", Texte13RegularUnderlined, BOADGrayBrush, new PointF(331, 685));
			graphics.DrawString("Directeur du DAG", Texte13Regular, BOADGrayBrush, new PointF(331, 706));
			graphics.DrawString($"BE-DFI {Model.LibelleAgence} du {Model.DateSaisie.ToString("dd/MM/yyyy")}", Texte10Regular, BOADGrayBrush, new PointF(220, 805));


			PdfGrid Grille = new PdfGrid();
			Grille.Style.Font = Texte13Regular;

			Grille.Columns.Add(3);
			Grille.Columns[0].Width = 274;
			Grille.Columns[1].Width = 70;
			Grille.Columns[2].Width = 177;

			Grille.Headers.Add(1);
			PdfStringFormat stringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			stringFormat.WordWrap = PdfWordWrapType.Word;
			stringFormat.LineSpacing = 4;

			PdfGridRow Header = Grille.Headers[0];

			Header.Cells[0].Value = "Désignation des pièces";
			Header.Cells[0].StringFormat.LineAlignment = PdfVerticalAlignment.Middle;
			Header.Cells[1].Value = "Nombre";
			Header.Cells[1].StringFormat = stringFormat;
			Header.Cells[2].Value = "Observations";
			Header.Cells[2].StringFormat = stringFormat;

			PdfGridCellStyle cellStyle = new PdfGridCellStyle();
			cellStyle.Borders.All = PdfPens.Black;
			cellStyle.TextBrush = PdfBrushes.Black;
			cellStyle.BackgroundBrush = new PdfSolidBrush(Color.FromArgb(0, 255, 40, 40));

			PdfGridCellStyle cellStyleSansBorder = new PdfGridCellStyle();
			cellStyleSansBorder.Borders.All = PdfPens.Transparent;
			cellStyleSansBorder.CellPadding = new PdfPaddings(5,5,0,0);


			PdfGridCellStyle LastCellStyleSansBorder = new PdfGridCellStyle();
			LastCellStyleSansBorder.Borders.Bottom = PdfPens.Black;
			LastCellStyleSansBorder.Borders.Top = PdfPens.Transparent;
			LastCellStyleSansBorder.CellPadding = new PdfPaddings(5, 5, 0, 0);


			for (int j = 0; j < Header.Cells.Count; j++)
			{
				PdfGridCell cell = Header.Cells[j];
				cell.Style = cellStyle;
			}

			float GridHeight = 0;

			for (var i=0; i< Model.ListeLigneFacture.Count; i++)
			{
				PdfGridRow Ligne = Grille.Rows.Add();
				Ligne.Cells[0].Value = $"Original de la facture N° {Model.ListeLigneFacture[i].RefFacture} en date du {Model.ListeLigneFacture[i].DateEmissionFacture.ToString("dd/MM/yyyy")} de la compagnie {Model.LibelleAgence}, d'un montant de {Model.ListeLigneFacture[i].MontantTotalFacture.Value.ToString("N0")} FCFA en faveur de";
				Ligne.Cells[0].StringFormat = stringFormat;
				Ligne.ApplyStyle(cellStyleSansBorder);
				Ligne.Cells[0].Style.Borders.Left = PdfPens.Black;
				Ligne.Cells[2].Style.Borders.Right = PdfPens.Black;

				Ligne.Cells[1].Value = "01";
				Ligne.Cells[1].StringFormat = stringFormat;

				Ligne.Cells[2].Value = "Pour règlement";
				Ligne.Cells[2].StringFormat = stringFormat;

				foreach( var ligneBene in Model.ListeLigneFacture[i].ListeAgentsBeneficiaires)
				{
					PdfGridRow Ligne2 = Grille.Rows.Add();
					Ligne2.Cells[0].Value = $"\t- {ligneBene.NomBeneficiaire}";
					Ligne2.Cells[0].StringFormat = stringFormat;

					if(Model.ListeLigneFacture[i].ListeAgentsBeneficiaires.IndexOf(ligneBene) == (Model.ListeLigneFacture[i].ListeAgentsBeneficiaires.Count - 1))
					{
						Ligne2.ApplyStyle(LastCellStyleSansBorder);
					}
					else {
						Ligne2.ApplyStyle(cellStyleSansBorder);
					}

					Ligne2.Cells[0].Style.Borders.Left = PdfPens.Black;
					Ligne2.Cells[2].Style.Borders.Right = PdfPens.Black;
				}
			}

			Grille.ApplyBuiltinStyle(PdfGridBuiltinStyle.TableGrid);
			PdfGridStyle GrilleStyle = new PdfGridStyle();
			GrilleStyle.CellPadding = new PdfPaddings(5, 5, 5, 5);
			Grille.Style = GrilleStyle;
			PdfGridLayoutFormat layoutFormat = new PdfGridLayoutFormat();
			layoutFormat.Layout = PdfLayoutType.Paginate;

			Grille.Draw(page, 50, 233, 495, layoutFormat);

            foreach (var item in Grille.Rows)
            {
				GridHeight += item.Height;
            }

			GridHeight += Header.Height;

			if(GridHeight > 400)
			{
				throw new Exception("Le BE déborde réduisez le nombre de factures.");
			}

			MemoryStream stream = new MemoryStream();
			document.Save(stream);
			document.Close(true);
			return stream;
		}
		public MemoryStream ImprimerBEReglementV2(ImprimerBERModel Model)
		{
			PdfDocument document = new PdfDocument();
			document.PageSettings.Size = PdfPageSize.A4;

			document.PageSettings.Margins.Top = 0;
			document.PageSettings.Margins.Bottom = 0;
			document.PageSettings.Margins.Left = 0;
			document.PageSettings.Margins.Right = 0;

			PdfPage page = document.Pages.Add();
			PdfGraphics graphics = page.Graphics;
			PdfColor BOADBlack = new PdfColor(0, 0, 0);
			PdfSolidBrush BOADBlackBrush = new PdfSolidBrush(BOADBlack);

			PdfStandardFont Texte10Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Regular);
			PdfStandardFont Texte12Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Regular);
			PdfStandardFont Texte13Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 13, PdfFontStyle.Regular);
			PdfStandardFont Texte12RegularUnderlined = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Underline);
			PdfStandardFont Texte20Bold = new PdfStandardFont(PdfFontFamily.Helvetica, 20, PdfFontStyle.Bold);

			FileStream imageStream = new FileStream(_hostingEnvironment.WebRootPath + "/images/logo_big.png", FileMode.Open, FileAccess.Read);
			PdfBitmap LogoBOAD = new PdfBitmap(imageStream);
			graphics.DrawImage(LogoBOAD, new PointF(40, 40), new SizeF(70, 101));

			graphics.DrawString("BORDEREAU D ENVOI", Texte20Bold, BOADBlackBrush, new PointF(163, 104));
			graphics.DrawString("Monsieur le Directeur du DFI", Texte13Regular, BOADBlackBrush, new PointF(290, 142));
			graphics.DrawString($"Référence : DAG/DVPD-{DateTime.Now.ToString("yyyy")}l", Texte13Regular, BOADBlackBrush, new PointF(373, 192));
			graphics.DrawString($"Lomé, le {Model.DateSaisie.ToString("dd/MM/yyyy")}", Texte13Regular, BOADBlackBrush, new PointF(60, 192));

			graphics.DrawString("Signature", Texte12Regular, BOADBlackBrush, new PointF(331, 706));
			graphics.DrawString($"{Model.Signataire.NomSignataire}", Texte12RegularUnderlined, BOADBlackBrush, new PointF(331, 732));
			graphics.DrawString($"{Model.Signataire.FonctionSignataire}", Texte10Regular, BOADBlackBrush, new PointF(331, 753));
			graphics.DrawString($"BE-DFI {Model.LibelleAgence} du {Model.DateSaisie.ToString("dd/MM/yyyy")}", Texte10Regular, BOADBlackBrush, new PointF(220, 805));


			PdfGrid Grille = new PdfGrid();
			Grille.Style.Font = Texte13Regular;

			Grille.Columns.Add(4);
			Grille.Columns[0].Width = 210;
			Grille.Columns[1].Width = 130;
			Grille.Columns[2].Width = 64;
			Grille.Columns[3].Width = 91;

			Grille.Headers.Add(1);
			PdfStringFormat stringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			stringFormat.WordWrap = PdfWordWrapType.Word;
			stringFormat.LineSpacing = 4;

			PdfGridRow Header = Grille.Headers[0];

			Header.Cells[0].Value = "Désignation des pièces";
			Header.Cells[0].StringFormat.LineAlignment = PdfVerticalAlignment.Middle;
			Header.Cells[1].Value = "Référence Billet";
			Header.Cells[1].StringFormat = stringFormat;
			Header.Cells[2].Value = "Nombre";
			Header.Cells[2].StringFormat = stringFormat;
			Header.Cells[3].Value = "Observations";
			Header.Cells[3].StringFormat = stringFormat;

			PdfGridCellStyle cellStyle = new PdfGridCellStyle();
			cellStyle.Borders.All = PdfPens.Black;
			cellStyle.TextBrush = PdfBrushes.Black;
			cellStyle.BackgroundBrush = new PdfSolidBrush(Color.FromArgb(0, 255, 40, 40));

			PdfGridCellStyle cellStyleSansBorder = new PdfGridCellStyle();
			cellStyleSansBorder.Borders.All = PdfPens.Transparent;
			cellStyleSansBorder.CellPadding = new PdfPaddings(5, 5, 0, 0);


			PdfGridCellStyle LastCellStyleSansBorder = new PdfGridCellStyle();
			LastCellStyleSansBorder.Borders.Bottom = PdfPens.Black;
			LastCellStyleSansBorder.Borders.Top = PdfPens.Transparent;
			LastCellStyleSansBorder.CellPadding = new PdfPaddings(5, 5, 0, 0);

			for (int j = 0; j < Header.Cells.Count; j++)
			{
				PdfGridCell cell = Header.Cells[j];
				cell.Style = cellStyle;
			}

			float GridHeight = 0;
			for (var i = 0; i < Model.ListeLigneFacture.Count; i++)
			{
				PdfGridRow Ligne = Grille.Rows.Add();
				Ligne.Cells[0].Value = $"Original de la facture N° {Model.ListeLigneFacture[0].RefFacture} en date du {Model.ListeLigneFacture[i].DateEmissionFacture.ToString("dd/MM/yyyy")} de la compagnie {Model.LibelleAgence}, d'un montant de {Model.ListeLigneFacture[i].MontantTotalFacture.Value.ToString("N0")} FCFA en faveur de";
				Ligne.Cells[0].StringFormat = stringFormat;
				Ligne.ApplyStyle(cellStyleSansBorder);
				Ligne.Cells[0].Style.Borders.Left = PdfPens.Black;
				Ligne.Cells[3].Style.Borders.Right = PdfPens.Black;

				Ligne.Cells[2].Value = "01";
				Ligne.Cells[2].StringFormat = stringFormat;

				Ligne.Cells[3].Value = "Pour règlement";
				Ligne.Cells[3].StringFormat = stringFormat;

				for (var j = 0; j < Model.ListeLigneFacture[i].ListeAgentsBeneficiaires.Count; j++)
				{
					PdfGridRow Ligne2 = Grille.Rows.Add();
					Ligne2.Cells[0].Value = $"\t- {Model.ListeLigneFacture[i].ListeAgentsBeneficiaires[j].NomBeneficiaire}";
					Ligne2.Cells[0].StringFormat = stringFormat;

					Ligne2.Cells[1].Value = $"{Model.ListeLigneFacture[i].ListeAgentsBeneficiaires[j].RefTitreBeneficiaire}";
					Ligne2.Cells[1].StringFormat = stringFormat;

					if (j == (Model.ListeLigneFacture[i].ListeAgentsBeneficiaires.Count -1))
					{
						Ligne2.ApplyStyle(LastCellStyleSansBorder);
					}
					else
					{
						Ligne2.ApplyStyle(cellStyleSansBorder);
					}

					Ligne2.Cells[0].Style.Borders.Left = PdfPens.Black;
					Ligne2.Cells[3].Style.Borders.Right = PdfPens.Black;
				}
			}

			Grille.ApplyBuiltinStyle(PdfGridBuiltinStyle.TableGrid);
			PdfGridStyle GrilleStyle = new PdfGridStyle();
			GrilleStyle.CellPadding = new PdfPaddings(5, 5, 5, 5);
			Grille.Style = GrilleStyle;
			PdfGridLayoutFormat layoutFormat = new PdfGridLayoutFormat();
			layoutFormat.Layout = PdfLayoutType.Paginate;
			Grille.Draw(page, 50, 233, 495, layoutFormat);

			foreach (var item in Grille.Rows)
			{
				GridHeight += item.Height;
			}

			GridHeight += Header.Height;
			if (GridHeight > 470)
			{
				throw new Exception("Le BE déborde réduisez le nombre de factures.");
			}

			MemoryStream stream = new MemoryStream();
			document.Save(stream);
			document.Close(true);
			return stream;
		}
		public bool VerifierSiBEReglementImprimable()
		{
			PdfDocument document = new PdfDocument();
			document.PageSettings.Size = PdfPageSize.A4;

			document.PageSettings.Margins.Top = 0;
			document.PageSettings.Margins.Left = 0;
			document.PageSettings.Margins.Right = 0;
			document.PageSettings.Margins.Bottom = 0;

			PdfPage page = document.Pages.Add();

			PdfStandardFont Texte13Regular = new PdfStandardFont(PdfFontFamily.Helvetica, 13, PdfFontStyle.Regular);


			PdfGrid Grille = new PdfGrid();
			Grille.Style.Font = Texte13Regular;

			Grille.Columns.Add(3);
			Grille.Columns[1].Width = 70;
			Grille.Columns[0].Width = 274;
			Grille.Columns[2].Width = 177;

			Grille.Headers.Add(1);
			PdfStringFormat stringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			stringFormat.WordWrap = PdfWordWrapType.Word;
			stringFormat.LineSpacing = 4;

			PdfGridRow Header = Grille.Headers[0];

			Header.Cells[0].Value = "Désignation des pièces";
			Header.Cells[0].StringFormat.LineAlignment = PdfVerticalAlignment.Middle;
			Header.Cells[1].Value = "Nombre";
			Header.Cells[1].StringFormat = stringFormat;
			Header.Cells[2].Value = "Observations";
			Header.Cells[2].StringFormat = stringFormat;

			PdfGridCellStyle cellStyle = new PdfGridCellStyle();
			cellStyle.Borders.All = PdfPens.Black;
			cellStyle.TextBrush = PdfBrushes.Black;
			cellStyle.BackgroundBrush = new PdfSolidBrush(Color.FromArgb(0, 255, 40, 40));

			PdfGridCellStyle cellStyleSansBorder = new PdfGridCellStyle();
			cellStyleSansBorder.Borders.All = PdfPens.Transparent;
			cellStyleSansBorder.CellPadding = new PdfPaddings(5, 5, 0, 0);

			PdfGridCellStyle LastCellStyleSansBorder = new PdfGridCellStyle();
			LastCellStyleSansBorder.Borders.Bottom = PdfPens.Black;
			LastCellStyleSansBorder.Borders.Top = PdfPens.Transparent;
			LastCellStyleSansBorder.CellPadding = new PdfPaddings(5, 5, 0, 0);

			for (int j = 0; j < Header.Cells.Count; j++)
			{
				PdfGridCell cell = Header.Cells[j];
				cell.Style = cellStyle;
			}

			float GridHeight = 0;
			for (var i = 0; i <= 3; i++)
			{
				PdfGridRow Ligne = Grille.Rows.Add();
				Ligne.Cells[0].Value = "Original de la facture N° TG30500004 en date du 24/05/2023 de la compagnie AIR FRANCE, d'un montant de 17 612 600 FCFA en faveur de";
				Ligne.Cells[0].StringFormat = stringFormat;
				Ligne.ApplyStyle(cellStyleSansBorder);
				Ligne.Cells[0].Style.Borders.Left = PdfPens.Black;
				Ligne.Cells[2].Style.Borders.Right = PdfPens.Black;

				Ligne.Cells[1].Value = "01";
				Ligne.Cells[1].StringFormat = stringFormat;

				Ligne.Cells[2].Value = "Pour règlement";
				Ligne.Cells[2].StringFormat = stringFormat;


				PdfGridRow Ligne2 = Grille.Rows.Add();
				Ligne2.Cells[0].Value = "\t- BOBY AKAMA Yao";
				Ligne2.Cells[0].StringFormat = stringFormat;
				Ligne2.ApplyStyle(cellStyleSansBorder);
				Ligne2.Cells[0].Style.Borders.Left = PdfPens.Black;
				Ligne2.Cells[2].Style.Borders.Right = PdfPens.Black;


				PdfGridRow Ligne3 = Grille.Rows.Add();
				Ligne3.Cells[0].Value = "\t- KABORE Modeste Romeo";
				Ligne3.Cells[0].StringFormat = stringFormat;
				Ligne3.ApplyStyle(cellStyleSansBorder);
				Ligne3.Cells[0].Style.Borders.Left = PdfPens.Black;
				Ligne3.Cells[2].Style.Borders.Right = PdfPens.Black;


				PdfGridRow Ligne4 = Grille.Rows.Add();
				Ligne4.Cells[0].Value = "\t- KAFANDO Ambroise";
				Ligne4.Cells[0].StringFormat = stringFormat;
				Ligne4.ApplyStyle(cellStyleSansBorder);
				Ligne4.Cells[0].Style.Borders.Left = PdfPens.Black;
				Ligne4.Cells[2].Style.Borders.Right = PdfPens.Black;


				PdfGridRow Ligne5 = Grille.Rows.Add();
				Ligne5.Cells[0].Value = "\t- MOGLO Komlanvi";
				Ligne5.Cells[0].StringFormat = stringFormat;
				Ligne5.ApplyStyle(LastCellStyleSansBorder);
				Ligne5.Cells[0].Style.Borders.Left = PdfPens.Black;
				Ligne5.Cells[2].Style.Borders.Right = PdfPens.Black;
			}

			Grille.ApplyBuiltinStyle(PdfGridBuiltinStyle.TableGrid);
			PdfGridStyle GrilleStyle = new PdfGridStyle();
			GrilleStyle.CellPadding = new PdfPaddings(5, 5, 5, 5);
			Grille.Style = GrilleStyle;
			PdfGridLayoutFormat layoutFormat = new PdfGridLayoutFormat();
			layoutFormat.Layout = PdfLayoutType.Paginate;

			Grille.Draw(page, 50, 233, 495, layoutFormat);

			foreach (var item in Grille.Rows)
			{
				GridHeight += item.Height;
			}

			GridHeight += Header.Height;

			if (GridHeight > 400)
			{
				return false;
			}
			
			document.Dispose();
			document.Close();
			return true;
		}
	}
}
