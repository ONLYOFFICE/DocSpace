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
  Russia,
} from "./svg";

export const options = [
  {
    name: "Afghanistan",
    dialCode: "+93",
    code: "AF",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Afghanistan,
  },
  {
    name: "Aland Islands",
    dialCode: "+358",
    code: "AX",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: AlandIslands,
  },
  {
    name: "Albania",
    dialCode: "+355",
    code: "AL",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Albania,
  },
  {
    name: "Algeria",
    dialCode: "+213",
    code: "DZ",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Algeria,
  },
  {
    name: "American Samoa",
    dialCode: "+1684",
    code: "AS",
    mask: ["+", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: AmericanSamoa,
  },
  {
    name: "Andorra",
    dialCode: "+376",
    code: "AD",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Andorra,
  },
  {
    name: "Angola",
    dialCode: "+244",
    code: "AO",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Angola,
  },
  {
    name: "Anguilla",
    dialCode: "+1264",
    code: "AI",
    mask: ["+", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Anguilla,
  },
  {
    name: "Antarctica",
    dialCode: "+6721",
    code: "AQ",
    mask: ["+", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Antarctica,
  },
  {
    name: "Antigua and Barbuda",
    dialCode: "+1268",
    code: "AG",
    mask: ["+", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: AntiguaAndBarbuda,
  },
  {
    name: "Argentina",
    dialCode: "+54",
    code: "AR",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Argentina,
  },
  {
    name: "Armenia",
    dialCode: "+374",
    code: "AM",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Armenia,
  },
  {
    name: "Aruba",
    dialCode: "+297",
    code: "AW",
    mask: [ "+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Aruba,
  },
  {
    name: "Australia",
    dialCode: "+61",
    code: "AU",
    mask: ["+", /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Australia,
  },
  {
    name: "Austria",
    dialCode: "+43",
    code: "AT",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Austria,
  },
  {
    name: "Azerbaijan",
    dialCode: "+994",
    code: "AZ",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Azerbaijan,
  },
  {
    name: "Bahamas",
    dialCode: "+1242",
    code: "BS",
    mask: ["+", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/,],
    flag: Bahamas,
  },
  {
    name: "Bahrain",
    dialCode: "+973",
    code: "BH",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Bahrain,
  },
  {
    name: "Bangladesh",
    dialCode: "+880",
    code: "BD",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Bangladesh,
  },
  {
    name: "Barbados",
    dialCode: "+1246",
    code: "BB",
    mask: ["+", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Barbados,
  },
  {
    name: "Belarus",
    dialCode: "+375",
    code: "BY",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Belarus,
  },
  {
    name: "Belgium",
    dialCode: "+32",
    code: "BE",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Belgium,
  },
  {
    name: "Belize",
    dialCode: "+501",
    code: "BZ",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Belize,
  },
  {
    name: "Benin",
    dialCode: "+229",
    code: "BJ",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Benin,
  },
  {
    name: "Bermuda",
    dialCode: "+1441",
    code: "BM",
    mask: ["+", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Bermuda,
  },
  {
    name: "Bhutan",
    dialCode: "+975",
    code: "BT",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Bhutan,
  },
  {
    name: "Bolivia",
    dialCode: "+591",
    code: "BO",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Bolivia,
  },
  {
    name: "Bonaire, Sint Eustatius and Saba",
    dialCode: "+599",
    code: "BQ",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: BonaireSintEustatiusAndSaba,
  },
  {
    name: "Bosnia and Herzegovina",
    dialCode: "+387",
    code: "BA",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: BosniaAndHerzegovina
  },
  {
    name: "Botswana",
    dialCode: "+267",
    code: "BW",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Botswana
  },
  {
    name: "Bouvet Island",
    dialCode: "+47",
    code: "BV",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: BouvetIsland
  },
  {
    name: "Brazil",
    dialCode: "+55",
    code: "BR",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Brazil
  },
  {
    name: "British Indian Ocean Territory",
    dialCode: "+246",
    code: "IO",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: BritishIndianOceanTerritory
  },
  {
    name: "British Virgin Islands",
    dialCode: "+1284",
    code: "VG",
    mask: ["+", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: BritishVirginIslands
  },
  {
    name: "Brunei Darussalam",
    dialCode: "+673",
    code: "BN",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: BruneiDarussalam
  },
  {
    name: "Bulgaria",
    dialCode: "+359",
    code: "BG",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Bulgaria
  },
  {
    name: "Burkina Faso",
    dialCode: "+226",
    code: "BF",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: BurkinaFaso
  },
  {
    name: "Burundi",
    dialCode: "+257",
    code: "BI",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Burundi
  },
  {
    name: "Cape Verde",
    dialCode: "+238",
    code: "CV",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/],
    flag: CaboVerde,
  },
  {
    name: "Cambodia",
    dialCode: "+855",
    code: "KH",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Cambodia
  },
  {
    name: "Cameroon",
    dialCode: "+237",
    code: "CM",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Cameroon
  },
  {
    name: "Canada",
    dialCode: "+1",
    code: "CA",
    mask: ["+", /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Canada
  },
  {
    name: "Cayman Islands",
    dialCode: "+1345",
    code: "KY",
    mask: ["+", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: CaymanIslands
  },
  {
    name: "Central African Republic",
    dialCode: "+236",
    code: "CF",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: CentralAfricanRepublic
  },
  {
    name: "Chad",
    dialCode: "+235",
    code: "TD",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Chad
  },
  {
    name: "Chile",
    dialCode: "+56",
    code: "CL",
    mask: ["+", /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Chile
  },
  {
    name: "China",
    dialCode: "+86",
    code: "CN",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: China
  },
  {
    name: "Christmas Island",
    dialCode: "+61",
    code: "CX",
    mask: ["+", /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: ChristmasIsland
  },
  {
    name: "Cocos Islands",
    dialCode: "+61",
    code: "CC",
    mask: ["+", /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: CocosIslands
  },
  {
    name: "Colombia",
    dialCode: "+57",
    code: "CO",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Colombia
  },
  {
    name: "Comoros",
    dialCode: "+269",
    code: "KM",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Comoros
  },
  {
    name: "Cook Islands",
    dialCode: "+682",
    code: "CK",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: CookIslands
  },
  {
    name: "Costa Rica",
    dialCode: "+506",
    code: "CR",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: CostaRica
  },
  {
    name: "Cote d'Ivoire",
    dialCode: "+225",
    code: "CI",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: CoteDivoire,
  },
  {
    name: "Croatia",
    dialCode: "+385",
    code: "HR",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Croatia
  },
  {
    name: "Cuba",
    dialCode: "+53",
    code: "CU",
    mask: ["+", /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Cuba
  },
  {
    name: "Curacao",
    dialCode: "+599",
    code: "CW",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Curacao
  },
  {
    name: "Cyprus",
    dialCode: "+357",
    code: "CY",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Cyprus
  },
  {
    name: "Czech Republic",
    dialCode: "+420",
    code: "CZ",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: CzechRepublic
  },
  {
    name: "Democratic Republic of Congo",
    dialCode: "+243",
    code: "CD",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: DemocraticRepublicOfCongo
  },
  {
    name: "Denmark",
    dialCode: "+45",
    code: "DK",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Denmark
  },
  {
    name: "Djibouti",
    dialCode: "+253",
    code: "DJ",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Djibouti
  },
  {
    name: "Dominica",
    dialCode: "+1767",
    code: "DM",
    mask: ["+", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Dominica
  },
  {
    name: "Dominican Republic",
    dialCode: "+1849",
    code: "DO",
    mask: ["+", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: DominicanRepublic
  },
  {
    name: "Ecuador",
    dialCode: "+593",
    code: "EC",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Ecuador
  },
  {
    name: "Egypt",
    dialCode: "+20",
    code: "EG",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Egypt
  },
  {
    name: "El Salvador",
    dialCode: "+503",
    code: "SV",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: ElSalvador
  },
  {
    name: "Equatorial Guinea",
    dialCode: "+240",
    code: "GQ",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: EquatorialGuinea
  },
  {
    name: "Eritrea",
    dialCode: "+291",
    code: "ER",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Eritrea
  },
  {
    name: "Estonia",
    dialCode: "+372",
    code: "EE",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/],
    flag: Estonia
  },
  {
    name: "Ethiopia",
    dialCode: "+251",
    code: "ET",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Ethiopia
  },
  {
    name: "Falkland Islands",
    dialCode: "+500",
    code: "FK",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: FalklandIslands
  },
  {
    name: "Faroe Islands",
    dialCode: "+298",
    code: "FO",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: FaroeIslands
  },
  {
    name: "Federated States of Micronesia",
    dialCode: "+691",
    code: "FM",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: FederatedStatesOfMicronesia
  },
  {
    name: "Fiji",
    dialCode: "+679",
    code: "FJ",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Fiji
  },
  {
    name: "Finland",
    dialCode: "+358",
    code: "FI",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Finland
  },
  {
    name: "France",
    dialCode: "+33",
    code: "FR",
    mask: ["+", /\d/, /\d/, " ",  /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: France
  },
  {
    name: "French Guiana",
    dialCode: "+594",
    code: "GF",
    mask: ["+", /\d/, /\d/, /\d/, " ",  /\d/, /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: FrenchGuiana
  },
  {
    name: "Gabon",
    dialCode: "+241",
    code: "GA",
    mask: ["+", /\d/, /\d/, /\d/, " ",  /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Gabon
  },
  {
    name: "Gambia",
    dialCode: "+220",
    code: "GM",
    mask: ["+", /\d/, /\d/, /\d/, " ",  /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Gambia
  },
  {
    name: "Georgia",
    dialCode: "+995",
    code: "GE",
    mask: ["+", /\d/, /\d/, /\d/, " ",  /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Georgia
  },
  {
    name: "Germany",
    dialCode: "+49",
    code: "DE",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Germany
  },
  {
    name: "Ghana",
    dialCode: "+233",
    code: "GH",
    mask: ["+", /\d/, /\d/, /\d/, " ",  /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Ghana
  },
  {
    name: "Gibraltar",
    dialCode: "+350",
    code: "GI",
    mask: ["+", /\d/, /\d/, /\d/, " ",  /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Gibraltar
  },
  {
    name: "Greece",
    dialCode: "+30",
    code: "GR",
    mask: ["+", /\d/, /\d/, " ",  /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Greece
  },
  {
    name: "Greenland",
    dialCode: "+299",
    code: "GL",
    mask: ["+", /\d/, /\d/, /\d/, " ",  /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Greenland
  },
  {
    name: "Grenada",
    dialCode: "+1473",
    code: "GD",
    mask: ["+", /\d/, /\d/, /\d/, /\d/, " ",  /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Grenada
  },
  {
    name: "Guadeloupe",
    dialCode: "+590",
    code: "GP",
    mask: ["+", /\d/, /\d/, /\d/, " ",  /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Guadeloupe
  },
  {
    name: "Guam",
    dialCode: "+1671",
    code: "GU",
    mask: ["+", /\d/, /\d/, /\d/, /\d/, " ",  /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Guam
  },
  {
    name: "Guatemala",
    dialCode: "+502",
    code: "GT",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Guatemala
  },
  {
    name: "Guernsey",
    dialCode: "+44",
    code: "GG",
    mask: ["+", /\d/, /\d/, " ",  /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Guernsey
  },
  {
    name: "Guinea",
    dialCode: "+224",
    code: "GN",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Guinea
  },
  {
    name: "Guinea-Bissau",
    dialCode: "+245",
    code: "GW",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: GuineaBissau
  },
  {
    name: "Guyana",
    dialCode: "+592",
    code: "GY",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Guyana
  },
  {
    name: "Haiti",
    dialCode: "+509",
    code: "HT",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Haiti
  },
  {
    name: "Heard Island and McDonald Islands",
    dialCode: "+672",
    code: "HM",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: HeardIslandAndMcDonaldIslands
  },
  {
    name: "Honduras",
    dialCode: "+504",
    code: "HN",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Honduras
  },
  {
    name: "Hong Kong",
    dialCode: "+852",
    code: "HK",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: HongKong
  },
  {
    name: "Hungary",
    dialCode: "+36",
    code: "HU",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Hungary
  },
  {
    name: "Iceland",
    dialCode: "+354",
    code: "IS",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Iceland
  },
  {
    name: "India",
    dialCode: "+91",
    code: "IN",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: India
  },
  {
    name: "Indonesia",
    dialCode: "+62",
    code: "ID",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/],
    flag: Indonesia
  },
  {
    name: "Iran",
    dialCode: "+98",
    code: "IR",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Iran
  },
  {
    name: "Iraq",
    dialCode: "+964",
    code: "IQ",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Iraq
  },
  {
    name: "Ireland",
    dialCode: "+353",
    code: "IE",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Ireland
  },
  {
    name: "Isle of Man",
    dialCode: "+44",
    code: "IM",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: IsleOfMan
  },
  {
    name: "Israel",
    dialCode: "+972",
    code: "IL",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Israel
  },
  {
    name: "Italy",
    dialCode: "+39",
    code: "IT",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Italy
  },
  {
    name: "Jamaica",
    dialCode: "+1876",
    code: "JM",
    mask: ["+", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Jamaica
  },
  {
    name: "Japan",
    dialCode: "+81",
    code: "JP",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Japan
  },
  {
    name: "Jersey",
    dialCode: "+44",
    code: "JE",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Jersey
  },
  {
    name: "Jordan",
    dialCode: "+962",
    code: "JO",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Jordan
  },
  {
    name: "Kazakhstan",
    dialCode: "+77",
    code: "KZ",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Kazakhstan
  },
  {
    name: "Kenya",
    dialCode: "+254",
    code: "KE",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Kenya
  },
  {
    name: "Kiribati",
    dialCode: "+686",
    code: "KI",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Kiribati
  },
  {
    name: "Kosovo",
    dialCode: "+383",
    code: "XK",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Kosovo,
  },
  {
    name: "Kuwait",
    dialCode: "+965",
    code: "KW",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Kuwait
  },
  {
    name: "Kyrgyzstan",
    dialCode: "+996",
    code: "KG",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Kyrgyzstan
  },
  {
    name: "Laos",
    dialCode: "+856",
    code: "LA",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Laos
  },
  {
    name: "Latvia",
    dialCode: "+371",
    code: "LV",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Latvia
  },
  {
    name: "Lebanon",
    dialCode: "+961",
    code: "LB",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Lebanon
  },
  {
    name: "Lesotho",
    dialCode: "+266",
    code: "LS",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Lesotho
  },
  {
    name: "Liberia",
    dialCode: "+231",
    code: "LR",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Liberia
  },
  {
    name: "Liechtenstein",
    dialCode: "+423",
    code: "LI",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Liechtenstein
  },
  {
    name: "Lithuania",
    dialCode: "+370",
    code: "LT",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Lithuania
  },
  {
    name: "Luxembourg",
    dialCode: "+352",
    code: "LU",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Luxembourg
  },
  {
    name: "Libya",
    dialCode: "+218",
    code: "LY",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Lybia
  },
  {
    name: "Macao",
    dialCode: "+853",
    code: "MO",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Macao
  },
  {
    name: "Macedonia",
    dialCode: "+389",
    code: "MK",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Macedonia
  },
  {
    name: "Madagascar",
    dialCode: "+261",
    code: "MG",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Madagascar
  },
  {
    name: "Malawi",
    dialCode: "+265",
    code: "MW",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, "-", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Malawi
  },
  {
    name: "Malaysia",
    dialCode: "+60",
    code: "MY",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Malaysia
  },
  {
    name: "Maldives",
    dialCode: "+960",
    code: "MV",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Maldives
  },
  {
    name: "Mali",
    dialCode: "+223",
    code: "ML",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Mali
  },
  {
    name: "Malta",
    dialCode: "+356",
    code: "MT",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Malta
  },
  {
    name: "Marshall Islands",
    dialCode: "+692",
    code: "MH",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: MarshallIslands
  },
  {
    name: "Martinique",
    dialCode: "+596",
    code: "MQ",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Martinique
  },
  {
    name: "Mauritania",
    dialCode: "+222",
    code: "MR",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Mauritania
  },
  {
    name: "Mauritius",
    dialCode: "+230",
    code: "MU",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Mauritius
  },
  {
    name: "Mayotte",
    dialCode: "+262",
    code: "YT",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Mayotte
  },
  {
    name: "Mexico",
    dialCode: "+52",
    code: "MX",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Mexico
  },
  {
    name: "Moldova",
    dialCode: "+373",
    code: "MD",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Moldova
  },
  {
    name: "Monaco",
    dialCode: "+377",
    code: "MC",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Monaco
  },
  {
    name: "Mongolia",
    dialCode: "+976",
    code: "MN",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Mongolia
  },
  {
    name: "Montenegro",
    dialCode: "+382",
    code: "ME",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Montenegro
  },
  {
    name: "Montserrat",
    dialCode: "+1664",
    code: "MS",
    mask: ["+", /\d/, /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/],
    flag: Montserrat
  },
  {
    name: "Morocco",
    dialCode: "+212",
    code: "MA",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Morocco
  },
  {
    name: "Mozambique",
    dialCode: "+258",
    code: "MZ",
    mask: ["+", /\d/, /\d/, /\d/, " ", /\d/, /\d/, "-", /\d/, /\d/, /\d/, "-", /\d/, /\d/, /\d/],
    flag: Mozambique
  },
  {
    name: "Myanmar",
    dialCode: "+95",
    code: "MM",
    mask: ["+", /\d/, /\d/, " ", /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/],
    flag: Myanmar
  },
  {
    name: "Russia",
    dialCode: "+7",
    code: "RU",
    mask: ["+", /\d/, " ", /\d/, /\d/, /\d/, " ", /\d/, /\d/, /\d/, "-", /\d/, /\d/, "-", /\d/, /\d/],
    flag: Russia,
  },
];
