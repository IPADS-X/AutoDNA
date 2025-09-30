from langchain.prompts import PromptTemplate

hypothesis_prompt = '''
You are a helpful assistant that provides hypotheses explaining why a target is not being achieved.
Context:
    Last procedure executed:
    =====================WORKFLOW BEGIN=====================
    {last_workflow}
    =====================WORKFLOW END=====================
    Outdated optimization history (Do not generate similar hypotheses as below):
    {text}
    Optimization target (all hypotheses must be based on this):
    {optimize_target}
    IMPORTANT THINGS MUST BE NOTED:
    {note}
Response Requirements:
    1. Carefully consider multiple factors AS MUCH AS POSSIBLE (contains physical, chemical, biological, and other factors).
        Present hypotheses you have thought in a numbered list format.
        Avoid using similar hypotheses have been listed before.
    2. Choose 1 from above at last.
        You should give the hypotheses you think best
    Each entry must contain:
    a) One clear hypotheses for the unmet target
'''

        # Avoid using similar hypotheses have been listed before.


paper_prompt = '''
Give answer in a json format. Don't include any other information.
Keys:
    "is_valid" : "true" or "false", shows whether you can answer this question.
    "answers" : a list of answers, each answer is a string.
Notes:
    1. Carefully consider multiple suggestions AS MUCH AS POSSIBLE.
        When answers, list methods in list format.
    2. If can not answer, just return "is_valid" as "false" and "answers" as an empty list.
'''

self_answer_prompt = '''
You are a helpful assistant that provides a suggestion to improve the target based on the hypothesis.
Context:
    Last workflow executed (may contain irrelevant experiment steps):
    =====================WORKFLOW BEGIN=====================
    {last_workflow}
    =====================WORKFLOW END=====================
    Optimization target (all hypothesis must be based on this):
    {optimize_target}
    Hypothesis:
    {hypothesis}
Response Requirements:
    1. Generate a suggestion to improve the target based on the hypothesis.
    2. The suggestion should be a clear and concise action that only changes one thing in the workflow.
'''

    # Question:
    # {question}
    # Response Requirements:
        # 1. Generate a suggestion to improve the target based on the hypothesis and the question.
        # 2. The suggestion should be a clear and concise action that only changes one thing in the workflow.


    # 1. Carefully consider multiple suggestions AS MUCH AS POSSIBLE.
    #     Present suggestions you have thought in a numbered list format.
    #     Avoid using similar suggestions have been listed before.
    # 2. Generate the most **feasible and brute-force** suggestion to improve the target based on the hypothesis.
    #     this suggestion may be benefical to other aspects of the workflow.
    # 3. The suggestion should be a clear and concise action that only changes one thing in the workflow.


