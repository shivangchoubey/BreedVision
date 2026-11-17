using BreedVision.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BreedVision.Web.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(BreedVisionDbContext context, ILogger logger)
        {
            try
            {
                await context.Database.EnsureCreatedAsync();

                if (await context.Breeds.AnyAsync())
                {
                    logger.LogInformation("Breed catalog already seeded ({Count} breeds present).", await context.Breeds.CountAsync());
                    return;
                }

                logger.LogInformation("Seeding 41-breed ICAR-NBAGR Indian cattle and buffalo catalog...");

                var breeds = new List<Breed>
                {
                    // ═══════════════════════════════════════════════
                    //  BUFFALO BREEDS (8)
                    // ═══════════════════════════════════════════════
                    new() {
                        Name = "Bhadawari", NormalizedKey = "bhadawari", Species = "Buffalo",
                        ScientificName = "Bubalus bubalis",
                        OriginState = "Uttar Pradesh (Agra, Etawah) & Madhya Pradesh (Bhind, Morena)",
                        PrimaryUtility = "Dairy & Draught",
                        AverageMilkYield = "1,200 – 1,800 kg per lactation (Fat: 8–13%)",
                        HornCharacteristics = "Flat, sword-shaped horns running backward, slightly upturned at tips.",
                        PhysicalTraits = "Copper-colored coat; two white chevron lines (Janeva) on lower neck; compact, heat-tolerant body.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_BUFFALO_2000_BHADAWARI_01001)",
                        ReferenceImageUrl = "/images/breeds/bhadawari.jpg",
                        Description = "Bhadawari produces milk with the highest butterfat content (8–13%) among all buffalo breeds, thriving in sub-tropical conditions."
                    },
                    new() {
                        Name = "Jaffrabadi", NormalizedKey = "jaffrabadi", Species = "Buffalo",
                        ScientificName = "Bubalus bubalis",
                        OriginState = "Gujarat (Jaffrabad, Gir Somnath, Amreli)",
                        PrimaryUtility = "Dairy",
                        AverageMilkYield = "2,500 – 4,200 kg per lactation (Fat: 7–8.5%)",
                        HornCharacteristics = "Heavy, drooping horns curling backwards and upward with recurved tips — the heaviest horns of any buffalo breed.",
                        PhysicalTraits = "Largest Indian buffalo breed; massive black body; prominent bulging forehead; white markings on face and lower legs.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_BUFFALO_2000_JAFFARABADI_01004)",
                        ReferenceImageUrl = "/images/breeds/jaffrabadi.jpg",
                        Description = "Jaffrabadi is India's largest dairy buffalo breed, prized for heavy milk yields and massive horns curved like a hairpin."
                    },
                    new() {
                        Name = "Mehsana", NormalizedKey = "mehsana", Species = "Buffalo",
                        ScientificName = "Bubalus bubalis",
                        OriginState = "Gujarat (Mehsana, Patan, Banaskantha, Sabarkantha)",
                        PrimaryUtility = "Dairy",
                        AverageMilkYield = "2,200 – 4,000 kg per lactation",
                        HornCharacteristics = "Sickle-shaped or irregular scimitar-curved horns, less drooping than Murrah.",
                        PhysicalTraits = "Longer face than Murrah; lighter body; black coat with white patches on muzzle and tail switch.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_BUFFALO_2000_MEHSANA_01006)",
                        ReferenceImageUrl = "/images/breeds/mehsana.jpg",
                        Description = "Mehsana is a high-producing dairy buffalo from northern Gujarat, believed to be a cross between Murrah and Surti."
                    },
                    new() {
                        Name = "Murrah", NormalizedKey = "murrah", Species = "Buffalo",
                        ScientificName = "Bubalus bubalis",
                        OriginState = "Haryana (Hisar, Rohtak, Bhiwani) & Punjab",
                        PrimaryUtility = "Dairy",
                        AverageMilkYield = "2,200 – 3,200 kg per lactation (Fat: 7.5–9%)",
                        HornCharacteristics = "Short, tightly coiled corkscrew-curled horns.",
                        PhysicalTraits = "Jet-black compact body; white markings on tail switch and muzzle; massive build; wedge-shaped dairy conformation.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_BUFFALO_2000_MURRAH_01007)",
                        ReferenceImageUrl = "/images/breeds/murrah.jpg",
                        Description = "Murrah is the world's highest-yielding dairy buffalo, widely exported to Brazil, Italy and over 25 countries for breed improvement."
                    },
                    new() {
                        Name = "Nagpuri", NormalizedKey = "nagpuri", Species = "Buffalo",
                        ScientificName = "Bubalus bubalis",
                        OriginState = "Maharashtra (Nagpur, Wardha, Vidarbha region)",
                        PrimaryUtility = "Draught & Dairy",
                        AverageMilkYield = "600 – 900 kg per lactation",
                        HornCharacteristics = "Long, flat, bow-shaped horns sweeping backward flat on the neck.",
                        PhysicalTraits = "Flat, long horns lying flat on neck and back; black coat; medium body suited for heavy agricultural work in Vidarbha.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_BUFFALO_2000_NAGPURI_01008)",
                        ReferenceImageUrl = "/images/breeds/nagpuri.jpg",
                        Description = "Nagpuri (Elitchpuri) buffalo is Maharashtra's work buffalo, recognized by its flat back-swept horns and suitability for field traction."
                    },
                    new() {
                        Name = "Nili Ravi", NormalizedKey = "nili_ravi", Species = "Buffalo",
                        ScientificName = "Bubalus bubalis",
                        OriginState = "Punjab (Nili: Sutlej/Ravi rivers; Ravi: Lahore region, shared with Pakistan)",
                        PrimaryUtility = "Dairy",
                        AverageMilkYield = "2,000 – 2,800 kg per lactation (Fat: 7–8%)",
                        HornCharacteristics = "Small tightly-curved horns.",
                        PhysicalTraits = "Black coat; distinctive white markings: wall eyes (blue iris), white muzzle, white tail switch, white stockings on legs; bulging forehead.",
                        ConservationStatus = "Indigenous — shared Trans-border Breed (ICAR-NBAGR registered)",
                        ReferenceImageUrl = "/images/breeds/nili_ravi.jpg",
                        Description = "Nili-Ravi is Pakistan/India's most important dairy buffalo distinguished by wall eyes and white facial markings; comparable to Murrah in milk yield."
                    },
                    new() {
                        Name = "Surti", NormalizedKey = "surti", Species = "Buffalo",
                        ScientificName = "Bubalus bubalis",
                        OriginState = "Gujarat (Vadodara, Kheda, Surat, Anand, Kaira)",
                        PrimaryUtility = "Dairy",
                        AverageMilkYield = "1,600 – 2,200 kg per lactation (Fat: 7–8.5%)",
                        HornCharacteristics = "Sickle-shaped horns curving downward and backward.",
                        PhysicalTraits = "Reddish or rust-brown coat; medium-sized elegant dairy frame; two white collars of hair on neck.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_BUFFALO_2000_SURTI_01010)",
                        ReferenceImageUrl = "/images/breeds/surti.jpg",
                        Description = "Surti is a distinctive medium-sized dairy buffalo of Gujarat, recognized by its reddish coat and two white rings around the neck."
                    },
                    new() {
                        Name = "Toda", NormalizedKey = "toda", Species = "Buffalo",
                        ScientificName = "Bubalus bubalis",
                        OriginState = "Tamil Nadu (Nilgiri Hills)",
                        PrimaryUtility = "Dairy (Sacred)",
                        AverageMilkYield = "400 – 700 kg per lactation (Fat: 8–9%)",
                        HornCharacteristics = "Wide-spreading, slightly curved horns.",
                        PhysicalTraits = "Light buff to reddish-brown coat; massive sturdy build; short legs; associated with the Toda tribal community of the Nilgiris.",
                        ConservationStatus = "Indigenous — Endangered (ICAR-NBAGR INDIA_BUFFALO_2000_TODA_01011)",
                        ReferenceImageUrl = "/images/breeds/toda.jpg",
                        Description = "The Toda buffalo is a rare, sacred breed of the Nilgiri Hills kept by the Toda tribe, known for high-fat milk used in traditional ritual dairy products."
                    },

                    // ═══════════════════════════════════════════════
                    //  CATTLE BREEDS (33)
                    // ═══════════════════════════════════════════════
                    new() {
                        Name = "Alambadi", NormalizedKey = "alambadi", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Tamil Nadu (Dharmapuri, Krishnagiri, Salem)",
                        PrimaryUtility = "Draught",
                        AverageMilkYield = "400 – 600 kg per lactation",
                        HornCharacteristics = "Medium-length horns curving forward and inward.",
                        PhysicalTraits = "Grey to dark grey compact muscular body; short upright ears; active gait suited for hilly Cauvery terrain.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_2300_ALAMBADI_03001)",
                        ReferenceImageUrl = "/images/breeds/alambadi.jpg",
                        Description = "Alambadi is a compact draught breed from the Cauvery valley of Tamil Nadu, hardy and well-adapted to tropical hill agriculture."
                    },
                    new() {
                        Name = "Amritmahal", NormalizedKey = "amritmahal", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Karnataka (Hassan, Chikmagalur, Mysore)",
                        PrimaryUtility = "Draught",
                        AverageMilkYield = "500 – 700 kg per lactation",
                        HornCharacteristics = "Long, forward-pointing, lyre-shaped horns — a hallmark feature.",
                        PhysicalTraits = "Grey body with dark markings on head and neck; compact muscular build; aggressive temperament; traditionally used by Hyder Ali's army.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_2300_AMRITMAHAL_03002)",
                        ReferenceImageUrl = "/images/breeds/amritmahal.jpg",
                        Description = "Amritmahal ('Herd of Nectar') is Karnataka's royal draught breed, famed for speed, endurance, and the most striking forward-swept horns in India."
                    },
                    new() {
                        Name = "Ayrshire", NormalizedKey = "ayrshire", Species = "Cattle",
                        ScientificName = "Bos taurus",
                        OriginState = "Scotland (Exotic — present in Indian dairy farms)",
                        PrimaryUtility = "Dairy",
                        AverageMilkYield = "4,500 – 6,000 kg per lactation",
                        HornCharacteristics = "Medium upward-curving horns (now often dehorned).",
                        PhysicalTraits = "Red-and-white patched coat; wedge-shaped dairy conformation; medium-sized; well-adapted to Indian hill stations.",
                        ConservationStatus = "Exotic Breed (Introduced)",
                        ReferenceImageUrl = "/images/breeds/ayrshire.jpg",
                        Description = "Ayrshire is a Scottish dairy breed adapted in Indian hill farms for high milk production, recognizable by red-and-white coat patterns."
                    },
                    new() {
                        Name = "Banni", NormalizedKey = "banni", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Gujarat (Banni grasslands, Kutch)",
                        PrimaryUtility = "Dairy",
                        AverageMilkYield = "1,200 – 2,000 kg per lactation",
                        HornCharacteristics = "Medium horns curving outward and upward.",
                        PhysicalTraits = "White to light grey robust body; tolerant to Kutch's extreme salinity and heat; kept by the Maldhari pastoral community.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR registered — recently listed)",
                        ReferenceImageUrl = "/images/breeds/banni.jpg",
                        Description = "Banni is Kutch's pastoral dairy breed, raised by Maldhari nomads in one of India's harshest environments, with remarkable heat and drought resilience."
                    },
                    new() {
                        Name = "Bargur", NormalizedKey = "bargur", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Tamil Nadu (Erode — Bargur hills, Sathyamangalam forest)",
                        PrimaryUtility = "Draught",
                        AverageMilkYield = "400 – 600 kg per lactation",
                        HornCharacteristics = "Long tapering horns sweeping outward and backward.",
                        PhysicalTraits = "Brown or reddish-brown with white or cream speckles/patches; agile and fast; adapted to forest hill terrain.",
                        ConservationStatus = "Indigenous — Vulnerable (ICAR-NBAGR INDIA_CATTLE_2300_BARGUR_03005)",
                        ReferenceImageUrl = "/images/breeds/bargur.jpg",
                        Description = "Bargur is a fleet-footed forest draught breed from the Bargur hills, prized for speed and agility in rough terrain and hill agriculture."
                    },
                    new() {
                        Name = "Brown Swiss", NormalizedKey = "brown_swiss", Species = "Cattle",
                        ScientificName = "Bos taurus",
                        OriginState = "Switzerland (Exotic — used in crossbreeding programs across India)",
                        PrimaryUtility = "Dual-Purpose (Dairy & Draught)",
                        AverageMilkYield = "5,000 – 7,000 kg per lactation",
                        HornCharacteristics = "Medium, lyre-shaped horns (often dehorned).",
                        PhysicalTraits = "Uniform brown coat ranging from mouse-grey to dark brown; large body; known for longevity and high components milk.",
                        ConservationStatus = "Exotic Breed (Introduced)",
                        ReferenceImageUrl = "/images/breeds/brown_swiss.jpg",
                        Description = "Brown Swiss is one of the world's oldest dairy breeds, used in Indian crossbreeding for high milk volume combined with good butterfat content."
                    },
                    new() {
                        Name = "Dangi", NormalizedKey = "dangi", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Maharashtra (Nasik, Ahmednagar) & Gujarat (Panchmahal)",
                        PrimaryUtility = "Draught & Dairy",
                        AverageMilkYield = "600 – 900 kg per lactation",
                        HornCharacteristics = "Medium horns curving outward and upward.",
                        PhysicalTraits = "Red-and-white or black-and-white mottled coat; compact muscular body; superb adaptability to heavy monsoon rainfall and hill terrain of Sahyadri.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_2300_DANGI_03007)",
                        ReferenceImageUrl = "/images/breeds/dangi.jpg",
                        Description = "Dangi is a robust dual-purpose breed from the Western Ghats, renowned for extreme drought resistance and working ability under heavy monsoon conditions."
                    },
                    new() {
                        Name = "Deoni", NormalizedKey = "deoni", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Karnataka & Telangana (Gulbarga, Latur, Nanded, Bidar)",
                        PrimaryUtility = "Dual-Purpose",
                        AverageMilkYield = "900 – 1,400 kg per lactation",
                        HornCharacteristics = "Medium horns curving outward, then upward.",
                        PhysicalTraits = "Bold black-and-white or red-and-white large patches on body; strong draft build; Gir-like convex forehead and pendulous ears.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_2300_DEONI_03008)",
                        ReferenceImageUrl = "/images/breeds/deoni.jpg",
                        Description = "Deoni ('Dongarpati') is a large dual-purpose breed of the Deccan Plateau with striking black-white markings, valued for both fieldwork and milk."
                    },
                    new() {
                        Name = "Gir", NormalizedKey = "gir", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Gujarat (Gir hills, Junagadh, Bhavnagar, Amreli, Rajkot)",
                        PrimaryUtility = "Dairy",
                        AverageMilkYield = "2,000 – 3,500 kg per lactation (Peak >4,000 kg)",
                        HornCharacteristics = "Half-moon shaped horns curving downward and backward, curling gracefully upward at tips.",
                        PhysicalTraits = "Convex dome-shaped forehead (heat radiator); long pendulous drooping leaf-folded ears; reddish-brown to speckled white coat; large hump and loose dewlap.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_0400_GIR_03008)",
                        ReferenceImageUrl = "/images/breeds/gir.jpg",
                        Description = "Gir is India's premier indigenous dairy Zebu, globally recognized for A2 milk, heat resilience, and disease resistance. Widely exported to Brazil."
                    },
                    new() {
                        Name = "Guernsey", NormalizedKey = "guernsey", Species = "Cattle",
                        ScientificName = "Bos taurus",
                        OriginState = "Guernsey Island (Exotic — present in hill stations)",
                        PrimaryUtility = "Dairy",
                        AverageMilkYield = "4,500 – 5,500 kg per lactation (High butterfat)",
                        HornCharacteristics = "Medium, forward-curving horns.",
                        PhysicalTraits = "Fawn/gold and white coat; known for golden-yellow milk rich in beta-carotene; medium build.",
                        ConservationStatus = "Exotic Breed (Introduced)",
                        ReferenceImageUrl = "/images/breeds/guernsey.jpg",
                        Description = "Guernsey produces golden-hued, high-butterfat milk naturally rich in beta-carotene; used in Indian premium dairy farms for specialty milk."
                    },
                    new() {
                        Name = "Hallikar", NormalizedKey = "hallikar", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Karnataka (Tumkur, Hassan, Mandya, Mysore)",
                        PrimaryUtility = "Draught",
                        AverageMilkYield = "400 – 600 kg per lactation",
                        HornCharacteristics = "Long, pointed horns curving backward, outward, and tipping upward — classic racing-bull horns.",
                        PhysicalTraits = "Grey to white medium body; compact; noted for spectacular gait and speed; used in traditional Karnataka bull races (Kambala).",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_2300_HALLIKAR_03010)",
                        ReferenceImageUrl = "/images/breeds/hallikar.jpg",
                        Description = "Hallikar is Karnataka's champion draught breed, famous for its regal gait, endurance, and participation in Kambala racing festivals."
                    },
                    new() {
                        Name = "Hariana", NormalizedKey = "hariana", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Haryana (Rohtak, Hisar, Jind, Bhiwani, Karnal)",
                        PrimaryUtility = "Dual-Purpose",
                        AverageMilkYield = "1,000 – 1,800 kg per lactation",
                        HornCharacteristics = "Short, stumpy horns emerging laterally, curving inward and upward.",
                        PhysicalTraits = "White to light grey elegant body; long narrow face; fine-boned; active gait; one of North India's most important dual-purpose breeds.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_0600_HARIANA_03011)",
                        ReferenceImageUrl = "/images/breeds/hariana.jpg",
                        Description = "Hariana is North India's prized dual-purpose breed: a reliable milker that also performs well as a draught animal across the Indo-Gangetic plains."
                    },
                    new() {
                        Name = "Holstein Friesian", NormalizedKey = "holstein_friesian", Species = "Cattle",
                        ScientificName = "Bos taurus",
                        OriginState = "Netherlands (Exotic — most common crossbred in India)",
                        PrimaryUtility = "Dairy",
                        AverageMilkYield = "6,000 – 10,000 kg per lactation",
                        HornCharacteristics = "Medium horns (usually dehorned commercially).",
                        PhysicalTraits = "Bold black-and-white patches; large body; highest milk yield of any breed; widely crossbred with Zebu breeds in India for commercial dairy.",
                        ConservationStatus = "Exotic Breed (Introduced)",
                        ReferenceImageUrl = "/images/breeds/holstein.jpg",
                        Description = "Holstein Friesian is the world's highest-yielding dairy breed, extensively crossbred in India for commercial milk production in organized farms."
                    },
                    new() {
                        Name = "Jersey", NormalizedKey = "jersey", Species = "Cattle",
                        ScientificName = "Bos taurus",
                        OriginState = "Jersey Island (Exotic — widely crossbred across India)",
                        PrimaryUtility = "Dairy",
                        AverageMilkYield = "3,500 – 5,500 kg per lactation (High fat 4.5–5.5%)",
                        HornCharacteristics = "Small fine horns (usually dehorned).",
                        PhysicalTraits = "Fawn, buff, or dark brown coat; distinctively large expressive eyes with black rims; small elegant frame; best feed conversion of any dairy breed.",
                        ConservationStatus = "Exotic Breed (Introduced)",
                        ReferenceImageUrl = "/images/breeds/jersey.jpg",
                        Description = "Jersey is the most heat-tolerant European breed, widely crossed in India for high butterfat and efficient feed-to-milk conversion on small farms."
                    },
                    new() {
                        Name = "Kangayam", NormalizedKey = "kangayam", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Tamil Nadu (Erode, Coimbatore, Tirupur — Kangayam tract)",
                        PrimaryUtility = "Draught",
                        AverageMilkYield = "400 – 700 kg per lactation",
                        HornCharacteristics = "Medium horns curving upward and slightly inward.",
                        PhysicalTraits = "Light grey to dark grey; compact, muscular powerful body; known for endurance walking long distances in black cotton soil.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_2300_KANGAYAM_03014)",
                        ReferenceImageUrl = "/images/breeds/kangayam.jpg",
                        Description = "Kangayam is Tamil Nadu's principal draught breed, capable of sustained heavy work in the black clay soils of Kongu Nadu."
                    },
                    new() {
                        Name = "Kankrej", NormalizedKey = "kankrej", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Gujarat (Banaskantha, Patan, Kutch, Surendranagar) & Rajasthan (Barmer, Jalore)",
                        PrimaryUtility = "Dual-Purpose",
                        AverageMilkYield = "1,500 – 2,600 kg per lactation",
                        HornCharacteristics = "Lyre-shaped horns that first grow laterally then sweep upward in graceful arcs.",
                        PhysicalTraits = "Silver-grey to iron-grey or steel colour; broad forehead; large drooping ears; heavy dewlap; speed and endurance on sandy terrain.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_0400_KANKREJ_03015)",
                        ReferenceImageUrl = "/images/breeds/kankrej.jpg",
                        Description = "Kankrej ('Wagad' or 'Wadhiyar') is Gujarat's fastest heavy draught breed, also a reliable milker, the genetic ancestor of the Brazilian Guzera breed."
                    },
                    new() {
                        Name = "Kasargod", NormalizedKey = "kasargod", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Kerala (Kasargod) & Karnataka (South Canara)",
                        PrimaryUtility = "Draught",
                        AverageMilkYield = "400 – 600 kg per lactation",
                        HornCharacteristics = "Medium, inward-curving horns.",
                        PhysicalTraits = "Light grey or brown; compact small-to-medium frame; adapted to Kerala's humid tropical conditions and paddy field work.",
                        ConservationStatus = "Indigenous — Endangered (ICAR-NBAGR registered)",
                        ReferenceImageUrl = "/images/breeds/kasargod.jpg",
                        Description = "Kasargod is a rare, endangered Zebu draught breed of Kerala's coastal Malabar region, adapted to waterlogged paddy field cultivation."
                    },
                    new() {
                        Name = "Kenkatha", NormalizedKey = "kenkatha", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Uttar Pradesh (Banda, Chitrakoot — Ken river valley) & Madhya Pradesh",
                        PrimaryUtility = "Draught",
                        AverageMilkYield = "400 – 600 kg per lactation",
                        HornCharacteristics = "Long, tapering horns curving forward with pointed tips.",
                        PhysicalTraits = "Grey to white; medium compact body; active and hardy for work in the Ken-Betwa rocky terrain.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_0900_KENKATHA_03016)",
                        ReferenceImageUrl = "/images/breeds/kenkatha.jpg",
                        Description = "Kenkatha is a sturdy small draught breed from the Ken river valley in Bundelkhand, adapted to rocky, semi-arid agricultural conditions."
                    },
                    new() {
                        Name = "Kherigarh", NormalizedKey = "kherigarh", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Uttar Pradesh (Lakhimpur Kheri — Terai belt)",
                        PrimaryUtility = "Draught",
                        AverageMilkYield = "400 – 700 kg per lactation",
                        HornCharacteristics = "Medium upward-curving horns.",
                        PhysicalTraits = "Light grey or white; medium compact frame; suited for heavy paddy field work in the Terai region of Uttar Pradesh.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_0900_KHERIGARH_03018)",
                        ReferenceImageUrl = "/images/breeds/kherigarh.jpg",
                        Description = "Kherigarh is a draught breed from the Terai sub-Himalayan belt of Uttar Pradesh, used in paddy cultivation of the flood-prone Lakhimpur region."
                    },
                    new() {
                        Name = "Khillari", NormalizedKey = "khillari", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Maharashtra (Satara, Solapur, Pune) & Karnataka (Dharwad)",
                        PrimaryUtility = "Draught",
                        AverageMilkYield = "500 – 800 kg per lactation",
                        HornCharacteristics = "Long tapering horns sweeping backward and outward with sharp tips.",
                        PhysicalTraits = "Grey to white with dark muzzle; fine-boned active body; known for speed and agility — one of India's fastest working cattle.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_1500_KHILLARI_03019)",
                        ReferenceImageUrl = "/images/breeds/khillari.jpg",
                        Description = "Khillari is Maharashtra's fleet-footed draught champion, capable of fast trotting pace on hard deccan roads, used historically by Maratha armies."
                    },
                    new() {
                        Name = "Krishna Valley", NormalizedKey = "krishna_valley", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Karnataka & Telangana (Krishna river valley — Bijapur, Gulbarga, Raichur)",
                        PrimaryUtility = "Draught",
                        AverageMilkYield = "600 – 900 kg per lactation",
                        HornCharacteristics = "Medium stumpy horns.",
                        PhysicalTraits = "Grey to white massive build; heavy-boned; one of the largest Indian draught breeds by body weight.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_2300_KRISHNAVALLEY_03020)",
                        ReferenceImageUrl = "/images/breeds/krishna_valley.jpg",
                        Description = "Krishna Valley is a massive heavy draught breed from the Krishna river valley, bred for the most demanding field operations on black cotton soils."
                    },
                    new() {
                        Name = "Malnad Gidda", NormalizedKey = "malnad_gidda", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Karnataka (Chikmagalur, Shimoga, Hassan — Western Ghats)",
                        PrimaryUtility = "Draught & Dairy",
                        AverageMilkYield = "600 – 900 kg per lactation (High fat)",
                        HornCharacteristics = "Small, inward-curving horns.",
                        PhysicalTraits = "Smallest Indian cattle breed; grey or dark grey tiny frame; extraordinary disease resistance in humid forest habitats.",
                        ConservationStatus = "Indigenous — Endangered (ICAR-NBAGR registered)",
                        ReferenceImageUrl = "/images/breeds/malnad_gidda.jpg",
                        Description = "Malnad Gidda is India's smallest cattle breed from the Western Ghats rainforests, valued for disease hardiness and high-fat forest-grazed milk."
                    },
                    new() {
                        Name = "Nagori", NormalizedKey = "nagori", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Rajasthan (Nagaur, Jodhpur, Pali, Ajmer)",
                        PrimaryUtility = "Draught",
                        AverageMilkYield = "500 – 800 kg per lactation",
                        HornCharacteristics = "Long, beautifully curved horns sweeping upward and backward.",
                        PhysicalTraits = "White to light grey; tall, long-legged elegant body; very fast walker across desert terrain; traditionally used in Rajasthan's cart transport.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_1700_NAGORI_03023)",
                        ReferenceImageUrl = "/images/breeds/nagori.jpg",
                        Description = "Nagori is Rajasthan's most elegant draught breed — tall, light-framed, and fast-walking, historically the fastest cart-pulling cattle across the Thar desert."
                    },
                    new() {
                        Name = "Nimari", NormalizedKey = "nimari", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Madhya Pradesh (Khargone, Barwani — Nimar region along Narmada)",
                        PrimaryUtility = "Draught",
                        AverageMilkYield = "600 – 900 kg per lactation",
                        HornCharacteristics = "Medium, outward-curving horns.",
                        PhysicalTraits = "Red to reddish-brown coat (Gir-influenced); medium compact body; strong for cultivation in the Narmada river valley.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_1200_NIMARI_03024)",
                        ReferenceImageUrl = "/images/breeds/nimari.jpg",
                        Description = "Nimari is a Narmada valley draught breed with Gir ancestry, exhibiting a distinctive reddish coat and dual-purpose potential."
                    },
                    new() {
                        Name = "Ongole", NormalizedKey = "ongole", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Andhra Pradesh (Prakasam — former Ongole district)",
                        PrimaryUtility = "Draught & Dairy",
                        AverageMilkYield = "1,500 – 2,600 kg per lactation",
                        HornCharacteristics = "Medium stumpy horns that may curve upward or outward; thick base.",
                        PhysicalTraits = "White with dark grey/black markings on head, neck, shoulder; massive frame; large pendulous dewlap; massive hump; tick-resistant white coat.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_0100_ONGOLE_03026)",
                        ReferenceImageUrl = "/images/breeds/ongole.jpg",
                        Description = "Ongole is India's largest indigenous cattle breed, prized worldwide for meat and work; ancestor of the American Brahman and Brazilian Nelore breeds."
                    },
                    new() {
                        Name = "Pulikulam", NormalizedKey = "pulikulam", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Tamil Nadu (Madurai, Sivaganga, Ramanathapuram)",
                        PrimaryUtility = "Draught (Jallikattu)",
                        AverageMilkYield = "400 – 600 kg per lactation",
                        HornCharacteristics = "Medium upward-curving horns.",
                        PhysicalTraits = "Grey to dark grey compact, muscular, aggressive body; primary breed used in Jallikattu (traditional Tamil Nadu bull-taming sport).",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_2300_PULIKULAM_03027)",
                        ReferenceImageUrl = "/images/breeds/pulikulam.jpg",
                        Description = "Pulikulam is the fierce agile bull of Jallikattu — Tamil Nadu's ancient tradition — bred for courage, speed, and defiant temperament."
                    },
                    new() {
                        Name = "Rathi", NormalizedKey = "rathi", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Rajasthan (Bikaner, Ganganagar, Hanumangarh)",
                        PrimaryUtility = "Dairy",
                        AverageMilkYield = "1,200 – 2,000 kg per lactation",
                        HornCharacteristics = "Short, outward-curving horns of medium length.",
                        PhysicalTraits = "Brown coat with white patches; medium build; tolerant of Rajasthan's harsh semi-arid climate; Sahiwal-Hariana cross influences.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_1700_RATHI_03028)",
                        ReferenceImageUrl = "/images/breeds/rathi.jpg",
                        Description = "Rathi is Rajasthan's principal dairy breed, a sister to Sahiwal but adapted to desert conditions; produces good milk with relatively low inputs."
                    },
                    new() {
                        Name = "Red Dane", NormalizedKey = "red_dane", Species = "Cattle",
                        ScientificName = "Bos taurus",
                        OriginState = "Denmark (Exotic — used in crossbreeding in India)",
                        PrimaryUtility = "Dairy",
                        AverageMilkYield = "5,000 – 7,000 kg per lactation",
                        HornCharacteristics = "Medium horns (often dehorned).",
                        PhysicalTraits = "Uniform deep red coat; medium to large build; excellent milk quality with high protein; heat-tolerant compared to other European breeds.",
                        ConservationStatus = "Exotic Breed (Introduced)",
                        ReferenceImageUrl = "/images/breeds/red_dane.jpg",
                        Description = "Red Dane is a Danish dairy breed used in Indian crossbreeding programs for high-quality A2 milk production with improved tropical heat tolerance."
                    },
                    new() {
                        Name = "Red Sindhi", NormalizedKey = "red_sindhi", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Sindh/Pakistan (now maintained in India: Punjab, Haryana, Kerala, Tamil Nadu)",
                        PrimaryUtility = "Dairy",
                        AverageMilkYield = "1,700 – 2,500 kg per lactation (Fat: 4.5–5%)",
                        HornCharacteristics = "Short stumpy horns curving outward with slightly darker tips.",
                        PhysicalTraits = "Deep cherry-red to dark mahogany coat; compact dairy frame; smaller and more compact than Sahiwal; tighter dewlap and sheath; robust heat tolerance.",
                        ConservationStatus = "Indigenous — Vulnerable (ICAR-NBAGR INDIA_CATTLE_0600_REDSINDHI_03029)",
                        ReferenceImageUrl = "/images/breeds/red_sindhi.jpg",
                        Description = "Red Sindhi is a premier A2 dairy Zebu from Sindh with deep red mahogany coat; sister to Sahiwal but more compact with higher butterfat."
                    },
                    new() {
                        Name = "Sahiwal", NormalizedKey = "sahiwal", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Punjab (Faisalabad-Montgomery region — shared with Pakistan)",
                        PrimaryUtility = "Dairy",
                        AverageMilkYield = "2,000 – 3,000 kg per lactation (Fat: 4.5–5%)",
                        HornCharacteristics = "Short stumpy horns emerging laterally, slightly curved.",
                        PhysicalTraits = "Reddish-dun to pale yellowish-red coat; voluminous pendulous dewlap; large loose navel flap (Lola); heavier dairy frame than Red Sindhi.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_0600_SAHIWAL_03030)",
                        ReferenceImageUrl = "/images/breeds/sahiwal.jpg",
                        Description = "Sahiwal is India's highest-yielding indigenous dairy Zebu; prized globally for A2 beta-casein milk, tick resistance, and tropical adaptability."
                    },
                    new() {
                        Name = "Tharparkar", NormalizedKey = "tharparkar", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Rajasthan (Jodhpur, Jaisalmer, Barmer) & Thar Desert",
                        PrimaryUtility = "Dual-Purpose (Dairy & Arid Draught)",
                        AverageMilkYield = "1,800 – 2,800 kg per lactation",
                        HornCharacteristics = "Medium horns curving outwards and upwards with sharp tips.",
                        PhysicalTraits = "Lustrous white or light grey coat reflecting solar radiation; darker face in winter; medium convex forehead; strong hooves for long desert walks.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR INDIA_CATTLE_1700_THARPARKAR_03028)",
                        ReferenceImageUrl = "/images/breeds/tharparkar.jpg",
                        Description = "Tharparkar ('White Sindhi') is an extraordinarily resilient desert breed maintaining robust milk yields under scarce forage and extreme heat."
                    },
                    new() {
                        Name = "Umblachery", NormalizedKey = "umblachery", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Tamil Nadu (Thanjavur, Nagapattinam, Tiruvarur — Cauvery delta)",
                        PrimaryUtility = "Draught",
                        AverageMilkYield = "400 – 600 kg per lactation",
                        HornCharacteristics = "Medium, inward-curving or twisted horns.",
                        PhysicalTraits = "Grey body; compact; adapted for working in the flooded paddy fields of the Cauvery delta; unique amphibious work capability.",
                        ConservationStatus = "Indigenous — Endangered (ICAR-NBAGR INDIA_CATTLE_2300_UMBLACHERY_03033)",
                        ReferenceImageUrl = "/images/breeds/umblachery.jpg",
                        Description = "Umblachery is a rare endangered draught breed uniquely adapted to waterlogged Cauvery delta paddy cultivation in coastal Tamil Nadu."
                    },
                    new() {
                        Name = "Vechur", NormalizedKey = "vechur", Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Kerala (Alappuzha, Kottayam — Vembanad backwaters)",
                        PrimaryUtility = "Dairy",
                        AverageMilkYield = "300 – 500 kg per lactation (High medicinal-quality fat)",
                        HornCharacteristics = "Very small, slightly curved horns.",
                        PhysicalTraits = "World's smallest cattle breed; tiny compact body (~90–100 cm height); brown or dark brown coat; revered in Ayurveda for unique medicinal milk properties.",
                        ConservationStatus = "Indigenous — Critically Endangered (ICAR-NBAGR registered — saved from extinction by KAU 1998)",
                        ReferenceImageUrl = "/images/breeds/vechur.jpg",
                        Description = "Vechur is the world's smallest cattle breed and a living treasure of Kerala, rescued from extinction; its milk is prized in Ayurvedic medicine."
                    },
                };

                await context.Breeds.AddRangeAsync(breeds);
                await context.SaveChangesAsync();
                logger.LogInformation("Successfully seeded {Count} breeds into the database.", breeds.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
}
