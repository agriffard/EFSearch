# PRD – EFSearch

## 1. Overview
Nom du produit : EFSearch  
Type : Librairie .NET (NuGet)  
Cible : ASP.NET Core / EF Core / SQL Server  

Objectif : Fournir un moteur de recherche dynamique, typé et sécurisé basé sur des modèles de requêtes transformés en Expression Trees applicables à IQueryable<T>.

## 2. Problème
- Recherches codées manuellement et non réutilisables
- Filtres non typés, erreurs silencieuses
- Combinaisons AND/OR complexes difficiles
- Problèmes de performance SQL
- Dette technique importante

## 3. Objectifs
- Standardiser les recherches
- Garantir sécurité et performance
- Fournir une API simple et extensible
- Compatibilité totale EF Core

## 4. Modèle de requête
- Filters : Field / Operator / Value
- Sort : Field / Direction
- Pagination : PageNumber / PageSize

## 5. Fonctionnalités MVP
- Filtres dynamiques
- Tri dynamique
- Pagination
- Mapping typé (whitelist)
- Génération d’Expression<Func<T, bool>>

## 6. API publique
IQueryable<T> ApplySearch(IQueryable<T>, SearchRequest, SearchMap<T>)

## 7. Sécurité
- Champs whitelistés
- Opérateurs validés
- Limites de complexité
- Zéro SQL injection

## 8. Performance
- LINQ-to-Entities uniquement
- Filtres avant projection
- Pagination SQL native

## 9. Livrables
- Package NuGet
- README
- Tests unitaires
- Exemples