solution_prompt = PromptTemplate.from_template(
'''
You are a helpful assistant that provides solutions to improve the target based on the hypothesis.
Context:
    Last procedure executed :
    {last_procedure}
    The hypothesis is:
    {text}

Response Requirements:
1. Give your answers to the hypothesis above. You must provide an optimizing advice to improve the target based on the hypothesis. The advice must be a specific and actionable step that could be taken into original experiment procedure. You don't need to provide a modified procedure, just give the optimizing advice only.
2. The advice should be a clear and concise action that only changes one thing in the procedure.

Information you can refer to (may contain redundant information):
{context}
--- START OF DOCUMENT: .\papers\manuals\beads_manual.pdf ---


--- Page 1 ---

[TABLE 1]
                                                       For coupling of nucleic acids                                                                                 For RNA applications
Binding and washing (B&W) Buffer (2X):\n10 mM Tris-HCl (pH 7.5)\n1 mM EDTA\n2 M NaCl Solution A:\nDEPC-treated 0.1 M NaOH\nDEPC-treated 0.05 M NaCl\nSolution B:\nDEPC-treated 0.1 M NaCl
[END TABLE]


[TABLE 2]
Cat. No. Volume Concentration
   65305   2 mL     10 mg /mL
   65306  10 mL     10 mg /mL
[END TABLE]


[TABLE 3]
         Biotinylated target Binding/mg
          Free Biotin (pmol)    650–900
Biotinylated peptides (pmol)       ~200
  Biotinylated antibody (µg)        ~10
               ds DNA (µg) *        ~10
ss oligonucleotides (pmol) *       ~200
[END TABLE]

Table 1 Recommended buffers and solutions
For coupling of nucleic acids For RNA applications
Binding and washing (B&W) Buffer (2X): Solution A:
Dynabeads™ M-270 Streptavidin
10 mM Tris-HCl (pH 7.5) DEPC-treated 0.1 M NaOH
1 mM EDTA DEPC-treated 0.05 M NaCl
2 M NaCl
Solution B:
Catalog Nos. 65305, 65306 Store at 2˚C to 8˚C DEPC-treated 0.1 M NaCl
Publication No. MAN0008449 Rev. B.0 The salt concentration and pH (typically 5–9) of the chosen binding/washing buffers can be
varied depending on the type of molecule to be immobilized. Beads with immobilized molecules
are stable in common buffers.
Product contents Required materials Both the size of the molecule to be immobilized and the biotinylation procedure
will affect the binding capacity. The capacity for biotinylated molecules depends on
• DynaMag™ Magnet (See steric availability and charge interaction between bead and molecule and between
Cat. No. Volume Concentration thermofisher.com/magnets for molecules. There are two or three biotin binding sites available for each streptavidin
65305 2 mL 10 mg /mL recommendations on magnets molecule on the surface of the bead after immobilization.
appropriate for manual or
65306 10 mL 10 mg /mL • Optimize the quantity of beads used for each individual application by titration.
automated protocols)
• Use up to two-fold excess of the binding capacity of the biotinylated molecule to
• Mixing device with tilting and
saturate streptavidin.
Dynabeads™ M-270 Streptavidin
rotation (e.g. HulaMixer™ Sample
contains 10 mg (6–7 × 108) magnetic • Binding efficiency can be determined by comparing molecule concentration
Mixer)
beads/mL, in 0.0065 M phosphate buffer before and after coupling.
• Buffers and Solutions, see Table 1
pH 7.4, with 0.14 M NaCl and 0.02%
sodium azide as a preservative. Caution: • For biotinylation details, download Protocol
Sodium azide may react with lead the Molecular Probes Handbook
and copper plumbing to form highly from thermofisher.com/handbook Recommended washing buffers
explosive metal azides.
• Nucleic acid applications: 1X B&W Buffer (see Table 1 for recipe). Dilute to
General guidelines 1X B&W Buffer with distilled water.
Product description
• Keep the tube on the magnet for • Antibody/protein applications: PBS, pH 7.4.
Dynabeads™ M-270 Streptavidin are ideal 2 min to ensure that all the beads
Wash Dynabeads™ magnetic beads
for nucleic acid applications, specifically are collected on the tube wall.
with samples with a high chaotropic Calculate the amount of beads required based on their binding capacity (see Table 2),
• For diluted samples, increase the
salt concentration, immunoassays and transfer the beads to a new tube.
incubation time or divide the sample
involving small biotinylated antigens into several smaller aliquots. 1. Resuspend the beads in the vial (i.e. vortex for >30 sec, or tilt and rotate for 5 min).
and applications that are not compatible
• Avoid air bubbles during pipetting. 2. Transfer the desired volume of beads to a tube.
with bovine serum albumin (BSA) (these
beads are not blocked with BSA). • Free biotin in the sample reduces 3. Add an equal volume of Buffer, or at least 1 mL and resuspend.
the binding capacity of the beads. A 4. Place the tube on a magnet for 1 min and discard the supernatant.
Add the beads to a sample containing
disposable separation column or a
biotinylated molecules, e.g. peptides 5. Remove the tube from the magnet and resuspend the washed beads in the same
spin column can be used to remove
or oligonucleotides. During a short volume of Buffer as the initial volume of beads taken from the vial (step 2).
unincorporated biotin.
incubation, the biotinylated molecule 6. Repeat steps 4–5 twice, for a total of 3 washes.
binds to the beads. Separate the • For some applications, it can be
molecule-bead complex with a magnet. an advantage to add a detergent Table 2 Typical binding capacity for 1 mg (100 µL) of Dynabeads™ magnetic beads
Capture, washing, and detection can such as 0.01–0.1% Tween™-20 to the
be optimized for manual or automated washing/binding buffers to reduce Biotinylated target Binding/mg
use. With indirect target capture, mix the non-specific binding. Free Biotin (pmol) 650–900
biotinylated molecule with the sample • Run the PCR with limiting Biotinylated peptides (pmol) ~200
to capture the molecule-target complex concentrations of biotinylated
Biotinylated antibody (µg) ~10
before adding the beads. primer, or remove free biotinylated
ds DNA (µg) * ~10
Indirect target capture can be primer by ultrafiltration, micro-
advantageous when molecule-target dialysis, or other clean-up protocols ss oligonucleotides (pmol) * ~200
kinetics are slow, affinity is weak, (PCR clean-up products are available
* Oligonucleotides and DNA fragments
molecule concentration is low, or from thermofisher.com). For oligonucleotides, capacity is inversely related to molecule size (number of bases). Reduced binding capacity
for large DNA fragments may be due to steric hindrance.
molecule-target binding requires optimal • Use a mixer to tilt/rotate the tubes
molecule orientation and true liquid- so Dynabeads™ magnetic beads do
phase kinetics. not settle at the bottom of the tube.
For research use only. Not for use in diagnostic procedures.

--- Page 2 ---

[TABLE 1]
                          Product Cat. No.
    Dynabeads™ M-280 Streptavidin   11205D
Dynabeads™ MyOne™ Streptavidin C1    65001
Dynabeads™ MyOne™ Streptavidin T1    65601
  Dynabeads™ Kit kilobaseBINDER™*    60101
                DynaMag™-2 Magnet   12321D
          HulaMixer™ Sample Mixer   15920D
[END TABLE]

Dynabeads™ magnetic beads for RNA manipulation Automation
Dynabeads™ Streptavidin are not supplied in RNase-free solutions. When using the Magnetic separation and handling using Dynabeads™ magnetic beads can easily be
beads for RNA applications, perform the following steps after washing: automated on a wide variety of liquid handling platforms. Dynabeads™ MyOne™
1. Wash the beads twice in Solution A for 2 min. Use the same volume (or greater) Streptavidin C1 share similar properties to Dynabeads™ M-270 Streptavidin but are
of Solution A as the initial volume of beads taken from the vial. smaller, making them ideal for automation applications due to their small size, low
sedimentation rate and high magnetic mobility. Selected protocols are available at
2. Wash the beads once in Solution B. Use the same volume as with Solution A.
thermofisher.com/automation.
3. Resuspend the beads in Solution B.
The beads are now ready to be coated with the biotinylated molecule of your choice. Description of materials
Immobilization protocol Dynabeads™ M-270 Streptavidin are uniform, superparamagnetic beads of 2.8 µm in
Wash the Dynabeads™ magnetic beads according to “Wash Dynabeads™ magnetic diameter with a streptavidin monolayer covalently coupled to the surface. This layer
ensures negligible streptavidin leakage while the lack of excess adsorbed streptavidin
beads” section before use.
ensures batch consistency and reproducibility of results.
1. Add the biotinylated molecule to the washed beads.
2. Incubate for 15–30 min at room temperature with gentle rotation of the tube. Related products
3. Place the tube in a magnet for 2–3 min and discard the supernatant.
4. Wash the coated beads 3–4 times in washing buffer. Product Cat. No.
5. Resuspend to desired concentration in a suitable buffer for your downstream use. Dynabeads™ M-280 Streptavidin 11205D
Here are some examples of immobilization protocols for specific applications. Dynabeads™ MyOne™ Streptavidin C1 65001
Dynabeads™ MyOne™ Streptavidin T1 65601
Immobilize nucleic acids
Dynabeads™ Kit kilobaseBINDER™* 60101
1. Resuspend beads in 2X B&W Buffer to a final concentration of 5 µg/µL (twice
original volume). DynaMag™-2 Magnet 12321D
2. To immobilize, add an equal volume of the biotinylated DNA/RNA in distilled HulaMixer™ Sample Mixer 15920D
water to dilute the NaCl concentration in the 2X B&W Buffer from 2 M to 1 M for * For biotinylated DNA fragments >2 kb.
optimal binding.
REF on labels is the symbol for catalog number.
3. Incubate for 15 min at room temperature using gentle rotation. Incubation time
depends on the nucleic acid length: short oligonucleotides (<30 bases) require
Important licensing information
max. 10 min. DNA fragments up to 1 kb require 15 min.
These products may be covered by one or more Limited Use Label Licenses. By use
4. Separate the biotinylated DNA/RNA coated beads with a magnet for 2–3 min.
of these products, you accept the terms and conditions of all applicable Limited Use
5. Wash 2–3 times with a 1X B&W Buffer. Label Licenses.
6. Resuspend to the desired concentration. Binding is now complete. Resuspend Manufactured by Thermo Fisher Scientific AS, Ullernchausseen 52 PO Box 114,
the beads with the immobilized DNA/RNA fragment in a buffer with low salt Smestad, Olso, Norway N-0379 and by Thermo Fisher Scientific Baltics UAB, V.A.
concentration, suitable for downstream applications. Graiciuno 8, LT-02241 Vilnius, Lithuania. Thermo Fisher Scientific AS and Thermo
Fisher Scientific Baltics UAB comply with the Quality System Standards ISO 9001 and
Immobilize antibodies/proteins
ISO 13485.
1. Incubate the beads and biotinylated antibodies in PBS for 30 min at room
Limited product warranty
temperature using gentle rotation.
2. Separate the antibody-coated beads with a magnet for 2–3 min. Life Technologies Corporation and/or its affiliate(s) warrant their products as set
forth in the Life Technologies' General Terms and Conditions of Sale found on Life
3. Wash the coated beads 4–5 times in PBS containing 0.1% BSA.
Technologies' website at www.thermofisher.com/us/en/home/global/terms-and-
4. Resuspend to the desired concentration for your application.
conditions.html. If you have any questions, please contact Life Technologies at www.
thermofisher.com/support.
Corporate entity: Life Technologies | Carlsbad, CA 92008 USA | Toll Free in USA 1.800.955.6288
©2018 Thermo Fisher Scientific Inc. All rights reserved. All trademarks are the property of Thermo Fisher Scientific and its
subsidiaries unless otherwise specified.Tween is a registered trademark of Croda Americas, PLC
DISCLAIMER: TO THE EXTENT ALLOWED BY LAW, LIFE TECHNOLOGIES AND/OR ITS AFFILIATE(S) WILL NOT BE LIABLE FOR SPECIAL,
INCIDENTAL, INDIRECT, PUNITIVE, MULTIPLE OR CONSEQUENTIAL DAMAGES IN CONNECTION WITH OR ARISING FROM THIS DOCUMENT,
INCLUDING YOUR USE OF IT.
For support visit thermofisher.com/support or
email techsupport@lifetech.com
thermofisher.com
14 June 2018


--- END OF DOCUMENT: .\papers\manuals\beads_manual.pdf ---


--- START OF DOCUMENT: .\papers\manuals\firebird.pdf ---


--- Page 1 ---

[TABLE 1]
                Terminator                                             Description                                         Catalog Numbers
   Ready to Use (Untagged)    3’-ONH2. Does not require\ndeprotection prior to use TONH2-171,CONH2-172, AO NH2-\n173, GONH2-174, IONH2-175
Oxime Protected (Untagged) 3'-acetoxime. Requires\ndeprotection with methoxylamine TONH2-101,CONH2-102, AO NH2-\n103, GONH2-104, IONH2-105
[END TABLE]

Firebird Biomolecular Sciences, LLC
13709 Progress Blvd, Box 17
Alachua, FL 32615
(386) 418-0347
support@firebirdbio.com
TECH-100 ver.1
Technical Product Information Sheet
Using Untagged Aminoxy Reversible Terminators with
DNA Polymerases
Firebird Reversible Terminators:
Terminator Description Catalog Numbers
3’-ONH2. Does not require TONH2-171,
Ready to Use (Untagged)
deprotection prior to use
May 2019
This Report replaces Technical Report NONH2-21 for use of untagged reversible terminators
Firebird Biomolecular Sciences Technical Product Information Sheet TECH-100 (001)
Using Untagged Aminoxy Reversible Terminators Page 1 of 6
CONH2-172, AONH2-
173, GONH2-174, IONH2-175
3'-acetoxime. Requires TONH2-101,
Oxime Protected (Untagged)
deprotection with methoxylamine
CONH2-102 , AONH2-
103, GONH2-104, IONH2-105

--- Page 2 ---
OVERVIEW
The reagents
3'-Aminoxy triphosphates have the hydrogen on their 3'-hydroxyl (-O-H) group replaced by an
–NH2 group (3'-O-NH2). These triphosphates can be used by many enzymes to add a nucleotide with a 3'-
ONH2 moiety to a primer in either a template-directed fashion (as with DNA polymerases and reverse
transcriptases) or without template direction (as with terminal transferases). This Technical Information
sheet concerns the use of 3'-aminoxy nucleoside triphosphates with DNA polymerases.
Figure 1. Aminoxy terminated triphosphates
Thymine Ready to Use Cytosine Ready to Use Adenine Ready to Use Guanine Ready to Use
TONH2-171 CONH2-172 AONH2-173 GONH2-174
The polymerases
One preferred polymerase for incorporating 3’-aminoxy triphosphates is Therminator™, sold by New
England Biolabs (Cat#: M0261S). For those wishing to use aminoxy triphosphates in competition with
irreversibly terminating 2',3'-dideoxynucleoside triphosphates (for example, to detect SNPs), Firebird’s
POL475 polymerase (Cat # POL475) is recommended.
Resuming primer extension after incorporation of the 3’-Aminoxy nucleotide
Once an aminoxy terminated nucleotide is added to the 3'-end of a DNA strand, primer extension
terminates. The 3'-ONH group can then be reacted with other functional groups. Alternatively, the 3'-ONH
2 2
group can be cleaved to generate a natural 3'-OH group, at which point the DNA strand can be used like
standard DNA, including for further cycles of extension with 3'-aminoxy terminators.
The preferred reagent for cleavage of the 3'-ONH2 group to generate a 3'-OH is sodium nitrite buffered in
aqueous acetate at pH 5.5 at room temperature. This cleavage reaction is complete in a few minutes. While
these reaction conditions also cause deamination of the standard nucleobases C, A and G, that side reaction
is at least 1,000 times slower than the rate of cleavage of the 3'-ONH2 group (data shown below, Page 3).
Cautions
The 3'-ONH2 group forms oximes (R-O-N=CR2) with aldehydes and ketones, which are poor polymerase
substrates. Therefore, care must be taken not to expose the reagents to acetone, formaldehyde, or other
aldehydes and ketones, which are widespread in many laboratories. Since standard Milli-Q water may
contain small organic species, we recommend HPLC-grade water or water distilled from potassium
permanganate for the preparation of all buffers or solutions. Care should also be taken not to use glassware
rinsed with acetone. Conversely, the oxime protected untagged species must have the oxime deprotected
prior to use, as described further on Page 6.
One way to manage ketone/aldehyde contamination is to add excess methoxylamine (MeONH2) to the
reaction mixture. This scavenges any aldehyde or ketone, wherein the methoxylamine forms an oxime with
the aldehyde or ketone (MeO-N=CR2). This protects the reversible terminator from conversion to its oxime.
The aminoxy reversible terminators obtained from their acetoximes by our in- situ protocol (Page 6) already
contain a sufficient excess of methoxylamine. Methoxylamine does not generally interfere with polymerase
activity. It must, however, be removed before cleavage of the 3'-O-NH2 with buffered sodium nitrite. If the
primer-template complex is immobilized, this is done by washing away solutions containing
methoxylamine.
Firebird Biomolecular Sciences Technical Product Information Sheet TECH-100 (001)
Using Untagged Aminoxy Reversible Terminators Page 2 of 6

--- Page 3 ---

[TABLE 1]
 mM conc of actual pH (± 0.02) product after 1 min product after 2 min
N35a0N O\n2               5.50                  ND                 90%
        700               5.50                 98%                >99%
        700               5.65                 80%                 96%
[END TABLE]


[TABLE 2]
nucleoside byproducts after 72 h, UV quantitated at 260 nm
        dG                                             20%
        dA                                             13%
        dC                                             15%
[END TABLE]

Additionally, care must be taken to remove any methoxylamine from solution prior to storage of unused
reversible terminator, especially for cytosine terminators, as methoxylamine at high concentrations will
degrade the cytosine nucleobase over extended periods of time. Other cautions reflect those routinely taken
with triphosphates in general: store frozen, avoid contamination by bacteria, avoid heating.
MODEL REACTIONS
Experimental data, shown below, helps the informed practitioner better understand the performance
characteristic of the aminoxy triphosphates.
Cleavage of 3'-O-aminothymidine with buffered aqueous HONO
A procedure on the nucleoside itself illustrates the scope of the cleavage reaction, whose rate is sensitive to
pH. To show this, cleavage reagent (50 µL, 350-700 mM NaNO2 in 1 M aqueous sodium acetate buffer)
was added to an aqueous solution of 3'-O-aminothymidine (20 mM, 2 µL total volume) at a range of pH's.
The resulting pH was measured with a microelectrode. Parallel reactions were stopped after incubation at
room temperature for 1-2 min by raising the pH through the addition of potassium phosphate buffer (1 M,
200 µL, pH 7.0). The products were analyzed by analytical reversed-phase HPLC (Waters NovaPak column
C-18 4µm, 3.9x150, with guard column Waters NovaPak C-18 4µm, 3.9x15mm, eluent A = 25 mM TEAA
pH 7, eluent B = acetonitrile, gradient from 3% B to 13% B in 20 min, flow rate = 0.5 mL/min, Rt =
product: 8 min; starting material: 11 min). The amount of cleavage was determined by integrating the UV
absorbance peaks (267 nm) of the remaining 3'-O-aminothymidine and the product (thymidine). Results are
shown in Table 1.
Table 1. Amounts of cleavage of 3'-O-aminothymidine
mM conc of actual pH (± 0.02) product after 1 min product after 2 min
N35a0N O 5.50 ND 90%
2
700 5.50 98% >99%
700 5.65 80% 96%
ND – Not Determined
Testing for deamination of exocyclic amino group
To estimate the extent of deamination of various nucleobases under cleavage conditions, aqueous solutions
of 2'-deoxyguanosine, 2'-deoxyadenosine and 2'-deoxycytidine (20 mM, 30
µL) were separately treated with cleavage buffer (700 mM NaNO2, 1 M aqueous sodium acetate buffer, pH
5.5, 500 µL) at room temperature for 72 h. Aliquots (50 µL) were removed, neutralized by the addition of
potassium phosphate buffer (1 M, 200 µL, pH 7), and analyzed by analytical reversed-phase HPLC (Waters
NovaPak C-18 column 4µm, 3.9x150 mm, with guard column Waters NovaPak C-18 4µm, 3.9x15mm.
eluent A = 25 mM TEAA pH 7, eluent B = acetonitrile, gradient from 0% B to 3% B in 10 min, then to 30%
B in 20 min, flow rate = 0.5 mL/min, Rt = dG: 14 min, dA: 18 min, dC: 8 min). The amount of deamination
was determined by integrating the UV absorbance peaks (260 nm) of the remaining starting material
(nucleoside) and the product(s). The results are shown in Table 2.
Table 2. Extent of deamination (exocyclic amino group) caused by cleavage reaction after 72 hours
nucleoside byproducts after 72 h, UV quantitated at 260 nm
dG 20%
dA 13%
dC 15%
Firebird Biomolecular Sciences Technical Product Information Sheet TECH-100 (001)
Using Untagged Aminoxy Reversible Terminators Page 3 of 6

--- Page 4 ---
PRIMER EXTENSION USING AMINOXY NUCLEOSIDE TRIPHOSPHATES
Experimental data shown below used the aminoxy triphosphates to extend with termination a primer-template
complex that is immobilized on magnetic beads. This permits a wash cycle to remove reagents in solution
from DNA on the bead.
Cycle of polymerase extension with termination followed by cleavage on a support
Reagents and substrates
(a) dNTP-ONH2 stock solutions: Solutions of the triphosphates (5 mM) are prepared in-situ
following the cleavage protocol on Page 6.
(b) Thermopol® Buffer: The 10X solution (200 mM Tris-HCl, 100 mM (NH4)2SO4, 100 mM KCl, 20
mM MgSO4, 1% Triton X-100, pH 8.8 at 25 °C), diluted to 1X (final) in HPLC grade H2O.
(c) Dynabeads® M-270 Streptavidin: Magnetic beads carrying streptavidin were purchased from
Invitrogen (Cat number 653-06). Biotinylated nucleic acids were immobilized on the beads following the
manufacturer's procedure (Dynabeads product insert).
(d) Cleavage Reagent: This reagent is prepared fresh as an aqueous solution of NaNO2 (700 mM) in an
aqueous sodium acetate buffer (1 M, pH 5.5).
(e) Quench Buffer: 10 mM EDTA in HPLC grade H2O.
(f) 1% MeONH2 stock solution: Dilute 100 µL of methoxylamine-hydrochloride solution (25-30% in
water, e.g. Alfa Aesar Cat number L08415) with 2.6 mL of water (HPLC grade or better) and adjust pH
to ca. 7 with aqueous NaOH solution (10 M, ca. 30-50 µL).
(g) Primary Wash Buffer: 50 mM Tris pH 7.5, 500 µM EDTA, 1 M NaCl, 0.1% MeONH2.
(h) Post Extension Wash buffer: 50 mM Tris pH 8.5, 0.1% MeONH2.
(i) Therminator™: Purchased from New England Biolabs (Cat #: M0261S); 2 U/µL.
Firebird Biomolecular Sciences Technical Product Information Sheet TECH-100 (001)
Using Untagged Aminoxy Reversible Terminators Page 4 of 6

--- Page 5 ---
Polymerase Extension Reaction
The reaction mixture contains:
(a) 5'-Biotinylated template in water (30 pmol)
(b) 32P labeled primer in water (2.5 pmol)
(c) Unlabeled primer in water (20 pmol)
(d) Thermopol Buffer (1X)
(e) dNTP-ONH2, diluted from stock to 100 µM (if multiple dNTPs are used, they are diluted into the same
mixture to a final concentration of 100 µM each), adjusted as desired.
(f) Therminator 0.5 U per assay, adjusted as desired.
(g) ddH2O to bring the final reaction volume up to 10 µL
In this example, the biotinylated template is bound to magnetic beads, permitting extension and wash steps
that employ magnetic separation to change the buffer. In a 10 µL reaction volume, 5'-biotinylated template (30
pmol), complementary 5'-32P-labeled primer (2.5 pmol) and unlabeled primer (20 pmol) are annealed by
incubation at 96 °C for 5 min, followed by slow cooling (over 1 hour) to room temperature. The duplex is then
immobilized onto Dynabeads following the manufacturer's protocol (Dynabeads M- 270 Streptavidin,
Invitrogen). The beads carrying the immobilized DNA are resuspended in Thermopol buffer (1X) containing
0.5 Unit of Therminator and incubated (72 °C, 30 sec). The dNTP-ONH2 triphosphate (100 µM final of each)
is added to initiate the reaction, with incubating at 72 °C for 2 min. Extension is stopped with EDTA (3.33
mM, final concentration).
Cleavage Reaction to Reverse Termination
The magnetic beads are recovered with a small, hand-held magnet, and the quench buffer is largely removed
by pipette. Two aliquots of Primary Wash Buffer (500 µL each) are sequentially added and removed, and the
beads are washed with two aliquots of Post Extension Wash Buffer (500 µL each).
The Post Extension Wash Buffer is then removed. Cleavage Reagent (100 µL) is added, and the mixture is
incubated at room temperature for 5 min. This step is typically repeated.
Following cleavage, the beads are recovered, the Cleavage Reagent is removed, and the beads are washed
with two aliquots of Primary Wash Buffer (500 µL each).
To prepare the bead-bound primer-template complex for another round of extension, the beads are washed
with two aliquots of 1X Thermopol buffer (500 µL each).
Clean-up of 3'-OH contamination
All 3'-aminoxy triphosphates contain traces (< 0.2%) of their natural analog carrying a free 3'- OH that cannot
be removed by physical purification methods such as HPLC. Since polymerases prefer these natural
triphosphates over their 3'-aminoxy counterparts, even trace levels of contamination can lead to a significant
degree (several percent) of unblocked extension ("phasing") during each elongation cycle.
This preference of polymerases for natural triphosphates (i.e. the "contaminant") over the 3'-aminoxy analogs
led us to develop an easy in-situ clean-up procedure that completely prevents phasing: prior to the start of the
biological assay, each 3'-aminoxy triphosphate is incubated under extension conditions with polymerase and a
small amount of "trapping duplex". This trapping duplex consists of a primer and template where the template
enables the repeated incorporation of that particular nucleobase. As an example, to clean-up 3'-aminoxy
thymidine triphosphate, the template would contain an oligo-A overhang:
5'-GCG TAA TAC GAC TCA CTA TGG ACG-3'
3'-CGC ATT ATG CTG AGT GAT ACC TGC AAA AAA AAA AAA-5’
During this incubation, the polymerase will exhaust the entire trace of 3'-OH contaminant, leaving behind a
concentrated stock solution of the clean 3'-aminoxy triphosphate, ready to use.
Firebird Biomolecular Sciences Technical Product Information Sheet TECH-100 (001)
Using Untagged Aminoxy Reversible Terminators Page 5 of 6

--- Page 6 ---
The following protocol produces 15 µL of a 1 mM stock solution of clean TTP-ONH2:
TTP-ONH2 (10 mM, "contaminated") 1.5 µL 15 nmol
Trapping duplex (25 µM, annealed) 2.0 µL 50 pmol
Thermopol buffer (10X) 1.5 µL
ddH2O 8.5 µL
POL475 polymerase 1.5 µL Incubate
at 72 °C for 2 min. Keep on ice until ready to use.
Since this clean-up procedure focuses on the incorporation of natural triphosphates, a different polymerase
could be used, as long as it is compatible with the Thermopol buffer and does not interfere with the subsequent
assay.
USING AMINOXY REVERSIBLE TERMINATORS DELIVERED AS OXIMES
As an alternative to ready-to-use terminators, we also offer reversible terminators with the 3’-ONH group
2
protected as the acetoxime (TONH2-101, CONH2-102, AONH2-103, GONH2-104), which are more suitable
for longer term storage (years). These are not incorporated by polymerases, however, and must be treated with
methoxylamine (Me-ONH ) according to the protocol below to deprotect the oxime and yield aminoxy
2
triphosphates that can be used directly in the polymerase extension reaction.
Figure 2. Aminoxy terminated triphosphates protected as acetoximes
Thymine Oxime Cytosine Oxime Adenine Oxime Guanine Oxime
TONH2-101 CONH2-102 AONH2-103 GONH2-104
Deprotection of terminator oximes in-situ for subsequent polymerase incorporation.
The triphosphate-oximes (TONH2-101, CONH2-102, AONH2-103, GONH2- 104) must be
deoximated before they can be used as polymerase substrates. First, a sample of the lyophilized
terminator-oxime (5 µmol) is dissolved in 500 µL of water (HPLC grade or better) to give a 10 mM
stock solution. This stock solution may be stored at -20°C.
A buffered aqueous MeONH2 solution is prepared as follows (the following recipe yields about
1.9 mL and should be prepared fresh daily): 200 µL of a commercial methoxylamine-hydrochloride
solution (25-30% in water, e.g. Alfa Aesar Catalog number L08415) is diluted with 1.2 mL of water
(HPLC grade or better); the pH of this solution will be below 1. To this is added aqueous sodium
hydroxide solution (10 M) in portions until the pH is about 5-6, approximately 60-90 µL (depending on
the exact concentration of the particular batch of MeONH2•HCl solution). This solution is buffered by
the addition of 400 µL of aqueous sodium acetate solution (1 M, pH 5.5).
Immediately before use, an aliquot of the terminator-oxime stock solution (10 mM) is mixed with an
equal volume of the buffered aqueous MeONH2 solution, and the mixture is incubated at room
temperature for 1 h.
The resulting solution is 5 mM dNTP-ONH2, in ca. 200 mM in MeONH2, ca. 200 mM in NaCl, and
100 mM in sodium acetate buffer, with a pH of 5.5. The solution can be used directly with polymerases
(after appropriate dilution to the desired triphosphate concentration). To prevent degradation of the
nucleobases by the high concentration of methoxylamine, this solution should not be stored as-is but
rather diluted to the final terminator concentration and used within 24 h; alternatively, the excess
methoxylamine can be removed by freeze-drying followed by re-dissolving the terminator in water. The
resulting solution can then be stored frozen at -20 °C for several months.
Firebird Biomolecular Sciences Technical Product Information Sheet TECH-100 (001)
Using Untagged Aminoxy Reversible Terminators Page 6 of 6


--- END OF DOCUMENT: .\papers\manuals\firebird.pdf ---
'''
)