using System.Drawing;
using Gestions_des_Titres_de_Transport.Constant;
using Gestions_des_Titres_de_Transport.Models;
using Gestions_des_Titres_de_Transport.SAPCENTERDBModelsEF;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;
using Microsoft.EntityFrameworkCore;

namespace Gestions_des_Titres_de_Transport.Services.BER
{
	public class SaisieBERService : ISaisieBERService
	{
		private readonly IFichierService _fichierService;
		private readonly IUtilisateurService _agentService;
		private readonly SAPCENTERDBContext _sapCenterDBContext;
		private readonly TitreDeTransportContext _titreDeTransportContext;

		public SaisieBERService(
			TitreDeTransportContext titreDeTransportContext,
	IFichierService fichierService,
	IUtilisateurService agentService,
	SAPCENTERDBContext sapCenterDBContext
			)
		{
			_agentService = agentService;
			_fichierService = fichierService;
			_sapCenterDBContext = sapCenterDBContext;
			_titreDeTransportContext = titreDeTransportContext;
		}
		public async Task CreerBEReglement(CreerBEReglementModel Model)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var NouveauBEReglement = new Bereglement();
				var MatAgentSaisie = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var EtapeSaisieBEReglement = _titreDeTransportContext.Etape.Single(facture => facture.Numeroetape == ConstantesEtapes.SAISI_BEREGLEMENT);

				var ListeDesFacturesChoisis = _titreDeTransportContext.Facture.Where(facture => Model.ListeDesIDFacturesChoisis.Contains(facture.Idfacture));
				foreach (var facture in ListeDesFacturesChoisis)
				{
					facture.Idetapefacture = EtapeSaisieBEReglement.Idetape;
					NouveauBEReglement.Facture.Add(facture);
				}

				NouveauBEReglement.Datesaiber = DateTime.Now;
				NouveauBEReglement.Obserber = Model.Observation;
				NouveauBEReglement.Idetapeber = EtapeSaisieBEReglement.Idetape;
				NouveauBEReglement.Matagentsaiber = MatAgentSaisie;
				NouveauBEReglement.Idagencevoyageber = Model.IDAgenceVoyage;

				await _titreDeTransportContext.Bereglement.AddAsync(NouveauBEReglement);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}

		public async Task CreerBEReglementV2(CreerBEReglementModelV2 Model, List<BERFactureModel> ListeFactures)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var NouveauBEReglement = new Bereglement();
				var MatAgentSaisie = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var MatAgentSignataire = (_sapCenterDBContext.Agentssap.Single(age => age.NomEtPrenoms == Model.NomSignataire)).Matricule;
				var EtapeSaisieBEReglement = _titreDeTransportContext.Etape.Single(facture => facture.Numeroetape == ConstantesEtapes.SAISI_BEREGLEMENT);

				NouveauBEReglement.Datesaiber = DateTime.Now;
				NouveauBEReglement.Obserber = Model.Observation;
				NouveauBEReglement.Idetapeber = EtapeSaisieBEReglement.Idetape;
				NouveauBEReglement.Matagentsaiber = MatAgentSaisie;
				NouveauBEReglement.Idagencevoyageber = Model.IDAgenceVoyage;
				NouveauBEReglement.Matagentvalber = MatAgentSignataire;

				_titreDeTransportContext.Bereglement.Add(NouveauBEReglement);
				_titreDeTransportContext.SaveChanges();

				var FactureListe = new List<TitreDeTransportModelsEF.Facture>();

				var ListeReferencesFactures = ListeFactures.Select(fact => fact.RefFacture);
                foreach (var item in ListeFactures)
                {
					var Resultat = _titreDeTransportContext.Facture
						.Where(fact => ListeReferencesFactures.Contains(fact.Numerofactureagence))
						.ToList();

					if (Resultat.Count != 0)
					{
						throw new Exception("Une des référénces de facture existe déjà. Veuillez vérifier et corrigez.");
					}
                }

