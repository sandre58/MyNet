// -----------------------------------------------------------------------
// <copyright file="Country.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Geography;

/// <summary>
/// Represents a country using standardized ISO 3166 codes and associated continent information.
/// </summary>
/// <remarks>The Country class provides a strongly-typed representation of countries, including their ISO 3166
/// numeric, alpha-2, and alpha-3 codes, as well as the continent to which each country belongs. This enables consistent
/// handling of country data across applications, particularly when interoperating with external systems or data formats
/// that require standardized country identifiers.</remarks>
public sealed class Country : EnumClass<Country>
{
    public static readonly Country Afghanistan = new(4, nameof(Afghanistan), "af", "afg", Continent.Asia);
    public static readonly Country Albania = new(8, nameof(Albania), "al", "alb", Continent.Europe);
    public static readonly Country Antarctica = new(10, nameof(Antarctica), "aq", "ata", Continent.Antarctica);
    public static readonly Country Algeria = new(12, nameof(Algeria), "dz", "dza", Continent.Africa);
    public static readonly Country AmericanSamoa = new(16, nameof(AmericanSamoa), "as", "asm", Continent.Oceania);
    public static readonly Country Andorra = new(20, nameof(Andorra), "ad", "and", Continent.Europe);
    public static readonly Country Angola = new(24, nameof(Angola), "ao", "ago", Continent.Africa);
    public static readonly Country AntiguaAndBarbuda = new(28, nameof(AntiguaAndBarbuda), "ag", "atg", Continent.NorthAmerica);
    public static readonly Country Azerbaijan = new(31, nameof(Azerbaijan), "az", "aze", Continent.Asia);
    public static readonly Country Argentina = new(32, nameof(Argentina), "ar", "arg", Continent.SouthAmerica);
    public static readonly Country Australia = new(36, nameof(Australia), "au", "aus", Continent.Oceania);
    public static readonly Country Austria = new(40, nameof(Austria), "at", "aut", Continent.Europe);
    public static readonly Country Bahamas = new(44, nameof(Bahamas), "bs", "bhs", Continent.NorthAmerica);
    public static readonly Country Bahrain = new(48, nameof(Bahrain), "bh", "bhr", Continent.Asia);
    public static readonly Country Bangladesh = new(50, nameof(Bangladesh), "bd", "bgd", Continent.Asia);
    public static readonly Country Armenia = new(51, nameof(Armenia), "am", "arm", Continent.Europe);
    public static readonly Country Barbados = new(52, nameof(Barbados), "bb", "brb", Continent.NorthAmerica);
    public static readonly Country Belgium = new(56, nameof(Belgium), "be", "bel", Continent.Europe);
    public static readonly Country Bermuda = new(60, nameof(Bermuda), "bm", "bmu", Continent.NorthAmerica);
    public static readonly Country Bhutan = new(64, nameof(Bhutan), "bt", "btn", Continent.Asia);
    public static readonly Country Bolivia = new(68, nameof(Bolivia), "bo", "bol", Continent.SouthAmerica);
    public static readonly Country BosniaAndHerzegovina = new(70, nameof(BosniaAndHerzegovina), "ba", "bih", Continent.Europe);
    public static readonly Country Botswana = new(72, nameof(Botswana), "bw", "bwa", Continent.Africa);
    public static readonly Country BouvetIsland = new(74, nameof(BouvetIsland), "bv", "bvt", Continent.Antarctica);
    public static readonly Country Brazil = new(76, nameof(Brazil), "br", "bra", Continent.SouthAmerica);
    public static readonly Country Belize = new(84, nameof(Belize), "bz", "blz", Continent.NorthAmerica);
    public static readonly Country BritishIndianOceanTerritory = new(86, nameof(BritishIndianOceanTerritory), "io", "iot", Continent.Asia);
    public static readonly Country SolomonIslands = new(90, nameof(SolomonIslands), "sb", "slb", Continent.Oceania);
    public static readonly Country VirginIslandsUK = new(92, nameof(VirginIslandsUK), "vg", "vgb", Continent.NorthAmerica);
    public static readonly Country BruneiDarussalam = new(96, nameof(BruneiDarussalam), "bn", "brn", Continent.Asia);
    public static readonly Country Bulgaria = new(100, nameof(Bulgaria), "bg", "bgr", Continent.Europe);
    public static readonly Country Myanmar = new(104, nameof(Myanmar), "mm", "mmr", Continent.Asia);
    public static readonly Country Burundi = new(108, nameof(Burundi), "bi", "bdi", Continent.Africa);
    public static readonly Country Belarus = new(112, nameof(Belarus), "by", "blr", Continent.Europe);
    public static readonly Country Cambodia = new(116, nameof(Cambodia), "kh", "khm", Continent.Asia);
    public static readonly Country Cameroon = new(120, nameof(Cameroon), "cm", "cmr", Continent.Africa);
    public static readonly Country Canada = new(124, nameof(Canada), "ca", "can", Continent.NorthAmerica);
    public static readonly Country CaboVerde = new(132, nameof(CaboVerde), "cv", "cpv", Continent.Africa);
    public static readonly Country CaymanIslands = new(136, nameof(CaymanIslands), "ky", "cym", Continent.NorthAmerica);
    public static readonly Country CentralAfricanRepublic = new(140, nameof(CentralAfricanRepublic), "cf", "caf", Continent.Africa);
    public static readonly Country SriLanka = new(144, nameof(SriLanka), "lk", "lka", Continent.Asia);
    public static readonly Country Chad = new(148, nameof(Chad), "td", "tcd", Continent.Africa);
    public static readonly Country Chile = new(152, nameof(Chile), "cl", "chl", Continent.SouthAmerica);
    public static readonly Country China = new(156, nameof(China), "cn", "chn", Continent.Asia);
    public static readonly Country Taiwan = new(158, nameof(Taiwan), "tw", "twn", Continent.Asia);
    public static readonly Country ChristmasIsland = new(162, nameof(ChristmasIsland), "cx", "cxr", Continent.Asia);
    public static readonly Country CocosIslands = new(166, nameof(CocosIslands), "cc", "cck", Continent.Asia);
    public static readonly Country Colombia = new(170, nameof(Colombia), "co", "col", Continent.SouthAmerica);
    public static readonly Country Comoros = new(174, nameof(Comoros), "km", "com", Continent.Africa);
    public static readonly Country Mayotte = new(175, nameof(Mayotte), "yt", "myt", Continent.Africa);
    public static readonly Country Congo = new(178, nameof(Congo), "cg", "cog", Continent.Africa);
    public static readonly Country CongoDemocraticRepublic = new(180, nameof(CongoDemocraticRepublic), "cd", "cod", Continent.Africa);
    public static readonly Country CookIslands = new(184, nameof(CookIslands), "ck", "cok", Continent.Oceania);
    public static readonly Country CostaRica = new(188, nameof(CostaRica), "cr", "cri", Continent.NorthAmerica);
    public static readonly Country Croatia = new(191, nameof(Croatia), "hr", "hrv", Continent.Europe);
    public static readonly Country Cuba = new(192, nameof(Cuba), "cu", "cub", Continent.NorthAmerica);
    public static readonly Country Cyprus = new(196, nameof(Cyprus), "cy", "cyp", Continent.Europe);
    public static readonly Country Czechia = new(203, nameof(Czechia), "cz", "cze", Continent.Europe);
    public static readonly Country Benin = new(204, nameof(Benin), "bj", "ben", Continent.Africa);
    public static readonly Country Denmark = new(208, nameof(Denmark), "dk", "dnk", Continent.Europe);
    public static readonly Country Dominica = new(212, nameof(Dominica), "dm", "dma", Continent.Africa);
    public static readonly Country DominicanRepublic = new(214, nameof(DominicanRepublic), "do", "dom", Continent.Africa);
    public static readonly Country Ecuador = new(218, nameof(Ecuador), "ec", "ecu", Continent.SouthAmerica);
    public static readonly Country ElSalvador = new(222, nameof(ElSalvador), "sv", "slv", Continent.NorthAmerica);
    public static readonly Country EquatorialGuinea = new(226, nameof(EquatorialGuinea), "gq", "gnq", Continent.Africa);
    public static readonly Country Ethiopia = new(231, nameof(Ethiopia), "et", "eth", Continent.Africa);
    public static readonly Country Eritrea = new(232, nameof(Eritrea), "er", "eri", Continent.Africa);
    public static readonly Country Estonia = new(233, nameof(Estonia), "ee", "est", Continent.Europe);
    public static readonly Country FaroeIslands = new(234, nameof(FaroeIslands), "fo", "fro", Continent.Europe);
    public static readonly Country FalklandIslands = new(238, nameof(FalklandIslands), "fk", "flk", Continent.SouthAmerica);
    public static readonly Country SouthGeorgiaAndTheSouthSandwichIslands = new(239, nameof(SouthGeorgiaAndTheSouthSandwichIslands), "gs", "sgs", Continent.SouthAmerica);
    public static readonly Country Fiji = new(242, nameof(Fiji), "fj", "fji", Continent.Asia);
    public static readonly Country Finland = new(246, nameof(Finland), "fi", "fin", Continent.Europe);
    public static readonly Country AlandIslands = new(248, nameof(AlandIslands), "ax", "ala", Continent.Europe);
    public static readonly Country France = new(250, nameof(France), "fr", "fra", Continent.Europe);
    public static readonly Country FrenchGuiana = new(254, nameof(FrenchGuiana), "gf", "guf", Continent.SouthAmerica);
    public static readonly Country FrenchPolynesia = new(258, nameof(FrenchPolynesia), "pf", "pyf", Continent.Oceania);
    public static readonly Country FrenchSouthernTerritories = new(260, nameof(FrenchSouthernTerritories), "tf", "atf", Continent.Antarctica);
    public static readonly Country Djibouti = new(262, nameof(Djibouti), "dj", "dji", Continent.Africa);
    public static readonly Country Gabon = new(266, nameof(Gabon), "ga", "gab", Continent.Africa);
    public static readonly Country Georgia = new(268, nameof(Georgia), "ge", "geo", Continent.Europe);
    public static readonly Country Gambia = new(270, nameof(Gambia), "gm", "gmb", Continent.Africa);
    public static readonly Country Palestine = new(275, nameof(Palestine), "ps", "pse", Continent.Asia);
    public static readonly Country Germany = new(276, nameof(Germany), "de", "deu", Continent.Europe);
    public static readonly Country Ghana = new(288, nameof(Ghana), "gh", "gha", Continent.Africa);
    public static readonly Country Gibraltar = new(292, nameof(Gibraltar), "gi", "gib", Continent.Europe);
    public static readonly Country Kiribati = new(296, nameof(Kiribati), "ki", "kir", Continent.Oceania);
    public static readonly Country Greece = new(300, nameof(Greece), "gr", "grc", Continent.Europe);
    public static readonly Country Greenland = new(304, nameof(Greenland), "gl", "grl", Continent.NorthAmerica);
    public static readonly Country Grenada = new(308, nameof(Grenada), "gd", "grd", Continent.NorthAmerica);
    public static readonly Country Guadeloupe = new(312, nameof(Guadeloupe), "gp", "glp", Continent.NorthAmerica);
    public static readonly Country Guam = new(316, nameof(Guam), "gu", "gum", Continent.Oceania);
    public static readonly Country Guatemala = new(320, nameof(Guatemala), "gt", "gtm", Continent.NorthAmerica);
    public static readonly Country Guinea = new(324, nameof(Guinea), "gn", "gin", Continent.Africa);
    public static readonly Country Guyana = new(328, nameof(Guyana), "gy", "guy", Continent.SouthAmerica);
    public static readonly Country Haiti = new(332, nameof(Haiti), "ht", "hti", Continent.NorthAmerica);
    public static readonly Country HeardIslandAndMcdonaldIslands = new(334, nameof(HeardIslandAndMcdonaldIslands), "hm", "hmd", Continent.Oceania);
    public static readonly Country HolySee = new(336, nameof(HolySee), "va", "vat", Continent.Europe);
    public static readonly Country Honduras = new(340, nameof(Honduras), "hn", "hnd", Continent.NorthAmerica);
    public static readonly Country HongKong = new(344, nameof(HongKong), "hk", "hkg", Continent.Asia);
    public static readonly Country Hungary = new(348, nameof(Hungary), "hu", "hun", Continent.Europe);
    public static readonly Country Iceland = new(352, nameof(Iceland), "is", "isl", Continent.Europe);
    public static readonly Country India = new(356, nameof(India), "in", "ind", Continent.Asia);
    public static readonly Country Indonesia = new(360, nameof(Indonesia), "id", "idn", Continent.Asia);
    public static readonly Country Iran = new(364, nameof(Iran), "ir", "irn", Continent.Asia);
    public static readonly Country Iraq = new(368, nameof(Iraq), "iq", "irq", Continent.Asia);
    public static readonly Country Ireland = new(372, nameof(Ireland), "ie", "irl", Continent.Europe);
    public static readonly Country Israel = new(376, nameof(Israel), "il", "isr", Continent.Europe);
    public static readonly Country Italy = new(380, nameof(Italy), "it", "ita", Continent.Europe);
    public static readonly Country CoteIvoire = new(384, nameof(CoteIvoire), "ci", "civ", Continent.Africa);
    public static readonly Country Jamaica = new(388, nameof(Jamaica), "jm", "jam", Continent.NorthAmerica);
    public static readonly Country Japan = new(392, nameof(Japan), "jp", "jpn", Continent.Asia);
    public static readonly Country Kazakhstan = new(398, nameof(Kazakhstan), "kz", "kaz", Continent.Asia);
    public static readonly Country Jordan = new(400, nameof(Jordan), "jo", "jor", Continent.Asia);
    public static readonly Country Kenya = new(404, nameof(Kenya), "ke", "ken", Continent.Africa);
    public static readonly Country NorthKorea = new(408, nameof(NorthKorea), "kp", "prk", Continent.Asia);
    public static readonly Country SouthKorea = new(410, nameof(SouthKorea), "kr", "kor", Continent.Asia);
    public static readonly Country Kuwait = new(414, nameof(Kuwait), "kw", "kwt", Continent.Asia);
    public static readonly Country Kyrgyzstan = new(417, nameof(Kyrgyzstan), "kg", "kgz", Continent.Asia);
    public static readonly Country Lao = new(418, nameof(Lao), "la", "lao", Continent.Asia);
    public static readonly Country Lebanon = new(422, nameof(Lebanon), "lb", "lbn", Continent.Asia);
    public static readonly Country Lesotho = new(426, nameof(Lesotho), "ls", "lso", Continent.Africa);
    public static readonly Country Latvia = new(428, nameof(Latvia), "lv", "lva", Continent.Europe);
    public static readonly Country Liberia = new(430, nameof(Liberia), "lr", "lbr", Continent.Africa);
    public static readonly Country Libya = new(434, nameof(Libya), "ly", "lby", Continent.Africa);
    public static readonly Country Liechtenstein = new(438, nameof(Liechtenstein), "li", "lie", Continent.Europe);
    public static readonly Country Lithuania = new(440, nameof(Lithuania), "lt", "ltu", Continent.Europe);
    public static readonly Country Luxembourg = new(442, nameof(Luxembourg), "lu", "lux", Continent.Europe);
    public static readonly Country Macao = new(446, nameof(Macao), "mo", "mac", Continent.Asia);
    public static readonly Country Madagascar = new(450, nameof(Madagascar), "mg", "mdg", Continent.Africa);
    public static readonly Country Malawi = new(454, nameof(Malawi), "mw", "mwi", Continent.Africa);
    public static readonly Country Malaysia = new(458, nameof(Malaysia), "my", "mys", Continent.Asia);
    public static readonly Country Maldives = new(462, nameof(Maldives), "mv", "mdv", Continent.Asia);
    public static readonly Country Mali = new(466, nameof(Mali), "ml", "mli", Continent.Africa);
    public static readonly Country Malta = new(470, nameof(Malta), "mt", "mlt", Continent.Europe);
    public static readonly Country Martinique = new(474, nameof(Martinique), "mq", "mtq", Continent.NorthAmerica);
    public static readonly Country Mauritania = new(478, nameof(Mauritania), "mr", "mrt", Continent.Africa);
    public static readonly Country Mauritius = new(480, nameof(Mauritius), "mu", "mus", Continent.Africa);
    public static readonly Country Mexico = new(484, nameof(Mexico), "mx", "mex", Continent.NorthAmerica);
    public static readonly Country Monaco = new(492, nameof(Monaco), "mc", "mco", Continent.Europe);
    public static readonly Country Mongolia = new(496, nameof(Mongolia), "mn", "mng", Continent.Asia);
    public static readonly Country Moldova = new(498, nameof(Moldova), "md", "mda", Continent.Europe);
    public static readonly Country Montenegro = new(499, nameof(Montenegro), "me", "mne", Continent.Europe);
    public static readonly Country Montserrat = new(500, nameof(Montserrat), "ms", "msr", Continent.NorthAmerica);
    public static readonly Country Morocco = new(504, nameof(Morocco), "ma", "mar", Continent.Africa);
    public static readonly Country Mozambique = new(508, nameof(Mozambique), "mz", "moz", Continent.Africa);
    public static readonly Country Oman = new(512, nameof(Oman), "om", "omn", Continent.Asia);
    public static readonly Country Namibia = new(516, nameof(Namibia), "na", "nam", Continent.Africa);
    public static readonly Country Nauru = new(520, nameof(Nauru), "nr", "nru", Continent.Oceania);
    public static readonly Country Nepal = new(524, nameof(Nepal), "np", "npl", Continent.Asia);
    public static readonly Country Netherlands = new(528, nameof(Netherlands), "nl", "nld", Continent.Europe);
    public static readonly Country Curacao = new(531, nameof(Curacao), "cw", "cuw", Continent.SouthAmerica);
    public static readonly Country Aruba = new(533, nameof(Aruba), "aw", "abw", Continent.SouthAmerica);
    public static readonly Country SintMaarten = new(534, nameof(SintMaarten), "sx", "sxm", Continent.NorthAmerica);
    public static readonly Country Bonaire = new(535, nameof(Bonaire), "bq", "bes", Continent.SouthAmerica);
    public static readonly Country NewCaledonia = new(540, nameof(NewCaledonia), "nc", "ncl", Continent.Oceania);
    public static readonly Country Vanuatu = new(548, nameof(Vanuatu), "vu", "vut", Continent.Asia);
    public static readonly Country NewZealand = new(554, nameof(NewZealand), "nz", "nzl", Continent.Asia);
    public static readonly Country Nicaragua = new(558, nameof(Nicaragua), "ni", "nic", Continent.NorthAmerica);
    public static readonly Country Niger = new(562, nameof(Niger), "ne", "ner", Continent.Africa);
    public static readonly Country Nigeria = new(566, nameof(Nigeria), "ng", "nga", Continent.Africa);
    public static readonly Country Niue = new(570, nameof(Niue), "nu", "niu", Continent.Oceania);
    public static readonly Country NorfolkIsland = new(574, nameof(NorfolkIsland), "nf", "nfk", Continent.Oceania);
    public static readonly Country Norway = new(578, nameof(Norway), "no", "nor", Continent.Europe);
    public static readonly Country NorthernMarianaIslands = new(580, nameof(NorthernMarianaIslands), "mp", "mnp", Continent.Oceania);
    public static readonly Country UnitedStatesMinorOutlyingIslands = new(581, nameof(UnitedStatesMinorOutlyingIslands), "um", "umi", Continent.NorthAmerica);
    public static readonly Country Micronesia = new(583, nameof(Micronesia), "fm", "fsm", Continent.Oceania);
    public static readonly Country MarshallIslands = new(584, nameof(MarshallIslands), "mh", "mhl", Continent.Oceania);
    public static readonly Country Palau = new(585, nameof(Palau), "pw", "plw", Continent.Oceania);
    public static readonly Country Pakistan = new(586, nameof(Pakistan), "pk", "pak", Continent.Asia);
    public static readonly Country Panama = new(591, nameof(Panama), "pa", "pan", Continent.NorthAmerica);
    public static readonly Country PapuaNewGuinea = new(598, nameof(PapuaNewGuinea), "pg", "png", Continent.Oceania);
    public static readonly Country Paraguay = new(600, nameof(Paraguay), "py", "pry", Continent.SouthAmerica);
    public static readonly Country Peru = new(604, nameof(Peru), "pe", "per", Continent.SouthAmerica);
    public static readonly Country Philippines = new(608, nameof(Philippines), "ph", "phl", Continent.Asia);
    public static readonly Country Pitcairn = new(612, nameof(Pitcairn), "pn", "pcn", Continent.Oceania);
    public static readonly Country Poland = new(616, nameof(Poland), "pl", "pol", Continent.Europe);
    public static readonly Country Portugal = new(620, nameof(Portugal), "pt", "prt", Continent.Europe);
    public static readonly Country GuineaBissau = new(624, nameof(GuineaBissau), "gw", "gnb", Continent.Africa);
    public static readonly Country TimorLeste = new(626, nameof(TimorLeste), "tl", "tls", Continent.Asia);
    public static readonly Country PuertoRico = new(630, nameof(PuertoRico), "pr", "pri", Continent.NorthAmerica);
    public static readonly Country Qatar = new(634, nameof(Qatar), "qa", "qat", Continent.Asia);
    public static readonly Country Reunion = new(638, nameof(Reunion), "re", "reu", Continent.Africa);
    public static readonly Country Romania = new(642, nameof(Romania), "ro", "rou", Continent.Europe);
    public static readonly Country RussianFederation = new(643, nameof(RussianFederation), "ru", "rus", Continent.Europe);
    public static readonly Country Rwanda = new(646, nameof(Rwanda), "rw", "rwa", Continent.Africa);
    public static readonly Country SaintBartheLemy = new(652, nameof(SaintBartheLemy), "bl", "blm", Continent.NorthAmerica);
    public static readonly Country SaintHelena = new(654, nameof(SaintHelena), "sh", "shn", Continent.Africa);
    public static readonly Country SaintKittsAndNevis = new(659, nameof(SaintKittsAndNevis), "kn", "kna", Continent.NorthAmerica);
    public static readonly Country Anguilla = new(66, nameof(Anguilla), "ai", "aia", Continent.NorthAmerica);
    public static readonly Country SaintLucia = new(662, nameof(SaintLucia), "lc", "lca", Continent.NorthAmerica);
    public static readonly Country SaintMartin = new(663, nameof(SaintMartin), "mf", "maf", Continent.NorthAmerica);
    public static readonly Country SaintPierreAndMiquelon = new(666, nameof(SaintPierreAndMiquelon), "pm", "spm", Continent.NorthAmerica);
    public static readonly Country SaintVincentAndTheGrenadines = new(670, nameof(SaintVincentAndTheGrenadines), "vc", "vct", Continent.NorthAmerica);
    public static readonly Country SanMarino = new(674, nameof(SanMarino), "sm", "smr", Continent.Europe);
    public static readonly Country SaoTomeAndPrincipe = new(678, nameof(SaoTomeAndPrincipe), "st", "stp", Continent.Africa);
    public static readonly Country SaudiArabia = new(682, nameof(SaudiArabia), "sa", "sau", Continent.Asia);
    public static readonly Country Senegal = new(686, nameof(Senegal), "sn", "sen", Continent.Africa);
    public static readonly Country Serbia = new(688, nameof(Serbia), "rs", "srb", Continent.Europe);
    public static readonly Country Seychelles = new(690, nameof(Seychelles), "sc", "syc", Continent.Africa);
    public static readonly Country SierraLeone = new(694, nameof(SierraLeone), "sl", "sle", Continent.Africa);
    public static readonly Country Singapore = new(702, nameof(Singapore), "sg", "sgp", Continent.Asia);
    public static readonly Country Slovakia = new(703, nameof(Slovakia), "sk", "svk", Continent.Europe);
    public static readonly Country VietNam = new(704, nameof(VietNam), "vn", "vnm", Continent.Asia);
    public static readonly Country Slovenia = new(705, nameof(Slovenia), "si", "svn", Continent.Europe);
    public static readonly Country Somalia = new(706, nameof(Somalia), "so", "som", Continent.Africa);
    public static readonly Country SouthAfrica = new(710, nameof(SouthAfrica), "za", "zaf", Continent.Africa);
    public static readonly Country Zimbabwe = new(716, nameof(Zimbabwe), "zw", "zwe", Continent.Africa);
    public static readonly Country Spain = new(724, nameof(Spain), "es", "esp", Continent.Europe);
    public static readonly Country SouthSudan = new(728, nameof(SouthSudan), "ss", "ssd", Continent.Africa);
    public static readonly Country Sudan = new(729, nameof(Sudan), "sd", "sdn", Continent.Africa);
    public static readonly Country WesternSahara = new(732, nameof(WesternSahara), "eh", "esh", Continent.Africa);
    public static readonly Country Suriname = new(740, nameof(Suriname), "sr", "sur", Continent.Asia);
    public static readonly Country SvalbardAndJanMayen = new(744, nameof(SvalbardAndJanMayen), "sj", "sjm", Continent.Europe);
    public static readonly Country Eswatini = new(748, nameof(Eswatini), "sz", "swz", Continent.Africa);
    public static readonly Country Sweden = new(752, nameof(Sweden), "se", "swe", Continent.Europe);
    public static readonly Country Switzerland = new(756, nameof(Switzerland), "ch", "che", Continent.Europe);
    public static readonly Country SyrianArabRepublic = new(760, nameof(SyrianArabRepublic), "sy", "syr", Continent.Asia);
    public static readonly Country Tajikistan = new(762, nameof(Tajikistan), "tj", "tjk", Continent.Asia);
    public static readonly Country Thailand = new(764, nameof(Thailand), "th", "tha", Continent.Asia);
    public static readonly Country Togo = new(768, nameof(Togo), "tg", "tgo", Continent.Africa);
    public static readonly Country Tokelau = new(772, nameof(Tokelau), "tk", "tkl", Continent.Oceania);
    public static readonly Country Tonga = new(776, nameof(Tonga), "to", "ton", Continent.Oceania);
    public static readonly Country TrinidadAndTobago = new(780, nameof(TrinidadAndTobago), "tt", "tto", Continent.SouthAmerica);
    public static readonly Country UnitedArabEmirates = new(784, nameof(UnitedArabEmirates), "ae", "are", Continent.Asia);
    public static readonly Country Tunisia = new(788, nameof(Tunisia), "tn", "tun", Continent.Africa);
    public static readonly Country Turkey = new(792, nameof(Turkey), "tr", "tur", Continent.Europe);
    public static readonly Country Turkmenistan = new(795, nameof(Turkmenistan), "tm", "tkm", Continent.Asia);
    public static readonly Country TurksAndCaicosIslands = new(796, nameof(TurksAndCaicosIslands), "tc", "tca", Continent.NorthAmerica);
    public static readonly Country Tuvalu = new(798, nameof(Tuvalu), "tv", "tuv", Continent.Asia);
    public static readonly Country Uganda = new(800, nameof(Uganda), "ug", "uga", Continent.Africa);
    public static readonly Country Ukraine = new(804, nameof(Ukraine), "ua", "ukr", Continent.Europe);
    public static readonly Country NorthMacedonia = new(807, nameof(NorthMacedonia), "mk", "mkd", Continent.Europe);
    public static readonly Country Egypt = new(818, nameof(Egypt), "eg", "egy", Continent.Africa);
    public static readonly Country UnitedKingdomOfGreatBritainAndNorthernIreland = new(826, nameof(UnitedKingdomOfGreatBritainAndNorthernIreland), "gb", "gbr", Continent.Europe);
    public static readonly Country Guernsey = new(831, nameof(Guernsey), "gg", "ggy", Continent.Europe);
    public static readonly Country Jersey = new(832, nameof(Jersey), "je", "jey", Continent.Europe);
    public static readonly Country IsleOfMan = new(833, nameof(IsleOfMan), "im", "imn", Continent.Europe);
    public static readonly Country Tanzania = new(834, nameof(Tanzania), "tz", "tza", Continent.Africa);
    public static readonly Country UnitedStatesOfAmerica = new(840, nameof(UnitedStatesOfAmerica), "us", "usa", Continent.NorthAmerica);
    public static readonly Country VirginIslandsUS = new(850, nameof(VirginIslandsUS), "vi", "vir", Continent.NorthAmerica);
    public static readonly Country BurkinaFaso = new(854, nameof(BurkinaFaso), "bf", "bfa", Continent.Africa);
    public static readonly Country Uruguay = new(855, nameof(Uruguay), "uy", "ury", Continent.SouthAmerica);
    public static readonly Country Uzbekistan = new(860, nameof(Uzbekistan), "uz", "uzb", Continent.Asia);
    public static readonly Country Venezuela = new(862, nameof(Venezuela), "ve", "ven", Continent.SouthAmerica);
    public static readonly Country WallisAndFutuna = new(876, nameof(WallisAndFutuna), "wf", "wlf", Continent.Oceania);
    public static readonly Country Samoa = new(882, nameof(Samoa), "ws", "wsm", Continent.Asia);
    public static readonly Country Yemen = new(887, nameof(Yemen), "ye", "yem", Continent.Asia);
    public static readonly Country Zambia = new(894, nameof(Zambia), "zm", "zmb", Continent.Africa);

