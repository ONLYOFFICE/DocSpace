import {
  Afghanistan,
  AlandIslands,
  Albania,
  Algeria,
  AmericanSamoa,
  Andorra,
  Angola,
  Anguilla,
  Antarctica,
  AntiguaAndBarbuda,
  Argentina,
  Armenia,
  Aruba,
  Australia,
  Austria,
  Azerbaijan,
  Bahamas,
  Bahrain,
  Bangladesh,
  Barbados,
  Belarus,
  Belgium,
  Belize,
  Benin,
  Bermuda,
  Bhutan,
  Bolivia,
  BonaireSintEustatiusAndSaba,
  BosniaAndHerzegovina,
  Botswana,
  BouvetIsland,
  Brazil,
  BritishIndianOceanTerritory,
  BritishVirginIslands,
  BruneiDarussalam,
  Bulgaria,
  BurkinaFaso,
  Burundi,
  CaboVerde,
  Cambodia,
  Cameroon,
  Canada,
  CaymanIslands,
  CentralAfricanRepublic,
  Chad,
  Chile,
  China,
  ChristmasIsland,
  CocosIslands,
  Colombia,
  Comoros,
  CookIslands,
  CostaRica,
  CoteDivoire,
  Croatia,
  Cuba,
  Curacao,
  Cyprus,
  CzechRepublic,
  DemocraticRepublicOfCongo,
  Denmark,
  Djibouti,
  Dominica,
  DominicanRepublic,
  Ecuador,
  Egypt,
  ElSalvador,
  EquatorialGuinea,
  Eritrea,
  Estonia,
  Ethiopia,
  FalklandIslands,
  FaroeIslands,
  FederatedStatesOfMicronesia,
  Fiji,
  Finland,
  France,
  FrenchGuiana,
  Gabon,
  Gambia,
  Georgia,
  Germany,
  Ghana,
  Gibraltar,
  Greece,
  Greenland,
  Grenada,
  Guadeloupe,
  Guam,
  Guatemala,
  Guernsey,
  GuineaBissau,
  Guinea,
  Guyana,
  Haiti,
  HeardIslandAndMcDonaldIslands,
  Honduras,
  HongKong,
  Hungary,
  Iceland,
  India,
  Indonesia,
  Iran,
  Iraq,
  Ireland,
  IsleOfMan,
  Israel,
  Italy,
  Jamaica,
  Japan,
  Jersey,
  Jordan,
  Kazakhstan,
  Kenya,
  Kiribati,
  Kosovo,
  Kuwait,
  Kyrgyzstan,
  Laos,
  Latvia,
  Lebanon,
  Lesotho,
  Liberia,
  Liechtenstein,
  Lithuania,
  Luxembourg,
  Lybia,
  Macao,
  Macedonia,
  Madagascar,
  Malawi,
  Malaysia,
  Maldives,
  Mali,
  Malta,
  MarshallIslands,
  Martinique,
  Mauritania,
  Mauritius,
  Mayotte,
  Mexico,
  Moldova,
  Monaco,
  Mongolia,
  Montenegro,
  Montserrat,
  Morocco,
  Mozambique,
  Myanmar,
  Namibia,
  Nauru,
  Nepal,
  NetherlandsAntilles,
  Netherlands,
  NewCaledonia,
  NewGuinea,
  NewZealand,
  Nicaragua,
  Niger,
  Nigeria,
  Niue,
  NorfolkIslands,
  NorthKorea,
  NorthenMarianaIslands,
  Norway,
  Oman,
  Pakistan,
  Palau,
  Palestine,
  Panama,
  Paraguay,
  Peru,
  Phillipines,
  PitcairnIslands,
  Poland,
  Polynesia,
  Portugal,
  PuertoRico,
  Qatar,
  RepublicOfTheCongo,
  Reunion,
  Romania,
  Russia,
  Rwanda,
  SainVincentAndGrenadines,
  SaintBarthelemy,
  SaintKittsAndNevis,
  SaintLucia,
  SaintMartin,
  SaintPierreAndMiquelon,
  Samoa,
  SanMarino,
  SaoTomeAndPrincipe,
  SaudiArabia,
  Senegal,
  Serbia,
  Seychelles,
  SierraLeone,
  Singapore,
  SintMaarten,
  Slovakia,
  Slovenia,
  SolomonIslands,
  Somalia,
  SouthAfrica,
  SouthGeorgiaAndSandwichIslands,
  SouthKorea,
  SouthSudan,
  Spain,
  SriLanka,
  Sudan,
  Suriname,
  SvalbardAndJanMayen,
  Swaziland,
  Sweden,
  Switzerland,
  Syria,
  Taiwan,
  Tajikistan,
  Tanzania,
  Thailand,
  TimorLeste,
  Togo,
  Tokelau,
  Tonga,
  TrinidadAndTobago,
  Tunisia,
  Turkey,
  Turkmenistan,
  TurksAndCaicosIslands,
  Tuvalu,
  Uganda,
  Ukraine,
  UnitedArabEmirates,
  UnitedKingdom,
  UnitedStatesVirginIslands,
  UnitedStates,
  Uruguay,
  Uzbekistan,
  Vatican,
  Venezuela,
  Vietnam,
  Vunuatu,
  WallisAndFutuna,
  Yemen,
  Zambia,
  Zimbabwe,
} from "./svg";

