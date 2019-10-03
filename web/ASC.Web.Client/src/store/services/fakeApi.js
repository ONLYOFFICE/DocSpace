import axios from "axios";

const BASE_URL = "http://localhost";
const PORT = "8080";
const PREFIX = "api";
const VERSION = "2.0";
const API_URL = `${BASE_URL}:${PORT}/${PREFIX}/${VERSION}`;

function fakeResponse(data) {
  return Promise.resolve({
    data: {
      response: data
    }
  });
}

export function login(data) {
  return axios.post(`${API_URL}/authentication`, data);
}

export function getModulesList() {
  let data = [
    {
      title: "Documents",
      link: "/products/files/",
      imageUrl: "images/documents240.png",
      description:
        "Create, edit and share documents. Collaborate on them in real-time. 100% compatibility with MS Office formats guaranteed.",
      isPrimary: true
    },
    {
      title: "People",
      link: "/products/people/",
      imageUrl: "images/people.svg",
      isPrimary: false
    }
  ];

  return fakeResponse(data);
}

export function getUser() {
  let data = {
    index: "a",
    type: "person",
    id: "2881e6c6-7c9a-11e9-81fb-0242ac120002",
    timestamp: null,
    crtdate: null,
    displayCrtdate: "NaN:NaN PM NaN/NaN/NaN",
    displayDateCrtdate: "NaN/NaN/NaN",
    displayTimeCrtdate: "NaN:NaN PM",
    trtdate: null,
    displayTrtdate: "",
    displayDateTrtdate: "",
    displayTimeTrtdate: "",
    birthday: null,
    userName: "",
    firstName: "",
    lastName: "",
    displayName: "Administrator ",
    email: "paul.bannov@gmail.com",
    tel: "",
    contacts: {
      mailboxes: [
        {
          type: 0,
          name: "mail",
          title: "paul.bannov@gmail.com",
          label: "Email",
          istop: false,
          val: "paul.bannov@gmail.com"
        }
      ],
      telephones: [],
      links: []
    },
    avatar: "/skins/default/images/default_user_photo_size_32-32.png",
    avatarBig: "/skins/default/images/default_user_photo_size_82-82.png",
    avatarSmall: "/skins/default/images/default_user_photo_size_32-32.png",
    groups: [],
    status: 0,
    activationStatus: 0,
    isActivated: false,
    isPending: false,
    isTerminated: false,
    isMe: true,
    isManager: false,
    isPortalOwner: true,
    isAdmin: true,
    listAdminModules: [],
    isVisitor: false,
    isOutsider: false,
    sex: "",
    location: "",
    title: "",
    notes: "",
    culture: "",
    profileUrl: "/products/people/profile.aspx?user=administrator",
    isLDAP: false,
    isSSO: false
  };

  return fakeResponse(data);
}

export function getSettings() {
  const data = {
    timezone:
      "Russian Standard Time;180;(UTC+03:00) Moscow, St. Petersburg;Russia TZ 2 Standard Time;Russia TZ 2 Daylight Time;[01:01:0001;12:31:2010;60;[0;02:00:00;3;5;0;];[0;03:00:00;10;5;0;];][01:01:2011;12:31:2011;60;[0;02:00:00;3;5;0;];[0;00:00:00;1;1;6;];][01:01:2012;12:31:2012;0;[1;00:00:00;1;1;];[1;00:00:00.001;1;1;];60;][01:01:2013;12:31:2013;0;[1;00:00:00;1;1;];[1;00:00:00.001;1;1;];60;][01:01:2014;12:31:2014;60;[0;00:00:00;1;1;3;];[0;02:00:00;10;5;0;];];",
    trustedDomains: [],
    trustedDomainsType: 1,
    culture: "ru-RU",
    utcOffset: "03:00:00",
    utcHoursOffset: 3
  };

  return fakeResponse(data);
}

export function getPasswordSettings() {
  const data = {
    minLength: 12,
    upperCase: true,
    digits: true,
    specSymbols: true
  };

  return fakeResponse(data);
}

export function createUser() {
  const data = {
    id: "00000000-0000-0000-0000-000000000000"
  };
  return fakeResponse(data);
}

export function validateConfirmLink(link) {
  const data = {
    isValid: true
  };
  return fakeResponse(data);
}

export function changePassword() {
  const data = { password: "password" };

  return fakeResponse(data);
}

export function updateActivationStatus() {

  return fakeResponse();
}

export function updateUser(data) {
  return fakeResponse(data);
}

export function checkConfirmLink(data) {
  return fakeResponse(data);
}

export function deleteUser(data) {
  return fakeResponse(data);
}

export function updateUserStatus(data) {
  return fakeResponse(data);
}

export function sendInstructionsToChangePassword() {
  return fakeResponse("Instruction has been sent successfully");
}
