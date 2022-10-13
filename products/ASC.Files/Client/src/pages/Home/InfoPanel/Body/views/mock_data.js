const files = [
  {
    fileExst: ".png",
    icon: "/static/images/icons/24/image.svg",
    isFolder: false,
    id: 34,
    title: "Windows 12 Wallpaper Purple.png",
  },
  {
    fileExst: ".pptx",
    icon: "/static/images/icons/24/pptx.svg",
    isFolder: false,
    id: 3,
    title: "ONLYOFFICE Sample Presentation With Long Name.pptx",
  },
  {
    fileExst: ".docx",
    icon: "/static/images/icons/24/docx.svg",
    isFolder: false,
    id: 5,
    title: "курсач.docx",
  },
];

const people = [
  {
    email: "nikita.mushka@onlyoffice.com",
    avatar:
      "http://192.168.0.102:8092/images/default_user_photo_size_82-82.png",
    isOwner: true,
    id: "66faa6e4-f133-11ea-b126-00ffeec8b4ef",
    displayName: "Мушка Никита",
    profileUrl: "http://192.168.9.102:8092/products/people/view/administrator",
    role: "Owner",
  },
  {
    email: "yoshiko05@rohan.biz",
    avatar: "",
    isOwner: false,
    id: "1231234",
    displayName: "Rebecca Holt",
    profileUrl: "asdasd",
    role: "Room manager",
  },
  {
    email: "yoshiko05@rohan.biz",
    avatar:
      "https://images.unsplash.com/photo-1519699047748-de8e457a634e?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=880&q=80",
    isOwner: false,
    id: "345970",
    displayName: "Angela Garcia",
    profileUrl: "",
    role: "Viewer",
  },
  {
    email: "kk@mail.ru",
    avatar: "/images/default_user_photo_size_82-82.png",
    isOwner: false,
    id: "5db1213e-7f73-434e-9d6a-af699821d3c9",
    displayName:
      "Some random guy with a really long name, like i mean some dumb long one",
    profileUrl: "http://localhost:8092/products/people/view/kk",
    role: "Viewer",
  },
  {
    email: "ycummerata@yahoo.com",
    avatar: "",
    isOwner: false,
    id: "389457",
    displayName: "",
    profileUrl: "",
    role: "Viewer",
  },
];

const fillingFormsVR = {
  title: "Filling forms room",
  icon: "images/icons/32/folder.svg",
  members: {
    inRoom: [...people.slice(0, 3)],
    expect: [...people.slice(3)],
  },

  history: [
    {
      id: 3,
      user: people[0],
      date: "2022-04-23T19:04:43.511Z",
      action: "files",
      details: { type: "delete", files: [...files] },
    },
    {
      id: 2,
      user: people[0],
      date: "2022-04-23T18:39:43.511Z",
      action: "appointing",
      details: {
        appointedUser: people[1],
        appointedRole: "room administrator",
      },
    },

    {
      id: 1,
      user: people[0],
      date: "2022-04-23T18:35:43.511Z",
      action: "users",
      details: {
        type: "add",
        users: [...people.slice(1)],
      },
    },
    {
      id: 0,
      user: people[0],
      date: "2022-04-23T18:25:43.511Z",
      action: "message",
      details: { message: 'Created room "Filling forms room"' },
    },
  ],
};

export { fillingFormsVR };