                foreach (var item in ListeFactures)
				{
					var NouvelleFacture = new TitreDeTransportModelsEF.Facture()
					{
						Datesaisifacture = DateTime.Now,
						Matagentvalfacture = MatAgentSignataire,
						Idetapefacture = EtapeSaisieBEReglement.Idetape,
						Matagentsaifacture = MatAgentSaisie,
						Idagencevoyagefacture = Model.IDAgenceVoyage,
						Dateagenceemissionfacture = item.DateEmission,
						Numerofactureagence = item.RefFacture,
						Idbereglem = NouveauBEReglement.Idbereglem
					};

					_titreDeTransportContext.Facture.Add(NouvelleFacture);
					_titreDeTransportContext.SaveChanges();


					foreach (var item1 in item.ListeTitres)
					{
						var Titre = _titreDeTransportContext.Titredetransport.Single(_ => _.Idtitre == item1.IDTitre);
						Titre.Idfacture = NouvelleFacture.Idfacture;
						_titreDeTransportContext.Titredetransport.Update(Titre);
					}
				}

				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}

		public async Task EnvoyerBEREtapeSup(int IDBEReglement)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var BEREglementAEnvoyerEnEtapeSuperieure = _titreDeTransportContext.Bereglement.SingleOrDefault(ber => ber.Idbereglem == IDBEReglement);
				if (BEREglementAEnvoyerEnEtapeSuperieure is null)
				{
					throw new Exception("Le BE de règlement est introuvable.");
				}

				var IDEtapeActuelle = BEREglementAEnvoyerEnEtapeSuperieure.Idetapeber;
				var EtapeActuelle = await _titreDeTransportContext.Etape.SingleAsync(item => item.Idetape == IDEtapeActuelle);
				var EtapeProchaine = await _titreDeTransportContext.Etape
								.Where(e => EtapeActuelle.Numeroetape < e.Numeroetape && e.Sietapeactive == true)
								.OrderBy(item => item.Numeroetape)
								.FirstOrDefaultAsync();
				if (EtapeProchaine is null)
				{
					throw new Exception("Pas d'étape prochaine.");
				}

				var ListeFacturesLier = _titreDeTransportContext.Facture.Where(fact => fact.Idbereglem == IDBEReglement);

				foreach (var fact in ListeFacturesLier)
				{
					fact.Idetapefacture = EtapeProchaine.Idetape;
					var Titres = fact.Titredetransport;

					foreach (var titre in Titres)
					{
						titre.Idetape = EtapeProchaine.Idetape;
						_titreDeTransportContext.Titredetransport.Update(titre);
						_titreDeTransportContext.SaveChanges();
					}

					_titreDeTransportContext.Facture.Update(fact);
					_titreDeTransportContext.SaveChanges();
				}

				BEREglementAEnvoyerEnEtapeSuperieure.Idetapeber = EtapeProchaine.Idetape;
				_titreDeTransportContext.Bereglement.Update(BEREglementAEnvoyerEnEtapeSuperieure);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}

		public async Task EnvoyerBEREtapeSup(List<int> ListeIDBEReglement)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var ListeDesBEREglementAEnvoyerEnEtapeSuperieure = await _titreDeTransportContext.Bereglement.Where(ber => ListeIDBEReglement.Contains(ber.Idbereglem)).ToListAsync();
				if (ListeDesBEREglementAEnvoyerEnEtapeSuperieure.Count < 0)
				{
					throw new Exception("Les BE sont introuvables.");
				}

				var IDEtapeActuelle = ListeDesBEREglementAEnvoyerEnEtapeSuperieure[0].Idetapeber;
				var EtapeActuelle = await _titreDeTransportContext.Etape.SingleAsync(item => item.Idetape == IDEtapeActuelle);
				var EtapeProchaine = await _titreDeTransportContext.Etape
								.Where(e => EtapeActuelle.Numeroetape < e.Numeroetape && e.Sietapeactive == true)
								.OrderBy(item => item.Numeroetape)
								.FirstOrDefaultAsync();

				if (EtapeProchaine is null)
				{
					throw new Exception("Pas d'étape prochaine.");
				}

				foreach (var ber in ListeDesBEREglementAEnvoyerEnEtapeSuperieure)
				{
					var ListeFacturesLier = _titreDeTransportContext.Facture.Where(fact => fact.Idbereglem == ber.Idbereglem);

					foreach (var fact in ListeFacturesLier)
					{
						fact.Idetapefacture = EtapeProchaine.Idetape;
						var Titres = fact.Titredetransport;

						foreach (var titre in Titres)
						{
							titre.Idetape = EtapeProchaine.Idetape;
							_titreDeTransportContext.Titredetransport.Update(titre);
						}

						_titreDeTransportContext.Facture.Update(fact);
					}

					ber.Idetapeber = EtapeProchaine.Idetape;
					_titreDeTransportContext.Bereglement.Update(ber);
					_titreDeTransportContext.SaveChanges();
				}
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}

		public async Task ModifierBER(int IDBEReglement, CreerBEReglementModel Model)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var MatAgentSaisie = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var BEReglementAMettreAJour = await _titreDeTransportContext.Bereglement.SingleAsync(ber => ber.Idbereglem == IDBEReglement);
				var ListeAnciennesFacturesLier = _titreDeTransportContext.Facture.Where(fact => fact.Idbereglem == IDBEReglement);

				foreach (var item in ListeAnciennesFacturesLier)
				{
					item.Idbereglem = null;
				}
				_titreDeTransportContext.Facture.UpdateRange(ListeAnciennesFacturesLier);
				_titreDeTransportContext.SaveChanges();


				var ListeDesFacturesChoisis = _titreDeTransportContext.Facture.Where(facture => Model.ListeDesIDFacturesChoisis.Contains(facture.Idfacture));
				foreach (var facture in ListeDesFacturesChoisis)
				{
					BEReglementAMettreAJour.Facture.Add(facture);
				}

				BEReglementAMettreAJour.Datesaiber = DateTime.Now;
				BEReglementAMettreAJour.Obserber = Model.Observation;
				BEReglementAMettreAJour.Matagentsaiber = MatAgentSaisie;
				BEReglementAMettreAJour.Idagencevoyageber = Model.IDAgenceVoyage;

				_titreDeTransportContext.Bereglement.Update(BEReglementAMettreAJour);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}
		public async Task ModifierBEReglementV2(int IDBEReglement,CreerBEReglementModelV2 Model, List<BERFactureModel> ListeFactures)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var MatAgentSaisie = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var BEReglementAMettreAJour = await _titreDeTransportContext.Bereglement.SingleAsync(ber => ber.Idbereglem == IDBEReglement);
				var ListeAnciennesFacturesLier = _titreDeTransportContext.Facture.Where(fact => fact.Idbereglem == IDBEReglement).ToList();
				var MatAgentSignataire = (_sapCenterDBContext.Agentssap.Single(age => age.NomEtPrenoms == Model.NomSignataire)).Matricule;
				var EtapeSaisieBEReglement = _titreDeTransportContext.Etape.Single(facture => facture.Numeroetape == ConstantesEtapes.SAISI_BEREGLEMENT);

				foreach (var fact in ListeAnciennesFacturesLier)
				{
					var ListeTitreLier = _titreDeTransportContext.Titredetransport.Where(titre => fact.Idfacture == titre.Idfacture).ToList();
					foreach (var titre in ListeTitreLier)
					{
						titre.Idfacture = null;
						_titreDeTransportContext.Titredetransport.Update(titre);
						_titreDeTransportContext.SaveChanges();
					}
					_titreDeTransportContext.Facture.Remove(fact);
					_titreDeTransportContext.SaveChanges();
				}

				BEReglementAMettreAJour.Datesaiber = DateTime.Now;
				BEReglementAMettreAJour.Obserber = Model.Observation;
				BEReglementAMettreAJour.Matagentsaiber = MatAgentSaisie;
				BEReglementAMettreAJour.Idagencevoyageber = Model.IDAgenceVoyage;
				BEReglementAMettreAJour.Matagentvalber = MatAgentSignataire;

				_titreDeTransportContext.Bereglement.Update(BEReglementAMettreAJour);
				_titreDeTransportContext.SaveChanges();

				var ListeReferencesFactures = ListeFactures.Select(fact => fact.RefFacture);
				foreach (var item in ListeFactures)
				{
					var Resultat = _titreDeTransportContext.Facture
						.Where(fact => ListeReferencesFactures.Contains(fact.Numerofactureagence))
						.ToList();

					if (Resultat.Count != 0)
					{
						throw new Exception("Une des référénces de facture existe déjà. Veuillez vérifier et corrigez.");
					}
				}

				var FactureListe = new List<TitreDeTransportModelsEF.Facture>();
				foreach (var item in ListeFactures)
				{
					var NouvelleFacture = new TitreDeTransportModelsEF.Facture()
					{
						Datesaisifacture = DateTime.Now,
						Matagentvalfacture = MatAgentSignataire,
						Idetapefacture = EtapeSaisieBEReglement.Idetape,
						Matagentsaifacture = MatAgentSaisie,
						Idagencevoyagefacture = Model.IDAgenceVoyage,
						Dateagenceemissionfacture = item.DateEmission,
						Numerofactureagence = item.RefFacture,
						Idbereglem = BEReglementAMettreAJour.Idbereglem
					};

					_titreDeTransportContext.Facture.Add(NouvelleFacture);
					_titreDeTransportContext.SaveChanges();


					foreach (var item1 in item.ListeTitres)
					{
						var Titre = _titreDeTransportContext.Titredetransport.Single(_ => _.Idtitre == item1.IDTitre);
						Titre.Idfacture = NouvelleFacture.Idfacture;
						_titreDeTransportContext.Titredetransport.Update(Titre);
					}
				}

				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}

		public async Task<AffichageBERAvecFacturesLier> ObtenirDetailsBER(int IDBEReglement)
		{
			try
			{
				var BEReglement = await _titreDeTransportContext.Bereglement.SingleAsync(item => item.Idbereglem == IDBEReglement);
				if (BEReglement is null)
				{
					throw new Exception("Le BE de règlement est introuvable.");
				}

				var EtapeActuelle = _titreDeTransportContext.Etape.FirstOrDefault(item => item.Numeroetape == ConstantesEtapes.SAISI_BEREGLEMENT);
				if (EtapeActuelle is null)
				{
					throw new Exception("Etape non trouvée.");
				}

				var Resultats = await (from ber in _titreDeTransportContext.Bereglement
									   join agence in _titreDeTransportContext.Agencevoyage on ber.Idagencevoyageber equals agence.Idagencevoyage
									   where ber.Idbereglem == IDBEReglement
									   select new AffichageBERAvecFacturesLier
									   {
										   IDBEReglement = ber.Idbereglem,
										   DateSaisie = ber.Datesaiber,
										   DateValidation = ber.Datevalber,
										   IDEtape = ber.Idetapeber,
										   IDAgenceVoyage = ber.Idagencevoyageber,
										   IDRenvoi = ber.Idrenvoiber,
										   ListeFacturesLier = (from facture in _titreDeTransportContext.Facture
																join agence in _titreDeTransportContext.Agencevoyage on facture.Idagencevoyagefacture equals agence.Idagencevoyage
																where facture.Idbereglem == IDBEReglement
																select new AffichageFactureAvecPJ()
																{
																	IDAgenceVoyage = facture.Idagencevoyagefacture,
																	DateSaisie = facture.Datesaisifacture,
																	DateValidation = facture.Datevalfacture,
																	DateEmission = facture.Dateagenceemissionfacture,
																	IDEtape = facture.Idetapefacture,
																	IDFacture = facture.Idfacture,
																	IDRenvoi = facture.Idrenvoifacture,
																	LibelleAgence = agence.Libelleagencevoyage,
																	ListePiecesJointes = (from piecej in _titreDeTransportContext.Piecejointe
																						  where piecej.Idfacturepiecej == facture.Idfacture
																						  select new FichierAfficher()
																						  {
																							  CheminAbsolu = _fichierService.ObtenirCheminAbsoluFichier(piecej.Nompiecej),
																							  Nom = piecej.Nompiecej
																						  }).ToList(),
																	ListeTitres = new List<AffichageTitreAvecPJ>()
																}).ToList()
									   }).ToListAsync();

				return Resultats.First();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<BERDetails> ObtenirDetailsBERV2(int IDBEReglement)
		{
			try
			{
				var BEReglement = await _titreDeTransportContext.Bereglement.SingleAsync(item => item.Idbereglem == IDBEReglement);
				if (BEReglement is null)
				{
					throw new Exception("Le BE de règlement est introuvable.");
				}

				var EtapeActuelle = _titreDeTransportContext.Etape.FirstOrDefault(item => item.Numeroetape == ConstantesEtapes.SAISI_BEREGLEMENT);
				if (EtapeActuelle is null)
				{
					throw new Exception("Etape non trouvée.");
				}

				var Resultats = (from ber in _titreDeTransportContext.Bereglement
								 where ber.Idbereglem == IDBEReglement
								 select new BERDetails()
								 {
									 IDAgenceVoyage = ber.Idagencevoyageber,
									 NomSignataire = "",
									 MatSignataire = ber.Matagentvalber,
									 ListeFacture = (from fact in _titreDeTransportContext.Facture
													 where fact.Idbereglem == IDBEReglement
													 select new BERFactureModel()
													 {
														 ListeTitres = (from titre in _titreDeTransportContext.Titredetransport
																		join classe in _titreDeTransportContext.Classevoyage on titre.Idclasse equals classe.Idclasse
																		where titre.Idfacture == fact.Idfacture
																		select new AffichageTitreAutoCompletion()
																		{
																			ClasseVoyage = classe.Libelleclasse,
																			IDTitre = titre.Idtitre,
																			MatAgent = titre.Matagentbeneficiaire,
																			NomBeneficiaire = "ABC",
																			RefTitre = titre.Reftitret,
																			Routing = titre.Routing
																		}).ToList(),
														 RefFacture = fact.Numerofactureagence,
														 DateEmission = fact.Dateagenceemissionfacture
													 }).ToList()
								 }).ToList().First();

				foreach (var fact in Resultats.ListeFacture)
				{
					foreach (var titre in fact.ListeTitres)
					{
						titre.NomBeneficiaire = _sapCenterDBContext.Agentssap.Single(age => age.Matricule == titre.MatAgent).NomEtPrenoms;
					}
				}

				Resultats.NomSignataire = (_sapCenterDBContext.Agentssap.Single(age => age.Matricule == Resultats.MatSignataire)).NomEtPrenoms;

				return Resultats;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<AffichageRenvoi> ObtenirDetailsRenvoi(int IDRenvoi)
		{
			try
			{
				var Resultat = await _titreDeTransportContext.Renvoi.SingleAsync(item => item.Idrenvoi == IDRenvoi);
				var NomAgentRenvoi = await _sapCenterDBContext.Agentssap.Where(agent => agent.Matricule == Resultat.Matagentrenvoi).Select(agent => agent.NomEtPrenoms).SingleAsync();

				var DetailsDuRenvoi = new AffichageRenvoi
				{
					Daterenvoi = Resultat.Daterenvoi,
					Motifrenvoi = Resultat.Motifrenvoi,
					NomAgentRenvoi = NomAgentRenvoi
				};

				return DetailsDuRenvoi;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<List<AffichageFactureAvecPJ>> ObtenirFacturesAChoisir(int IDAgenceVoyage)
		{
			try
			{
				var EtapeActuelle = _titreDeTransportContext.Etape.First(item => item.Numeroetape == ConstantesEtapes.SAISI_BEREGLEMENT);

				var ResultatRequete = await (from facture in _titreDeTransportContext.Facture
											 join agence in _titreDeTransportContext.Agencevoyage on facture.Idagencevoyagefacture equals agence.Idagencevoyage
											 where facture.Idagencevoyagefacture == IDAgenceVoyage
											 where facture.Idetapefacture == EtapeActuelle.Idetape
											 where facture.Matagentvalfacture != null
											 where facture.Datevalfacture != null
											 where facture.Idbereglem == null
											 select new AffichageFactureAvecPJ
											 {
												 DateSaisie = facture.Datesaisifacture,
												 DateValidation = facture.Datevalfacture,
												 IDEtape = facture.Idetapefacture,
												 LibelleAgence = agence.Libelleagencevoyage,
												 NombreDeBillets = facture.Titredetransport.Count(),
												 IDFacture = facture.Idfacture,
												 IDRenvoi = facture.Idrenvoifacture,
												 Observation = facture.Obserfacture,
												 ListePiecesJointes = (from piecej in _titreDeTransportContext.Piecejointe
																	   where piecej.Idfacturepiecej == facture.Idfacture
																	   select new FichierAfficher()
																	   {
																		   CheminAbsolu = _fichierService.ObtenirCheminAbsoluFichier(piecej.Nompiecej),
																		   Nom = piecej.Nompiecej
																	   }).ToList(),
												 ListeTitres = new List<AffichageTitreAvecPJ>()
											 }).ToListAsync();

				return ResultatRequete;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<List<AffichageFactureAvecPJ>> ObtenirFacturesAChoisirModifier(int IDAgenceVoyage)
		{
			try
			{
				var EtapeActuelle = _titreDeTransportContext.Etape.First(item => item.Numeroetape == ConstantesEtapes.SAISI_BEREGLEMENT);

				var ResultatRequete = await (from facture in _titreDeTransportContext.Facture
											 join agence in _titreDeTransportContext.Agencevoyage on facture.Idagencevoyagefacture equals agence.Idagencevoyage
											 where facture.Idagencevoyagefacture == IDAgenceVoyage
											 where facture.Idetapefacture == EtapeActuelle.Idetape
											 where facture.Matagentvalfacture != null
											 where facture.Datevalfacture != null
											 select new AffichageFactureAvecPJ
											 {
												 DateSaisie = facture.Datesaisifacture,
												 DateValidation = facture.Datevalfacture,
												 IDEtape = facture.Idetapefacture,
												 LibelleAgence = agence.Libelleagencevoyage,
												 NombreDeBillets = facture.Titredetransport.Count(),
												 IDFacture = facture.Idfacture,
												 IDRenvoi = facture.Idrenvoifacture,
												 Observation = facture.Obserfacture,
												 ListePiecesJointes = (from piecej in _titreDeTransportContext.Piecejointe
																	   where piecej.Idfacturepiecej == facture.Idfacture
																	   select new FichierAfficher()
																	   {
																		   CheminAbsolu = _fichierService.ObtenirCheminAbsoluFichier(piecej.Nompiecej),
																		   Nom = piecej.Nompiecej
																	   }).ToList(),
												 ListeTitres = new List<AffichageTitreAvecPJ>()
											 }).ToListAsync();

				return ResultatRequete;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<List<AffichageBER>> ObtenirListeBER(SaisieBERRechercheModel ModelDeRecherche)
		{
			try
			{
				var EtapeActuelle = _titreDeTransportContext.Etape.Single(item => item.Numeroetape == ConstantesEtapes.SAISI_BEREGLEMENT);
				var EtapeValidation = _titreDeTransportContext.Etape.First(item => item.Numeroetape == ConstantesEtapes.VALID_BEREGLEMENT);

				var ResultatRequete = (from ber in _titreDeTransportContext.Bereglement
									   join agence in _titreDeTransportContext.Agencevoyage on ber.Idagencevoyageber equals agence.Idagencevoyage
									   join etape in _titreDeTransportContext.Etape on ber.Idetapeber equals etape.Idetape
									   select new AffichageBER
									   {
										   DateSaisie = ber.Datesaiber,
										   DateValidation = ber.Datevalber,
										   Observation = ber.Obserber,
										   IDBEReglement = ber.Idbereglem,
										   IDEtape = ber.Idetapeber,
										   IDRenvoi = ber.Idrenvoiber,
										   LibelleAgenceVoyage = agence.Libelleagencevoyage
									   }
				); ;

				if (ModelDeRecherche.DateDebut.HasValue)
				{
					ResultatRequete = ResultatRequete.Where(ber => ber.DateSaisie >= ModelDeRecherche.DateDebut.Value);
				}

				if (ModelDeRecherche.DateFin.HasValue)
				{
					ResultatRequete = ResultatRequete.Where(ber => ber.DateSaisie <= ModelDeRecherche.DateFin.Value);
				}

				if (!string.IsNullOrEmpty(ModelDeRecherche.TermeDeRecherche))
				{
					ResultatRequete = ResultatRequete
						.Where(ber => EF.Functions.Like(ber.IDBEReglement.ToString(), "%" + ModelDeRecherche.TermeDeRecherche + "%") ||
										 EF.Functions.Like(ber.LibelleAgenceVoyage.ToString(), "%" + ModelDeRecherche.TermeDeRecherche + "%"));
				}

				if (ModelDeRecherche.EnCours.HasValue && ModelDeRecherche.EnCours == true)
				{
					ResultatRequete = ResultatRequete.Where(ber => ber.DateSaisie != null && ber.DateValidation == null && ber.IDEtape == EtapeActuelle.Idetape && ber.IDRenvoi == null);
				}

				if (ModelDeRecherche.Valides.HasValue && ModelDeRecherche.Valides == true)
				{
					ResultatRequete = ResultatRequete.Where(ber => ber.DateSaisie != null && ber.DateValidation != null && ber.IDEtape == EtapeValidation.Idetape);
				}

				if (ModelDeRecherche.Rejetes.HasValue && ModelDeRecherche.Rejetes == true)
				{
					ResultatRequete = ResultatRequete.Where(ber => ber.DateValidation == null && ber.IDRenvoi != null && ber.IDEtape == EtapeActuelle.Idetape);
				}

				return await ResultatRequete.ToListAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task SupprimerBER(int IDBEReglement)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var BEReglementASupprimer = await _titreDeTransportContext.Bereglement.SingleAsync(ber => ber.Idbereglem == IDBEReglement);
				if (BEReglementASupprimer is null)
				{
					throw new Exception("BE de Règlement introuvable.");
				}

				var ListeFactures = _titreDeTransportContext.Facture.Where(fact => fact.Idbereglem == IDBEReglement);

				foreach (var item in ListeFactures)
				{
					item.Idbereglem = null;
				}
				_titreDeTransportContext.Facture.UpdateRange(ListeFactures);
				_titreDeTransportContext.SaveChanges();

				_titreDeTransportContext.Bereglement.Remove(BEReglementASupprimer);
				await _titreDeTransportContext.SaveChangesAsync();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}

		public async Task SupprimerBERV2(int IDBEReglement)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var BEReglementASupprimer = await _titreDeTransportContext.Bereglement.SingleAsync(ber => ber.Idbereglem == IDBEReglement);
				if (BEReglementASupprimer is null)
				{
					throw new Exception("BE de Règlement introuvable.");
				}

				var ListeFactures = _titreDeTransportContext.Facture.Where(fact => fact.Idbereglem == IDBEReglement).ToList();

				foreach (var fact in ListeFactures)
				{
					var ListeTitreLier = _titreDeTransportContext.Titredetransport.Where(titre => fact.Idfacture == titre.Idfacture).ToList();

					foreach (var titre in ListeTitreLier)
					{
						titre.Idfacture = null;
						_titreDeTransportContext.Titredetransport.Update(titre);
						_titreDeTransportContext.SaveChanges();
					}

					_titreDeTransportContext.Facture.Remove(fact);
					_titreDeTransportContext.SaveChanges();
				}

				_titreDeTransportContext.Bereglement.Remove(BEReglementASupprimer);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}

		public async Task<ImprimerBERModel> ObtenirModelBERAImprimer(int IDBEReglement)
		{
			try
			{
				var BEReglement = await _titreDeTransportContext.Bereglement.SingleAsync(ber => ber.Idbereglem == IDBEReglement);
				if (BEReglement == null)
				{
					throw new Exception("BE de règlement introuvable.");
				}

				var Resultats = (from ber in _titreDeTransportContext.Bereglement
								 join agence in _titreDeTransportContext.Agencevoyage on ber.Idagencevoyageber equals agence.Idagencevoyage
								 where ber.Idbereglem == IDBEReglement
								 select new ImprimerBERModel
								 {
									 DateSaisie = ber.Datesaiber,
									 LibelleAgence = agence.Libelleagencevoyage,
									 ListeLigneFacture = (from fact in _titreDeTransportContext.Facture
														  where fact.Idbereglem == IDBEReglement
														  select new LigneFacture
														  {
															  DateEmissionFacture = fact.Dateagenceemissionfacture,
															  RefFacture = fact.Numerofactureagence,
															  MontantTotalFacture = (from titre in _titreDeTransportContext.Titredetransport
																					 where titre.Idfacture == fact.Idfacture
																					 select new
																					 {
																						 montant = titre.Montanttitret
																					 }).Sum(_ => _.montant),
															  ListeAgentsBeneficiaires = (from titre in _titreDeTransportContext.Titredetransport
																						  where titre.Idfacture == fact.Idfacture
																						  select new LigneFactureAgent
																						  {
																							  MatriculeBeneficiaire = titre.Matagentbeneficiaire,
																							  RefTitreBeneficiaire = titre.Reftitret
																						  }).ToList(),
														  }).ToList(),
									 Signataire = new LigneSignataire()
									 {
										 MatSignataire = ber.Matagentvalber,
										 NomSignataire = "",
										 FonctionSignataire = ""
									 }
								 }).ToList().First();

				var Signataire = _sapCenterDBContext.Agentssap.Single(age => age.Matricule == Resultats.Signataire.MatSignataire);
				Resultats.Signataire.NomSignataire = Signataire.NomEtPrenoms;
				Resultats.Signataire.FonctionSignataire = Signataire.Fonction;

				foreach (var ligneFacture in Resultats.ListeLigneFacture)
				{
					var ListePassager = ligneFacture.ListeAgentsBeneficiaires.Select(_ => new LigneFactureAgent()
					{
						NomBeneficiaire = _sapCenterDBContext.Agentssap.Single(agent => agent.Matricule == _.MatriculeBeneficiaire).NomEtPrenoms,
						MatriculeBeneficiaire = _.MatriculeBeneficiaire,
						RefTitreBeneficiaire = _.RefTitreBeneficiaire
					});

					ligneFacture.ListeAgentsBeneficiaires = ListePassager.ToList();
				}

				return Resultats;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<List<AffichageAgentAutoCompletion>> ObtenirAgent()
		{
			try
			{
				var Resultat = await _sapCenterDBContext.Agentssap
					.Where(age => EF.Functions.Like(age.Fonction, "%directeur%"))
					.Select(_ => new AffichageAgentAutoCompletion
					{
						MatAgent = _.Matricule,
						NomAgent = _.NomEtPrenoms
					}).ToListAsync();

				return Resultat;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<List<AffichageTitreAutoCompletion>> ObtenirTitreAutoCompletion(int IDAgenceVoyage)
		{
			try
			{
				var Resu = (from titre in _titreDeTransportContext.Titredetransport
							join classe in _titreDeTransportContext.Classevoyage on titre.Idclasse equals classe.Idclasse
							where titre.Idagencevoyage == IDAgenceVoyage
							where titre.Idfacture == null
							select new AffichageTitreAutoCompletion
							{
								ClasseVoyage = classe.Libelleclasse,
								Routing = titre.Routing,
								IDTitre = titre.Idtitre,
								NomBeneficiaire = "NOM",
								RefTitre = titre.Reftitret,
								MatAgent = titre.Matagentbeneficiaire
							}).ToList();

				var ListeTitre = Resu.Select(_ => new AffichageTitreAutoCompletion
				{
					Routing = _.Routing,
					MatAgent = _.MatAgent,
					ClasseVoyage = _.ClasseVoyage,
					RefTitre = _.RefTitre,
					IDTitre = _.IDTitre,
					NomBeneficiaire = _sapCenterDBContext.Agentssap.Single(age => age.Matricule == _.MatAgent).NomEtPrenoms
				}).ToList();

				return ListeTitre;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<List<AffichageTitreAutoCompletion>> ObtenirTitreAutoCompletionModifier(int IDBEReglement)
		{
			try
			{
				var BERAModififer = _titreDeTransportContext.Bereglement.Single(ber => ber.Idbereglem == IDBEReglement);

				var ListeTitreBERAModifier = await (from fact in _titreDeTransportContext.Facture
											  join titre in _titreDeTransportContext.Titredetransport on fact.Idfacture equals titre.Idfacture
											  join classe in _titreDeTransportContext.Classevoyage on titre.Idclasse equals classe.Idclasse
											  where fact.Idbereglem == IDBEReglement
											  select new AffichageTitreAutoCompletion
											  {
												  ClasseVoyage = classe.Libelleclasse,
												  Routing = titre.Routing,
												  IDTitre = titre.Idtitre,
												  NomBeneficiaire = "NOM",
												  RefTitre = titre.Reftitret,
												  MatAgent = titre.Matagentbeneficiaire
											  }).ToListAsync();


				var Resu = await (from titre in _titreDeTransportContext.Titredetransport
							join classe in _titreDeTransportContext.Classevoyage on titre.Idclasse equals classe.Idclasse
							where titre.Idagencevoyage == BERAModififer.Idagencevoyageber
							where titre.Idfacture == null
							select new AffichageTitreAutoCompletion
							{
								ClasseVoyage = classe.Libelleclasse,
								Routing = titre.Routing,
								IDTitre = titre.Idtitre,
								NomBeneficiaire = "NOM",
								RefTitre = titre.Reftitret,
								MatAgent = titre.Matagentbeneficiaire
							}).ToListAsync();


				Resu.AddRange(ListeTitreBERAModifier);


				var ListeTitre = Resu.Select(_ => new AffichageTitreAutoCompletion
				{
					Routing = _.Routing,
					MatAgent = _.MatAgent,
					ClasseVoyage = _.ClasseVoyage,
					RefTitre = _.RefTitre,
					IDTitre = _.IDTitre,
					NomBeneficiaire = _sapCenterDBContext.Agentssap.Single(age => age.Matricule == _.MatAgent).NomEtPrenoms
				}).OrderBy(_ => _.NomBeneficiaire).ToList();

				return ListeTitre;
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
