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
                // Ensure database is created (runs migrations or creates schema)
                await context.Database.EnsureCreatedAsync();

                if (await context.Breeds.AnyAsync())
                {
                    logger.LogInformation("Breed catalog already seeded ({Count} breeds present).", await context.Breeds.CountAsync());
                    return;
                }

                logger.LogInformation("Seeding ICAR-NBAGR Indian cattle and buffalo breeds catalog...");

                var breeds = new List<Breed>
                {
                    new Breed
                    {
                        Name = "Bhadawari",
                        NormalizedKey = "bhadawari",
                        Species = "Buffalo",
                        ScientificName = "Bubalus bubalis",
                        OriginState = "Uttar Pradesh (Agra, Etawah) & Madhya Pradesh (Bhind, Morena)",
                        PrimaryUtility = "Dairy & Draught",
                        AverageMilkYield = "1,200 - 1,800 kg per lactation (Fat: 8% - 13%)",
                        HornCharacteristics = "Flat, sword-shaped horns running backwards alongside neck, turning slightly upward at tips.",
                        PhysicalTraits = "Characteristic copper-colored coat ('copper-skinned'), two distinct white lines ('Janeva' or chevron) on the lower neck, compact medium body, exceptional heat tolerance.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR Accession INDIA_BUFFALO_2000_BHADAWARI_01001)",
                        ReferenceImageUrl = "/images/breeds/bhadawari.jpg",
                        Description = "The Bhadawari is an indigenous river buffalo breed famous across India for producing milk with the highest butterfat content (often exceeding 10-12%), thriving under harsh sub-tropical climatic conditions."
                    },
                    new Breed
                    {
                        Name = "Gir",
                        NormalizedKey = "gir",
                        Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Gujarat (Gir hills, Junagadh, Bhavnagar, Amreli, Rajkot)",
                        PrimaryUtility = "Dairy",
                        AverageMilkYield = "2,000 - 3,500 kg per lactation (Peak > 4,000 kg)",
                        HornCharacteristics = "Half-moon shaped horns curving downwards and backwards, then curling gracefully upward at the tips.",
                        PhysicalTraits = "Prominent convex dome-shaped forehead acting as a natural heat radiator, long pendulous drooping ears folded like leaves, reddish-brown to speckled white coat, large hump and loose pendulous dewlap.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR Accession INDIA_CATTLE_0400_GIR_03008)",
                        ReferenceImageUrl = "/images/breeds/gir.jpg",
                        Description = "The Gir is one of India's premier indigenous dairy zebu cattle breeds, globally recognized for its high A2 milk production, heat resilience, resistance to tropical diseases, and docility."
                    },
                    new Breed
                    {
                        Name = "Jaffrabadi",
                        NormalizedKey = "jaffrabadi",
                        Species = "Buffalo",
                        ScientificName = "Bubalus bubalis",
                        OriginState = "Gujarat (Gir Forest, Saurashtra, Junagadh, Jamnagar)",
                        PrimaryUtility = "Dairy & Heavy Draught",
                        AverageMilkYield = "2,200 - 3,200 kg per lactation (Fat: 7.5% - 9%)",
                        HornCharacteristics = "Extremely heavy, broad, flat horns drooping downwards on either side of the neck and curling upwards like large hooks.",
                        PhysicalTraits = "Massive, powerful heavyweight body; prominent forehead with deep central groove; deep black coat; broad muzzle; largest and strongest buffalo breed in India.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR Accession INDIA_BUFFALO_0400_JAFFARABADI_01002)",
                        ReferenceImageUrl = "/images/breeds/jaffrabadi.jpg",
                        Description = "Jaffrabadi is the heaviest of all Indian buffalo breeds, known as the 'gentle giant' of Saurashtra. It yields rich, high-fat milk while possessing immense draught power."
                    },
                    new Breed
                    {
                        Name = "Kankrej",
                        NormalizedKey = "kankrej",
                        Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Gujarat (Banaskantha, Rann of Kutch) & Rajasthan",
                        PrimaryUtility = "Dual-Purpose (Draught & Dairy)",
                        AverageMilkYield = "1,500 - 2,600 kg per lactation",
                        HornCharacteristics = "Majestic lyre-shaped horns rising vertically from the outer angles of the poll, curving outward and forward.",
                        PhysicalTraits = "Silver-grey to iron-grey coat, darker neck and shoulders in bulls, broad dished forehead, strong pendulous dewlap, renowned for its characteristic rhythmic majestic gait called 'Sawai Chal'.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR Accession INDIA_CATTLE_0400_KANKREJ_03011)",
                        ReferenceImageUrl = "/images/breeds/kankrej.jpg",
                        Description = "Kankrej is a prized dual-purpose zebu breed celebrated for fast, powerful cart and agricultural draught ability, disease resistance, and solid milk yields in arid environments."
                    },
                    new Breed
                    {
                        Name = "Mehsana",
                        NormalizedKey = "mehsana",
                        Species = "Buffalo",
                        ScientificName = "Bubalus bubalis",
                        OriginState = "Gujarat (Mehsana, Sabarkantha, Banaskantha, Ahmedabad)",
                        PrimaryUtility = "Dairy",
                        AverageMilkYield = "1,800 - 2,800 kg per lactation (Fat: 7% - 8%)",
                        HornCharacteristics = "Curved horns resembling an intermediate between Murrah and Surti, semi-sickle shape curving upward.",
                        PhysicalTraits = "Evolved from crossing Murrah and Surti; predominantly jet black or dark brown coat, elongated cylindrical body, broad head, highly docile and consistent persistent lactator.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR Accession INDIA_BUFFALO_0400_MEHSANA_01004)",
                        ReferenceImageUrl = "/images/breeds/mehsana.jpg",
                        Description = "Mehsana is an economically vital dairy buffalo developed through selective breeding in North Gujarat, prized by commercial dairies for regular calving intervals and sustained milk persistence."
                    },
                    new Breed
                    {
                        Name = "Murrah",
                        NormalizedKey = "murrah",
                        Species = "Buffalo",
                        ScientificName = "Bubalus bubalis",
                        OriginState = "Haryana (Rohtak, Hisar, Jind) & Punjab",
                        PrimaryUtility = "Premier Dairy",
                        AverageMilkYield = "2,500 - 4,200 kg per lactation (Fat: 7% - 8.5%)",
                        HornCharacteristics = "Short, tightly coiled spiral horns turning backward and upward, curling tightly into an inward ring ('Jalebi horns').",
                        PhysicalTraits = "Deep jet-black coat ('Black Gold'), short neck, deep broad chest, prominent dairy wedge body, fine tail with white switch, well-developed symmetrical udder with prominent milk veins.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR Accession INDIA_BUFFALO_1700_MURRAH_01005)",
                        ReferenceImageUrl = "/images/breeds/murrah.jpg",
                        Description = "The Murrah is celebrated globally as the 'Black Gold of Haryana' and the highest-yielding dairy buffalo breed in the world, serving as the benchmark for buffalo dairy production worldwide."
                    },
                    new Breed
                    {
                        Name = "Ongole",
                        NormalizedKey = "ongole",
                        Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Andhra Pradesh (Prakasam, Guntur, Nellore)",
                        PrimaryUtility = "Dual-Purpose / Heavy Draught",
                        AverageMilkYield = "1,200 - 2,000 kg per lactation",
                        HornCharacteristics = "Short, stumpy, thick horns growing outward and backward from the outer angles of the poll.",
                        PhysicalTraits = "Large muscular white or light-grey body, massive prominent hump in males, dark muzzle and dark rims around large alert eyes, heavy folded dewlap, high thermal resistance.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR Accession INDIA_CATTLE_0100_ONGOLE_03019)",
                        ReferenceImageUrl = "/images/breeds/ongole.jpg",
                        Description = "Ongole is one of India's world-famous cattle breeds, renowned for immense physical power, heavy draught capability, and pest resistance. It is the primary genetic ancestor of the American Brahman and Brazilian Nelore breeds."
                    },
                    new Breed
                    {
                        Name = "Sahiwal",
                        NormalizedKey = "sahiwal",
                        Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Punjab, Haryana, Rajasthan & Uttar Pradesh",
                        PrimaryUtility = "Premier Dairy",
                        AverageMilkYield = "2,200 - 4,000 kg per lactation",
                        HornCharacteristics = "Short, stumpy, thick horns, sometimes loose or slightly curved outward.",
                        PhysicalTraits = "Reddish-brown to pale red coat, loose pliable skin (hence termed 'Lola' or 'Lambie Bar'), voluminous dewlap, well-developed symmetrical udder, exceptionally docile and calm temperament.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR Accession INDIA_CATTLE_1700_SAHIWAL_03026)",
                        ReferenceImageUrl = "/images/breeds/sahiwal.jpg",
                        Description = "Sahiwal is widely considered the best indigenous dairy cattle breed of the Indian subcontinent, renowned for high A2 milk production, thermal tolerance, and resistance to tropical parasites."
                    },
                    new Breed
                    {
                        Name = "Surti",
                        NormalizedKey = "surti",
                        Species = "Buffalo",
                        ScientificName = "Bubalus bubalis",
                        OriginState = "Gujarat (Kaira, Anand, Baroda, Charotar tract)",
                        PrimaryUtility = "Dairy",
                        AverageMilkYield = "1,600 - 2,200 kg per lactation (Fat: 7% - 8.5%)",
                        HornCharacteristics = "Characteristic sickle-shaped flat horns curving backward, downward, and then turning upward at the tips.",
                        PhysicalTraits = "Medium-sized compact body with straight back, rusty brown to silver-grey coat, two distinctive white chevrons/collars around the lower throat and brisket, white tuft on tail switch.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR Accession INDIA_BUFFALO_0400_SURTI_01008)",
                        ReferenceImageUrl = "/images/breeds/surti.jpg",
                        Description = "The Surti is a compact, economical dairy buffalo breed originating from the Charotar tract of Gujarat, highly valued by smallholder dairy farmers for modest maintenance costs and high fat milk."
                    },
                    new Breed
                    {
                        Name = "Tharparkar",
                        NormalizedKey = "tharparkar",
                        Species = "Cattle",
                        ScientificName = "Bos indicus",
                        OriginState = "Rajasthan (Jodhpur, Jaisalmer, Barmer) & Thar Desert",
                        PrimaryUtility = "Dual-Purpose (Dairy & Arid Draught)",
                        AverageMilkYield = "1,800 - 2,800 kg per lactation",
                        HornCharacteristics = "Medium horns curving outwards and upwards with sharp tips.",
                        PhysicalTraits = "Lustrous white or light grey coat reflecting high desert solar radiation, darker face in winter, medium convex forehead, strong hooves built for walking long desert distances without shoeing.",
                        ConservationStatus = "Indigenous (ICAR-NBAGR Accession INDIA_CATTLE_1700_THARPARKAR_03028)",
                        ReferenceImageUrl = "/images/breeds/tharparkar.jpg",
                        Description = "Tharparkar ('White Sindhi') is an extraordinarily resilient desert dual-purpose breed capable of maintaining robust milk yields and performing heavy agricultural work under scarce forage and extreme heat."
                    }
                };

                await context.Breeds.AddRangeAsync(breeds);
                await context.SaveChangesAsync();
                logger.LogInformation("Successfully seeded {Count} indigenous breeds into database.", breeds.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
}
