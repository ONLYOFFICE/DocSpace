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
  const data = [
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

export function getPortalPasswordSettings() {
  const data = {
    minLength: 6,
    upperCase: false,
    digits: false,
    specSymbols: false
  };

  return fakeResponse(data);
}

export function getUser() {
  const data = {
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

export function getUsers() {
  const data = [
    {
      id: "657d1d0e-c9f3-4c9d-bd48-07525e539fd6",
      userName: "Alexey.Safronov",
      isVisitor: false,
      firstName: "Alexey",
      lastName: "Safronov",
      email: "Alexey.Safronov@avsmedia.net",
      status: 1,
      activationStatus: 1,
      terminated: null,
      department: "",
      workFrom: "2017-07-11T20:22:53.0000000+03:00",
      displayName: "Safronov Alexey",
      contacts: [
        {
          type: "mail",
          value: "alexey.safronov@onlyoffice.com"
        }
      ],
      avatarMedium: "/images/default_user_photo_size_48-48.png?_=-48038267",
      avatar: "/images/default_user_photo_size_82-82.png?_=-48038267",
      isAdmin: false,
      isLDAP: true,
      isOwner: false,
      isSSO: false,
      avatarSmall: "/images/default_user_photo_size_32-32.png?_=-48038267",
      profileUrl:
        "http://localhost:8092/products/people/profile.aspx?user=alexey.safronov"
    },
    {
      id: "646a6cff-df57-4b83-8ffe-91a24910328c",
      userName: "Nikolay.Ivanov",
      isVisitor: false,
      firstName: "Nikolay",
      lastName: "Ivanov",
      email: "profi.troll@outlook.com",
      birthday: "1983-09-15T04:00:00.0000000+04:00",
      sex: "male",
      status: 1,
      activationStatus: 1,
      terminated: null,
      department: "",
      workFrom: "2007-10-03T04:00:00.0000000+04:00",
      displayName: "Ivanov Nikolay",
      mobilePhone: "79081612979",
      avatarMedium: "/images/default_user_photo_size_48-48.png?_=-562774739",
      avatar: "/images/default_user_photo_size_82-82.png?_=-562774739",
      isAdmin: true,
      isLDAP: false,
      listAdminModules: ["people"],
      isOwner: true,
      isSSO: false,
      avatarSmall: "/images/default_user_photo_size_32-32.png?_=-562774739",
      profileUrl:
        "http://localhost:8092/products/people/profile.aspx?user=nikolay.ivanov"
    }
  ];

  return fakeResponse(data);
}

export function getGroups() {
  const data = [
    {
      id: "0824d8a0-d860-46df-8142-9313bb298a5c",
      name: "Отдел продвижения и рекламы",
      manager: null
    },
    {
      id: "098f3dac-92bd-455f-8138-966313c098da",
      name: "Группа интернет-рекламы",
      manager: "galina.medvedeva"
    },
    {
      id: "1518cad6-c2b9-4644-bcb9-3fc816714ecc",
      name: "Отдел тестирования",
      manager: null
    },
    {
      id: "1d42a4fb-755e-44ab-bcf5-38482c9b2415",
      name: "Отдел программирования форматов",
      manager: "Sergey.Kirillov"
    },
    {
      id: "36800583-b347-4646-b303-65d969fabec1",
      name: "Рига",
      manager: "Kate.Osipova"
    },
    {
      id: "3b99e536-7b68-44c4-8d44-6e745fe48348",
      name: "Группа проектирования",
      manager: "Anna.Medvedeva"
    },
    {
      id: "40e2b7b4-bdb8-43f8-a012-5ab382754dba",
      name: "Группа технической поддержки клиентов",
      manager: "Alexey.Micheev"
    },
    {
      id: "5fd54d7e-aff8-435f-85d0-af0e2129be85",
      name: "Группа PR и продвижения",
      manager: "Alexander.Galkin"
    },
    {
      id: "613fc896-3ddd-4de1-a567-edbbc6cf1fc8",
      name: "Администрация",
      manager: "Lev.Bannov"
    },
    {
      id: "70fe34d0-e589-4810-bcf8-f3a791db20bf",
      name: "Отдел технической документации",
      manager: "Alexander.Vnuchkov"
    },
    {
      id: "72940d26-57f7-4994-aff9-e505e48558d4",
      name: "Даллас",
      manager: null
    },
    {
      id: "cc8eea30-1260-427e-83c4-ff9e9680edba",
      name: "Отдел интернет-приложений",
      manager: "Alex.Bannov"
    },
    {
      id: "f34754ff-ba4f-4f5e-bc35-1ea2921b2c44",
      name: "Отдел продаж",
      manager: "galina.goduhina"
    },
    {
      id: "ff00f2ea-2960-4fe9-b477-6a5a6ab14187",
      name: "Группа по работе с клиентами",
      manager: "Evgenia.Olshanskaja"
    }
  ];

  return fakeResponse(data);
}

export function createUser(data) {
  data.id = "00000000-0000-0000-0000-000000000000";
  return fakeResponse(data);
}

export function updateUser(data) {
  return fakeResponse(data);
}

export function updateUserStatus(status, userIds) {
  return fakeResponse([
    {
      id: userIds[0],
      userName: "Mike.Zanyatski",
      isVisitor: false,
      firstName: "Mike",
      lastName: "Zanyatski",
      email: "my@gmail.com",
      birthday: "2019-08-19T01:39:25.3240031Z",
      sex: "male",
      status: status,
      activationStatus: 0,
      terminated: "2019-08-19T01:39:25.3240031Z",
      department: "Marketing",
      workFrom: "2019-08-19T01:39:25.3240031Z",
      location: "Palo Alto",
      notes: "Notes to worker",
      displayName: null,
      title: "Manager",
      contacts: [
        {
          type: "GTalk",
          value: "my@gmail.com"
        }
      ],
      groups: [
        {
          id: "00000000-0000-0000-0000-000000000000",
          name: "Group Name",
          manager: "Jake.Zazhitski"
        }
      ],
      avatarMedium: "url to medium avatar",
      avatar: "url to big avatar",
      isAdmin: false,
      isLDAP: false,
      listAdminModules: ["projects", "crm"],
      isOwner: false,
      cultureName: "en-EN",
      isSSO: false,
      avatarSmall: "url to small avatar",
      profileUrl: ""
    }
  ]);
}

export function updateUserType(type, userIds) {
  return fakeResponse([
    {
      id: userIds[0],
      userName: "Mike.Zanyatski",
      isVisitor: type === 1 ? false : true,
      firstName: "Mike",
      lastName: "Zanyatski",
      email: "my@gmail.com",
      birthday: "2019-08-19T01:39:25.3240031Z",
      sex: "male",
      status: 1,
      activationStatus: 0,
      terminated: "2019-08-19T01:39:25.3240031Z",
      department: "Marketing",
      workFrom: "2019-08-19T01:39:25.3240031Z",
      location: "Palo Alto",
      notes: "Notes to worker",
      displayName: null,
      title: "Manager",
      contacts: [
        {
          type: "GTalk",
          value: "my@gmail.com"
        }
      ],
      groups: [
        {
          id: "00000000-0000-0000-0000-000000000000",
          name: "Group Name",
          manager: "Jake.Zazhitski"
        }
      ],
      avatarMedium: "url to medium avatar",
      avatar: "url to big avatar",
      isAdmin: false,
      isLDAP: false,
      listAdminModules: ["projects", "crm"],
      isOwner: false,
      cultureName: "en-EN",
      isSSO: false,
      avatarSmall: "url to small avatar",
      profileUrl: ""
    }
  ]);
}

export function resendUserInvites(userIds) {
  return fakeResponse([
    {
      id: userIds[0],
      userName: "Mike.Zanyatski",
      isVisitor: false,
      firstName: "Mike",
      lastName: "Zanyatski",
      email: "my@gmail.com",
      birthday: "2019-08-19T01:39:25.3240031Z",
      sex: "male",
      status: 1,
      activationStatus: 0,
      terminated: "2019-08-19T01:39:25.3240031Z",
      department: "Marketing",
      workFrom: "2019-08-19T01:39:25.3240031Z",
      location: "Palo Alto",
      notes: "Notes to worker",
      displayName: null,
      title: "Manager",
      contacts: [
        {
          type: "GTalk",
          value: "my@gmail.com"
        }
      ],
      groups: [
        {
          id: "00000000-0000-0000-0000-000000000000",
          name: "Group Name",
          manager: "Jake.Zazhitski"
        }
      ],
      avatarMedium: "url to medium avatar",
      avatar: "url to big avatar",
      isAdmin: false,
      isLDAP: false,
      listAdminModules: ["projects", "crm"],
      isOwner: false,
      cultureName: "en-EN",
      isSSO: false,
      avatarSmall: "url to small avatar",
      profileUrl: ""
    }
  ]);
}

export function deleteUser(userId) {
  return fakeResponse([
    {
      id: userId,
      userName: "Mike.Zanyatski",
      isVisitor: false,
      firstName: "Mike",
      lastName: "Zanyatski",
      email: "my@gmail.com",
      birthday: "2019-08-19T01:39:25.3240031Z",
      sex: "male",
      status: 1,
      activationStatus: 0,
      terminated: "2019-08-19T01:39:25.3240031Z",
      department: "Marketing",
      workFrom: "2019-08-19T01:39:25.3240031Z",
      location: "Palo Alto",
      notes: "Notes to worker",
      displayName: null,
      title: "Manager",
      contacts: [
        {
          type: "GTalk",
          value: "my@gmail.com"
        }
      ],
      groups: [
        {
          id: "00000000-0000-0000-0000-000000000000",
          name: "Group Name",
          manager: "Jake.Zazhitski"
        }
      ],
      avatarMedium: "url to medium avatar",
      avatar: "url to big avatar",
      isAdmin: false,
      isLDAP: false,
      listAdminModules: ["projects", "crm"],
      isOwner: false,
      cultureName: "en-EN",
      isSSO: false,
      avatarSmall: "url to small avatar",
      profileUrl: ""
    }
  ]);
}

export function deleteUsers(userIds) {
  return fakeResponse([
    {
      id: userIds[0],
      userName: "Mike.Zanyatski",
      isVisitor: false,
      firstName: "Mike",
      lastName: "Zanyatski",
      email: "my@gmail.com",
      birthday: "2019-08-19T01:39:25.3240031Z",
      sex: "male",
      status: 1,
      activationStatus: 0,
      terminated: "2019-08-19T01:39:25.3240031Z",
      department: "Marketing",
      workFrom: "2019-08-19T01:39:25.3240031Z",
      location: "Palo Alto",
      notes: "Notes to worker",
      displayName: null,
      title: "Manager",
      contacts: [
        {
          type: "GTalk",
          value: "my@gmail.com"
        }
      ],
      groups: [
        {
          id: "00000000-0000-0000-0000-000000000000",
          name: "Group Name",
          manager: "Jake.Zazhitski"
        }
      ],
      avatarMedium: "url to medium avatar",
      avatar: "url to big avatar",
      isAdmin: false,
      isLDAP: false,
      listAdminModules: ["projects", "crm"],
      isOwner: false,
      cultureName: "en-EN",
      isSSO: false,
      avatarSmall: "url to small avatar",
      profileUrl: ""
    }
  ]);
}

export function sendInstructionsToDelete() {
  return fakeResponse("Instruction has been sent successfully");
}

export function sendInstructionsToChangePassword() {
  return fakeResponse("Instruction has been sent successfully");
}

export function getGroup(groupId) {
  return fakeResponse({
    id: "06448c0a-7f10-4c6d-9ad4-f94de2235778",
    name: "All domain users",
    category: "00000000-0000-0000-0000-000000000000",
    parent: "00000000-0000-0000-0000-000000000000",
    description: null,
    manager: {
      id: "646a6cff-df57-4b83-8ffe-91a24910328c",
      displayName: "Alexey Safronov",
      avatarSmall:
        "/storage/userPhotos/root/646a6cff-df57-4b83-8ffe-91a24910328c_size_32-32.png?_=1787432031",
      profileUrl:
        "http://localhost:8092/products/people/profile.aspx?user=alexey.safronov1"
    },
    members: [
      {
        id: "646a6cff-df57-4b83-8ffe-91a24910328c",
        displayName: "Alexey Safronov",
        avatarSmall:
          "/storage/userPhotos/root/646a6cff-df57-4b83-8ffe-91a24910328c_size_32-32.png?_=1787432031",
        profileUrl:
          "http://localhost:8092/products/people/profile.aspx?user=alexey.safronov1"
      }
    ]
  });
}