export const options = [
  {
    name: "Afghanistan",
    dialCode: "93",
    code: "AF",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Afghanistan,
  },
  {
    name: "Aland Islands",
    dialCode: "358",
    code: "AX",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: AlandIslands,
  },
  {
    name: "Albania",
    dialCode: "355",
    code: "AL",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Albania,
  },
  {
    name: "Algeria",
    dialCode: "213",
    code: "DZ",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Algeria,
  },
  {
    name: "American Samoa",
    dialCode: "1684",
    code: "AS",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: AmericanSamoa,
  },
  {
    name: "Andorra",
    dialCode: "376",
    code: "AD",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Andorra,
  },
  {
    name: "Angola",
    dialCode: "244",
    code: "AO",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Angola,
  },
  {
    name: "Anguilla",
    dialCode: "1264",
    code: "AI",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Anguilla,
  },
  {
    name: "Antarctica",
    dialCode: "6721",
    code: "AQ",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Antarctica,
  },
  {
    name: "Antigua and Barbuda",
    dialCode: "1268",
    code: "AG",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: AntiguaAndBarbuda,
  },
  {
    name: "Argentina",
    dialCode: "54",
    code: "AR",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Argentina,
  },
  {
    name: "Armenia",
    dialCode: "374",
    code: "AM",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Armenia,
  },
  {
    name: "Aruba",
    dialCode: "297",
    code: "AW",
    mask: [ /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Aruba,
  },
  {
    name: "Australia",
    dialCode: "61",
    code: "AU",
    mask: [/\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Australia,
  },
  {
    name: "Austria",
    dialCode: "43",
    code: "AT",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Austria,
  },
  {
    name: "Azerbaijan",
    dialCode: "994",
    code: "AZ",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Azerbaijan,
  },
  {
    name: "Bahamas",
    dialCode: "1242",
    code: "BS",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/,],
    flag: Bahamas,
  },
  {
    name: "Bahrain",
    dialCode: "973",
    code: "BH",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Bahrain,
  },
  {
    name: "Bangladesh",
    dialCode: "880",
    code: "BD",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Bangladesh,
  },
  {
    name: "Barbados",
    dialCode: "1246",
    code: "BB",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Barbados,
  },
  {
    name: "Belarus",
    dialCode: "375",
    code: "BY",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Belarus,
  },
  {
    name: "Belgium",
    dialCode: "32",
    code: "BE",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Belgium,
  },
  {
    name: "Belize",
    dialCode: "501",
    code: "BZ",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Belize,
  },
  {
    name: "Benin",
    dialCode: "229",
    code: "BJ",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Benin,
  },
  {
    name: "Bermuda",
    dialCode: "1441",
    code: "BM",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Bermuda,
  },
  {
    name: "Bhutan",
    dialCode: "975",
    code: "BT",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Bhutan,
  },
  {
    name: "Bolivia",
    dialCode: "591",
    code: "BO",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Bolivia,
  },
  {
    name: "Bonaire, Sint Eustatius and Saba",
    dialCode: "599",
    code: "BQ",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: BonaireSintEustatiusAndSaba,
  },
  {
    name: "Bosnia and Herzegovina",
    dialCode: "387",
    code: "BA",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: BosniaAndHerzegovina
  },
  {
    name: "Botswana",
    dialCode: "267",
    code: "BW",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Botswana
  },
  {
    name: "Bouvet Island",
    dialCode: "47",
    code: "BV",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: BouvetIsland
  },
  {
    name: "Brazil",
    dialCode: "55",
    code: "BR",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Brazil
  },
  {
    name: "British Indian Ocean Territory",
    dialCode: "246",
    code: "IO",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: BritishIndianOceanTerritory
  },
  {
    name: "British Virgin Islands",
    dialCode: "1284",
    code: "VG",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: BritishVirginIslands
  },
  {
    name: "Brunei Darussalam",
    dialCode: "673",
    code: "BN",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: BruneiDarussalam
  },
  {
    name: "Bulgaria",
    dialCode: "359",
    code: "BG",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Bulgaria
  },
  {
    name: "Burkina Faso",
    dialCode: "226",
    code: "BF",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: BurkinaFaso
  },
  {
    name: "Burundi",
    dialCode: "257",
    code: "BI",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Burundi
  },
  {
    name: "Cape Verde",
    dialCode: "238",
    code: "CV",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/],
    flag: CaboVerde,
  },
  {
    name: "Cambodia",
    dialCode: "855",
    code: "KH",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Cambodia
  },
  {
    name: "Cameroon",
    dialCode: "237",
    code: "CM",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Cameroon
  },
  {
    name: "Canada",
    dialCode: "1",
    code: "CA",
    mask: [/\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Canada
  },
  {
    name: "Cayman Islands",
    dialCode: "1345",
    code: "KY",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: CaymanIslands
  },
  {
    name: "Central African Republic",
    dialCode: "236",
    code: "CF",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: CentralAfricanRepublic
  },
  {
    name: "Chad",
    dialCode: "235",
    code: "TD",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Chad
  },
  {
    name: "Chile",
    dialCode: "56",
    code: "CL",
    mask: [/\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Chile
  },
  {
    name: "China",
    dialCode: "86",
    code: "CN",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: China
  },
  {
    name: "Christmas Island",
    dialCode: "61",
    code: "CX",
    mask: [/\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: ChristmasIsland
  },
  {
    name: "Cocos Islands",
    dialCode: "61",
    code: "CC",
    mask: [/\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: CocosIslands
  },
  {
    name: "Colombia",
    dialCode: "57",
    code: "CO",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Colombia
  },
  {
    name: "Comoros",
    dialCode: "269",
    code: "KM",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Comoros
  },
  {
    name: "Cook Islands",
    dialCode: "682",
    code: "CK",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: CookIslands
  },
  {
    name: "Costa Rica",
    dialCode: "506",
    code: "CR",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: CostaRica
  },
  {
    name: "Cote d'Ivoire",
    dialCode: "225",
    code: "CI",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: CoteDivoire,
  },
  {
    name: "Croatia",
    dialCode: "385",
    code: "HR",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Croatia
  },
  {
    name: "Cuba",
    dialCode: "53",
    code: "CU",
    mask: [/\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Cuba
  },
  {
    name: "Curacao",
    dialCode: "599",
    code: "CW",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Curacao
  },
  {
    name: "Cyprus",
    dialCode: "357",
    code: "CY",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Cyprus
  },
  {
    name: "Czech Republic",
    dialCode: "420",
    code: "CZ",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: CzechRepublic
  },
  {
    name: "Democratic Republic of Congo",
    dialCode: "243",
    code: "CD",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: DemocraticRepublicOfCongo
  },
  {
    name: "Denmark",
    dialCode: "45",
    code: "DK",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Denmark
  },
  {
    name: "Djibouti",
    dialCode: "253",
    code: "DJ",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Djibouti
  },
  {
    name: "Dominica",
    dialCode: "1767",
    code: "DM",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Dominica
  },
  {
    name: "Dominican Republic",
    dialCode: "1849",
    code: "DO",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: DominicanRepublic
  },
  {
    name: "Ecuador",
    dialCode: "593",
    code: "EC",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Ecuador
  },
  {
    name: "Egypt",
    dialCode: "20",
    code: "EG",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Egypt
  },
  {
    name: "El Salvador",
    dialCode: "503",
    code: "SV",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: ElSalvador
  },
  {
    name: "Equatorial Guinea",
    dialCode: "240",
    code: "GQ",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: EquatorialGuinea
  },
  {
    name: "Eritrea",
    dialCode: "291",
    code: "ER",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Eritrea
  },
  {
    name: "Estonia",
    dialCode: "372",
    code: "EE",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/],
    flag: Estonia
  },
  {
    name: "Ethiopia",
    dialCode: "251",
    code: "ET",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Ethiopia
  },
  {
    name: "Falkland Islands",
    dialCode: "500",
    code: "FK",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: FalklandIslands
  },
  {
    name: "Faroe Islands",
    dialCode: "298",
    code: "FO",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: FaroeIslands
  },
  {
    name: "Federated States of Micronesia",
    dialCode: "691",
    code: "FM",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: FederatedStatesOfMicronesia
  },
  {
    name: "Fiji",
    dialCode: "679",
    code: "FJ",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Fiji
  },
  {
    name: "Finland",
    dialCode: "358",
    code: "FI",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Finland
  },
  {
    name: "France",
    dialCode: "33",
    code: "FR",
    mask: [/\d/, /\d/, " ",  /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: France
  },
  {
    name: "French Guiana",
    dialCode: "594",
    code: "GF",
    mask: [/\d/, /\d/, /\d/, " ",  /\d/, /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: FrenchGuiana
  },
  {
    name: "Gabon",
    dialCode: "241",
    code: "GA",
    mask: [/\d/, /\d/, /\d/, " ",  /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Gabon
  },
  {
    name: "Gambia",
    dialCode: "220",
    code: "GM",
    mask: [/\d/, /\d/, /\d/, " ",  /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Gambia
  },
  {
    name: "Georgia",
    dialCode: "995",
    code: "GE",
    mask: [/\d/, /\d/, /\d/, " ",  /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Georgia
  },
  {
    name: "Germany",
    dialCode: "49",
    code: "DE",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Germany
  },
  {
    name: "Ghana",
    dialCode: "233",
    code: "GH",
    mask: [/\d/, /\d/, /\d/, " ",  /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Ghana
  },
  {
    name: "Gibraltar",
    dialCode: "350",
    code: "GI",
    mask: [/\d/, /\d/, /\d/, " ",  /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Gibraltar
  },
  {
    name: "Greece",
    dialCode: "30",
    code: "GR",
    mask: [/\d/, /\d/, " ",  /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Greece
  },
  {
    name: "Greenland",
    dialCode: "299",
    code: "GL",
    mask: [/\d/, /\d/, /\d/, " ",  /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Greenland
  },
  {
    name: "Grenada",
    dialCode: "1473",
    code: "GD",
    mask: [/\d/, /\d/, /\d/, /\d/, " ",  /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Grenada
  },
  {
    name: "Guadeloupe",
    dialCode: "590",
    code: "GP",
    mask: [/\d/, /\d/, /\d/, " ",  /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Guadeloupe
  },
  {
    name: "Guam",
    dialCode: "1671",
    code: "GU",
    mask: [/\d/, /\d/, /\d/, /\d/, " ",  /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Guam
  },
  {
    name: "Guatemala",
    dialCode: "502",
    code: "GT",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Guatemala
  },
  {
    name: "Guernsey",
    dialCode: "44",
    code: "GG",
    mask: [/\d/, /\d/, " ",  /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Guernsey
  },
  {
    name: "Guinea",
    dialCode: "224",
    code: "GN",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Guinea
  },
  {
    name: "Guinea-Bissau",
    dialCode: "245",
    code: "GW",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: GuineaBissau
  },
  {
    name: "Guyana",
    dialCode: "592",
    code: "GY",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Guyana
  },
  {
    name: "Haiti",
    dialCode: "509",
    code: "HT",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Haiti
  },
  {
    name: "Heard Island and McDonald Islands",
    dialCode: "672",
    code: "HM",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: HeardIslandAndMcDonaldIslands
  },
  {
    name: "Honduras",
    dialCode: "504",
    code: "HN",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Honduras
  },
  {
    name: "Hong Kong",
    dialCode: "852",
    code: "HK",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: HongKong
  },
  {
    name: "Hungary",
    dialCode: "36",
    code: "HU",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Hungary
  },
  {
    name: "Iceland",
    dialCode: "354",
    code: "IS",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Iceland
  },
  {
    name: "India",
    dialCode: "91",
    code: "IN",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: India
  },
  {
    name: "Indonesia",
    dialCode: "62",
    code: "ID",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/],
    flag: Indonesia
  },
  {
    name: "Iran",
    dialCode: "98",
    code: "IR",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Iran
  },
  {
    name: "Iraq",
    dialCode: "964",
    code: "IQ",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Iraq
  },
  {
    name: "Ireland",
    dialCode: "353",
    code: "IE",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Ireland
  },
  {
    name: "Isle of Man",
    dialCode: "44",
    code: "IM",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: IsleOfMan
  },
  {
    name: "Israel",
    dialCode: "972",
    code: "IL",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Israel
  },
  {
    name: "Italy",
    dialCode: "39",
    code: "IT",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Italy
  },
  {
    name: "Jamaica",
    dialCode: "1876",
    code: "JM",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Jamaica
  },
  {
    name: "Japan",
    dialCode: "81",
    code: "JP",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Japan
  },
  {
    name: "Jersey",
    dialCode: "44",
    code: "JE",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Jersey
  },
  {
    name: "Jordan",
    dialCode: "962",
    code: "JO",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Jordan
  },
  {
    name: "Kazakhstan",
    dialCode: "77",
    code: "KZ",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Kazakhstan
  },
  {
    name: "Kenya",
    dialCode: "254",
    code: "KE",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Kenya
  },
  {
    name: "Kiribati",
    dialCode: "686",
    code: "KI",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Kiribati
  },
  {
    name: "Kosovo",
    dialCode: "383",
    code: "XK",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Kosovo,
  },
  {
    name: "Kuwait",
    dialCode: "965",
    code: "KW",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Kuwait
  },
  {
    name: "Kyrgyzstan",
    dialCode: "996",
    code: "KG",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Kyrgyzstan
  },
  {
    name: "Laos",
    dialCode: "856",
    code: "LA",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Laos
  },
  {
    name: "Latvia",
    dialCode: "371",
    code: "LV",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Latvia
  },
  {
    name: "Lebanon",
    dialCode: "961",
    code: "LB",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Lebanon
  },
  {
    name: "Lesotho",
    dialCode: "266",
    code: "LS",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Lesotho
  },
  {
    name: "Liberia",
    dialCode: "231",
    code: "LR",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Liberia
  },
  {
    name: "Liechtenstein",
    dialCode: "423",
    code: "LI",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Liechtenstein
  },
  {
    name: "Lithuania",
    dialCode: "370",
    code: "LT",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Lithuania
  },
  {
    name: "Luxembourg",
    dialCode: "352",
    code: "LU",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Luxembourg
  },
  {
    name: "Libya",
    dialCode: "218",
    code: "LY",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Lybia
  },
  {
    name: "Macao",
    dialCode: "853",
    code: "MO",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Macao
  },
  {
    name: "Macedonia",
    dialCode: "389",
    code: "MK",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Macedonia
  },
  {
    name: "Madagascar",
    dialCode: "261",
    code: "MG",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Madagascar
  },
  {
    name: "Malawi",
    dialCode: "265",
    code: "MW",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Malawi
  },
  {
    name: "Malaysia",
    dialCode: "60",
    code: "MY",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Malaysia
  },
  {
    name: "Maldives",
    dialCode: "960",
    code: "MV",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Maldives
  },
  {
    name: "Mali",
    dialCode: "223",
    code: "ML",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Mali
  },
  {
    name: "Malta",
    dialCode: "356",
    code: "MT",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Malta
  },
  {
    name: "Marshall Islands",
    dialCode: "692",
    code: "MH",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: MarshallIslands
  },
  {
    name: "Martinique",
    dialCode: "596",
    code: "MQ",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Martinique
  },
  {
    name: "Mauritania",
    dialCode: "222",
    code: "MR",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Mauritania
  },
  {
    name: "Mauritius",
    dialCode: "230",
    code: "MU",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Mauritius
  },
  {
    name: "Mayotte",
    dialCode: "262",
    code: "YT",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Mayotte
  },
  {
    name: "Mexico",
    dialCode: "52",
    code: "MX",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Mexico
  },
  {
    name: "Moldova",
    dialCode: "373",
    code: "MD",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Moldova
  },
  {
    name: "Monaco",
    dialCode: "377",
    code: "MC",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Monaco
  },
  {
    name: "Mongolia",
    dialCode: "976",
    code: "MN",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Mongolia
  },
  {
    name: "Montenegro",
    dialCode: "382",
    code: "ME",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Montenegro
  },
  {
    name: "Montserrat",
    dialCode: "1664",
    code: "MS",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Montserrat
  },
  {
    name: "Morocco",
    dialCode: "212",
    code: "MA",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Morocco
  },
  {
    name: "Mozambique",
    dialCode: "258",
    code: "MZ",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Mozambique
  },
  {
    name: "Myanmar",
    dialCode: "95",
    code: "MM",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Myanmar
  },
  {
    name: "Namibia",
    dialCode: "264",
    code: "NA",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Namibia
  },
  {
    name: "Nauru",
    dialCode: "674",
    code: "NR",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Nauru
  },
  {
    name: "Nepal",
    dialCode: "977",
    code: "NP",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Nepal
  },
  {
    name: "Netherlands Antilles",
    dialCode: "599",
    code: "AN",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: NetherlandsAntilles
  },
  {
    name: "Netherlands",
    dialCode: "31",
    code: "NL",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Netherlands
  },
  {
    name: "New Caledonia",
    dialCode: "687",
    code: "NC",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: NewCaledonia
  },
  {
    name: "New Guinea",
    dialCode: "675",
    code: "PG",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: NewGuinea
  },
  {
    name: "New Zealand",
    dialCode: "64",
    code: "NZ",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: NewZealand
  },
  {
    name: "Nicaragua",
    dialCode: "505",
    code: "NI",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Nicaragua
  },
  {
    name: "Niger",
    dialCode: "227",
    code: "NE",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Niger
  },
  {
    name: "Nigeria",
    dialCode: "234",
    code: "NG",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Nigeria
  },
  {
    name: "Niue",
    dialCode: "683",
    code: "NU",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/],
    flag: Niue
  },
  {
    name: "Norfolk Island",
    dialCode: "672",
    code: "NF",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: NorfolkIslands
  },
  {
    name: "North Korea",
    dialCode: "850",
    code: "KP",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: NorthKorea
  },
  {
    name: "Northern Mariana Islands",
    dialCode: "1670",
    code: "MP",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: NorthenMarianaIslands
  },
  {
    name: "Norway",
    dialCode: "47",
    code: "NO",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Norway
  },
  {
    name: "Oman",
    dialCode: "968",
    code: "OM",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Oman
  },
  {
    name: "Pakistan",
    dialCode: "92",
    code: "PK",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Pakistan
  },
  {
    name: "Palau",
    dialCode: "680",
    code: "PW",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Palau
  },
  {
    name: "Palestine",
    dialCode: "970",
    code: "PS",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Palestine
  },
  {
    name: "Panama",
    dialCode: "507",
    code: "PA",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Panama
  },
  {
    name: "Paraguay",
    dialCode: "595",
    code: "PY",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Paraguay
  },
  {
    name: "Peru",
    dialCode: "51",
    code: "PE",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Peru
  },
  {
    name: "Philippines",
    dialCode: "63",
    code: "PH",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Phillipines
  },
  {
    name: "Pitcairn Islands",
    dialCode: "872",
    code: "PN",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: PitcairnIslands
  },
  {
    name: "Poland",
    dialCode: "48",
    code: "PL",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Poland
  },
  {
    name: "Polynesia",
    dialCode: "689",
    code: "PF",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Polynesia
  },
  {
    name: "Portugal",
    dialCode: "351",
    code: "PT",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Portugal
  },
  {
    name: "Puerto Rico",
    dialCode: "1939",
    code: "PR",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/],
    flag: PuertoRico
  },
  {
    name: "Qatar",
    dialCode: "974",
    code: "QA",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Qatar
  },
  {
    name: "Republic of the Congo",
    dialCode: "242",
    code: "CG",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/,  "-", /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: RepublicOfTheCongo,
  },
  {
    name: "Reunion",
    dialCode: "262",
    code: "RE",
    mask: [ /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Reunion
  },
  {
    name: "Romania",
    dialCode: "40",
    code: "RO",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Romania
  },
  {
    name: "Russia",
    dialCode: "7",
    code: "RU",
    mask: [/\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Russia,
  },
  {
    name: "Rwanda",
    dialCode: "250",
    code: "RW",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Rwanda
  },
  {
    name: "Saint Vincent and the Grenadines",
    dialCode: "1784",
    code: "VC",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: SainVincentAndGrenadines
  },
  {
    name: "Saint Barthelemy",
    dialCode: "590",
    code: "BL",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: SaintBarthelemy
  },
  {
    name: "Saint Kitts and Nevis",
    dialCode: "1869",
    code: "KN",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: SaintKittsAndNevis
  },
  {
    name: "Saint Lucia",
    dialCode: "1758",
    code: "LC",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: SaintLucia
  },
  {
    name: "Saint Martin",
    dialCode: "590",
    code: "MF",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: SaintMartin
  },
  {
    name: "Saint Pierre and Miquelon",
    dialCode: "508",
    code: "PM",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: SaintPierreAndMiquelon
  },
  {
    name: "Samoa",
    dialCode: "685",
    code: "WS",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Samoa
  },
  {
    name: "San Marino",
    dialCode: "378",
    code: "SM",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: SanMarino
  },
  {
    name: "Sao Tome and Principe",
    dialCode: "239",
    code: "ST",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: SaoTomeAndPrincipe
  },
  {
    name: "Saudi Arabia",
    dialCode: "966",
    code: "SA",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: SaudiArabia
  },
  {
    name: "Senegal",
    dialCode: "221",
    code: "SN",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Senegal
  },
  {
    name: "Serbia",
    dialCode: "381",
    code: "RS",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Serbia
  },
  {
    name: "Seychelles",
    dialCode: "248",
    code: "SC",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Seychelles
  },
  {
    name: "Sierra Leone",
    dialCode: "232",
    code: "SL",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: SierraLeone
  },
  {
    name: "Singapore",
    dialCode: "65",
    code: "SG",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Singapore
  },
  {
    name: "Sint Maarten",
    dialCode: "1721",
    code: "SX",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: SintMaarten,
  },
  {
    name: "Slovakia",
    dialCode: "421",
    code: "SK",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Slovakia
  },
  {
    name: "Slovenia",
    dialCode: "386",
    code: "SI",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Slovenia
  },
  {
    name: "Solomon Islands",
    dialCode: "677",
    code: "SB",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: SolomonIslands
  },
  {
    name: "Somalia",
    dialCode: "252",
    code: "SO",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Somalia
  },
  {
    name: "South Africa",
    dialCode: "27",
    code: "ZA",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: SouthAfrica
  },
  {
    name: "South Georgia and Sandwich Islands",
    dialCode: "500",
    code: "GS",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: SouthGeorgiaAndSandwichIslands
  },
  {
    name: "South Korea",
    dialCode: "82",
    code: "KR",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: SouthKorea
  },
  {
    name: "South Sudan",
    dialCode: "211",
    code: "SS",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: SouthSudan,
  },
  {
    name: "Spain",
    dialCode: "34",
    code: "ES",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Spain
  },
  {
    name: "Sri Lanka",
    dialCode: "94",
    code: "LK",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: SriLanka
  },
  {
    name: "Sudan",
    dialCode: "249",
    code: "SD",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Sudan
  },
  {
    name: "Suriname",
    dialCode: "597",
    code: "SR",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Suriname
  },
  {
    name: "Svalbard and Jan Mayen",
    dialCode: "47",
    code: "SJ",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: SvalbardAndJanMayen
  },
  {
    name: "Swaziland",
    dialCode: "268",
    code: "SZ",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Swaziland
  },
  {
    name: "Sweden",
    dialCode: "46",
    code: "SE",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Sweden
  },
  {
    name: "Switzerland",
    dialCode: "41",
    code: "CH",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Switzerland
  },
  {
    name: "Syria",
    dialCode: "963",
    code: "SY",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Syria
  },
  {
    name: "Taiwan",
    dialCode: "886",
    code: "TW",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Taiwan
  },
  {
    name: "Tajikistan",
    dialCode: "992",
    code: "TJ",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Tajikistan
  },
  {
    name: "Tanzania",
    dialCode: "255",
    code: "TZ",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Tanzania
  },
  {
    name: "Thailand",
    dialCode: "66",
    code: "TH",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Thailand
  },
  {
    name: "Timor-Leste",
    dialCode: "670",
    code: "TL",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: TimorLeste
  },
  {
    name: "Togo",
    dialCode: "228",
    code: "TG",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Togo
  },
  {
    name: "Tokelau",
    dialCode: "690",
    code: "TK",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/],
    flag: Tokelau
  },
  {
    name: "Tonga",
    dialCode: "676",
    code: "TO",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Tonga
  },
  {
    name: "Trinidad and Tobago",
    dialCode: "1868",
    code: "TT",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: TrinidadAndTobago
  },
  {
    name: "Tunisia",
    dialCode: "216",
    code: "TN",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Tunisia
  },
  {
    name: "Turkey",
    dialCode: "90",
    code: "TR",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Turkey
  },
  {
    name: "Turkmenistan",
    dialCode: "993",
    code: "TM",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Turkmenistan
  },
  {
    name: "Turks and Caicos Islands",
    dialCode: "1649",
    code: "TC",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: TurksAndCaicosIslands
  },
  {
    name: "Tuvalu",
    dialCode: "688",
    code: "TV",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Tuvalu
  },
  {
    name: "Uganda",
    dialCode: "256",
    code: "UG",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Uganda
  },
  {
    name: "Ukraine",
    dialCode: "380",
    code: "UA",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Ukraine
  },
  {
    name: "United Arab Emirates",
    dialCode: "971",
    code: "AE",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: UnitedArabEmirates
  },
  {
    name: "United Kingdom",
    dialCode: "44",
    code: "GB",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: UnitedKingdom
  },
  {
    name: "United States Virgin Islands",
    dialCode: "1340",
    code: "VI",
    mask: [/\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: UnitedStatesVirginIslands
  },
  {
    name: "United States",
    dialCode: "1",
    code: "US",
    mask: [/\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: UnitedStates
  },
  {
    name: "Uruguay",
    dialCode: "598",
    code: "UY",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Uruguay
  },
  {
    name: "Uzbekistan",
    dialCode: "998",
    code: "UZ",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Uzbekistan
  },
  {
    name: "Vanuatu",
    dialCode: "678",
    code: "VU",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Vunuatu
  },
  {
    name: "Vatican",
    dialCode: "379",
    code: "VA",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Vatican
  },
  {
    name: "Venezuela",
    dialCode: "58",
    code: "VE",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Venezuela
  },
  {
    name: "Vietnam",
    dialCode: "84",
    code: "VN",
    mask: [/\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Vietnam
  },
  {
    name: "Wallis and Futuna",
    dialCode: "681",
    code: "WF",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: WallisAndFutuna
  },
  {
    name: "Yemen",
    dialCode: "967",
    code: "YE",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Yemen
  },
  {
    name: "Zambia",
    dialCode: "260",
    code: "ZM",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Zambia
  },
  {
    name: "Zimbabwe",
    dialCode: "263",
    code: "ZW",
    mask: [/\d/, /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Zimbabwe
  },
];
