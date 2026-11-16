using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase(IParcoursRepository parcoursRepository)
{
    public async Task<Parcours> ExecuteAsync(string nomParcours, int anneeFormation)
    {
        var parcours = new Parcours{NomParcours = nomParcours, AnneeFormation = anneeFormation};
        return await ExecuteAsync(parcours);
    }
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours p = await parcoursRepository.CreateAsync(parcours);
        parcoursRepository.SaveChangesAsync().Wait();
        return p;
    }
    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(parcoursRepository);

        // On recherche un parcours avec le même nom
        List<Parcours> existe = await parcoursRepository.FindByConditionAsync(p => p.NomParcours.Equals(parcours.NomParcours));

        // Si un parcours avec le même nom existe déjà, on lève une exception
        if (existe is {Count:>0}) throw new DuplicateNomParcoursException(parcours.NomParcours + " - ce nom de parcours existe déjà");

        // L'année de formation doit être 1 ou 2 (Master 1 ou Master 2)
        if (parcours.AnneeFormation < 1 || parcours.AnneeFormation > 2) throw new InvalidAnneeFormationException(parcours.AnneeFormation + " - l'année de formation doit être 1 ou 2");
    }
}