    /// <summary>
    /// Initializes a new instance of the <see cref="Country"/> class with the specified ISO code, name, alpha-2 code, alpha-3 code, and continent.
    /// </summary>
    /// <param name="iso">The ISO code of the country.</param>
    /// <param name="name">The name of the country.</param>
    /// <param name="alpha2">The alpha-2 code of the country.</param>
    /// <param name="alpha3">The alpha-3 code of the country.</param>
    /// <param name="continent">The continent of the country.</param>
    private Country(int iso, string name, string alpha2, string alpha3, Continent continent)
        : base(name, iso)
        => (Iso, Alpha2, Alpha3, Continent) = (iso, alpha2, alpha3, continent);

    /// <summary>
    /// Gets the ISO 3166 numeric code that uniquely identifies the country.
    /// </summary>
    /// <remarks>The ISO code is a standardized numeric identifier as defined by the ISO 3166-1 specification.
    /// This value is commonly used for interoperability with external systems and data formats that require country
    /// codes.</remarks>
    public int Iso { get; }

    /// <summary>
    /// Gets the two-letter country code as defined by ISO 3166-1 alpha-2.
    /// </summary>
    public string Alpha2 { get; }

    /// <summary>
    /// Gets the three-letter country code as defined by ISO 3166-1.
    /// </summary>
    public string Alpha3 { get; }

    /// <summary>
    /// Gets the continent associated with the current instance.
    /// </summary>
    public Continent Continent { get; }
}
