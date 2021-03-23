module Diseases

(*
https://www.ottawapublichealth.ca/en/professionals-and-partners/resources/Images/infection_chain_ccf_en.png
A chain of infection has 6 links;
  1. Infectious agent
  2. Reservoir
  3. Portal of exit
  4. Mode of transmission
  5. Portal of entry
  6. Susceptible host
*)

type InfectiousAgent =
  | Bacteria
  | Viruses
  | Parasites
  
type Reservoir =
  | People
  | Animals
  | Food
  | Water
  
type PortalOfExit =
  | Mouth
  | Blood

type ModeOfTransmission =
  | Contact
  | Droplets
  
type PortalOfEntry =
  | Mouth
  | CutsInSkin
  | Eyes
  
type SusceptibleHost =
  | Babies
  | Children
  | Elderly
  | Anyone
  
type ChainOfInfection =
  { InfectiousAgent : InfectiousAgent
    Reservoir : Reservoir list
    PortalOfExit : PortalOfExit list
    ModeOfTransmission : ModeOfTransmission list
    PortalOfEntry : PortalOfEntry list
    SusceptibleHost : SusceptibleHost list }
  
type Disease =
  { Name : string
    ChainOfInfection : ChainOfInfection }
  
type RiskLevel =
  | Pandemic
  | Epidemic
  | Sporadic
  
let createRiskLevel d =
  match d.ChainOfInfection.SusceptibleHost with
  | [Anyone] -> Pandemic
  | _ -> Sporadic