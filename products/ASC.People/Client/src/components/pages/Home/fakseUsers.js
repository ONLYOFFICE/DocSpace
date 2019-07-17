const fakeUsers = [
    {
      id: "1",
      userName: "Helen Walton",
      avatar: "",
      role: "owner",
      status: "normal",
      isHead: false,
      departments: [
        {
          title: "Administration",
          action: () => console.log("Department action")
        }
      ],
      phones: [
        {
          title: "+5 104 6473420",
          action: () => console.log("Phone action")
        }
      ],
      emails: [
        {
          title: "percival1979@yahoo.com",
          action: () => console.log("Email action")
        }
      ],
      contextOptions: [
        {
          key: "key1",
          label: "Send e-mail",
          onClick: () => console.log("Context action: Send e-mail")
        },
        {
          key: "key2",
          label: "Send message",
          onClick: () => console.log("Context action: Send message")
        },
        { key: "key3", isSeparator: true },
        {
          key: "key4",
          label: "Edit",
          onClick: () => console.log("Context action: Edit")
        },
        {
          key: "key5",
          label: "Change password",
          onClick: () => console.log("Context action: Change password")
        },
        {
          key: "key6",
          label: "Change e-mail",
          onClick: () => console.log("Context action: Change e-mail")
        },
        {
          key: "key7",
          label: "Disable",
          onClick: () => console.log("Context action: Disable")
        }
      ]
    },
    {
      id: "2",
      userName: "Nellie Harder",
      avatar: "",
      role: "user",
      status: "normal",
      isHead: true,
      departments: [
        {
          title: "Development",
          action: () => console.log("Department action")
        }
      ],
      phones: [
        {
          title: "+1 716 3748605",
          action: () => console.log("Phone action")
        }
      ],
      emails: [
        {
          title: "herta.reynol@yahoo.com",
          action: () => console.log("Email action")
        }
      ],
      contextOptions: [
        {
          key: "key1",
          label: "Send e-mail",
          onClick: () => console.log("Context action: Send e-mail")
        },
        {
          key: "key2",
          label: "Send message",
          onClick: () => console.log("Context action: Send message")
        },
        { key: "key3", isSeparator: true },
        {
          key: "key4",
          label: "Edit",
          onClick: () => console.log("Context action: Edit")
        },
        {
          key: "key5",
          label: "Change password",
          onClick: () => console.log("Context action: Change password")
        },
        {
          key: "key6",
          label: "Change e-mail",
          onClick: () => console.log("Context action: Change e-mail")
        },
        {
          key: "key7",
          label: "Disable",
          onClick: () => console.log("Context action: Disable")
        }
      ]
    },
    {
      id: "3",
      userName: "Alan Mason",
      avatar: "",
      role: "admin",
      status: "normal",
      isHead: true,
      departments: [
        {
          title: "Administration",
          action: () => console.log("Department action")
        }
      ],
      phones: [
        {
          title: "+3 956 2064314",
          action: () => console.log("Phone action")
        }
      ],
      emails: [
        {
          title: "davin_lindgr@hotmail.com",
          action: () => console.log("Email action")
        }
      ],
      contextOptions: [
        {
          key: "key1",
          label: "Send e-mail",
          onClick: () => console.log("Context action: Send e-mail")
        },
        {
          key: "key2",
          label: "Send message",
          onClick: () => console.log("Context action: Send message")
        },
        { key: "key3", isSeparator: true },
        {
          key: "key4",
          label: "Edit",
          onClick: () => console.log("Context action: Edit")
        },
        {
          key: "key5",
          label: "Change password",
          onClick: () => console.log("Context action: Change password")
        },
        {
          key: "key6",
          label: "Change e-mail",
          onClick: () => console.log("Context action: Change e-mail")
        },
        {
          key: "key7",
          label: "Disable",
          onClick: () => console.log("Context action: Disable")
        }
      ]
    },
    {
      id: "4",
      userName: "Michael Goldstein",
      avatar: "",
      role: "guest",
      status: "normal",
      isHead: false,
      departments: [
        {
          title: "Visitors",
          action: () => console.log("Department action")
        }
      ],
      phones: [
        {
          title: "+7 715 6018678",
          action: () => console.log("Phone action")
        }
      ],
      emails: [
        {
          title: "fidel_kerlu@hotmail.com",
          action: () => console.log("Email action")
        }
      ],
      contextOptions: [
        {
          key: "key1",
          label: "Send e-mail",
          onClick: () => console.log("Context action: Send e-mail")
        },
        {
          key: "key2",
          label: "Send message",
          onClick: () => console.log("Context action: Send message")
        },
        { key: "key3", isSeparator: true },
        {
          key: "key4",
          label: "Edit",
          onClick: () => console.log("Context action: Edit")
        },
        {
          key: "key5",
          label: "Change password",
          onClick: () => console.log("Context action: Change password")
        },
        {
          key: "key6",
          label: "Change e-mail",
          onClick: () => console.log("Context action: Change e-mail")
        },
        {
          key: "key7",
          label: "Disable",
          onClick: () => console.log("Context action: Disable")
        }
      ]
    },
    {
      id: "5",
      userName: "Robert Gardner",
      avatar: "",
      role: "user",
      status: "pending",
      isHead: false,
      departments: [
        {
          title: "Pending",
          action: () => console.log("Department action")
        }
      ],
      phones: [
        {
          title: "+0 000 0000000",
          action: () => console.log("Phone action")
        }
      ],
      emails: [
        {
          title: "robert_gardner@hotmail.com",
          action: () => console.log("Email action")
        }
      ],
      contextOptions: [
        {
          key: "key1",
          label: "Edit",
          onClick: () => console.log("Context action: Edit")
        },
        {
          key: "key2",
          label: "Invite again",
          onClick: () => console.log("Context action: Invite again")
        },
        {
          key: "key3",
          label: "Delete profile",
          onClick: () => console.log("Context action: Delete profile")
        }
      ]
    },
    {
      id: "6",
      userName: "Timothy Morphis",
      avatar: "",
      role: "user",
      status: "disabled",
      isHead: false,
      departments: [
        {
          title: "Disabled",
          action: () => console.log("Department action")
        }
      ],
      phones: [
        {
          title: "+9 641 1689548",
          action: () => console.log("Phone action")
        }
      ],
      emails: [
        {
          title: "timothy_j_morphis@hotmail.com",
          action: () => console.log("Email action")
        }
      ],
      contextOptions: [
        {
          key: "key1",
          label: "Edit",
          onClick: () => console.log("Context action: Edit")
        },
        {
          key: "key2",
          label: "Reassign data",
          onClick: () => console.log("Context action: Reassign data")
        },
        {
          key: "key3",
          label: "Delete personal data",
          onClick: () => console.log("Context action: Delete personal data")
        },
        {
          key: "key4",
          label: "Delete profile",
          onClick: () => console.log("Context action: Delete profile")
        }
      ]
    }
  ];

  export default fakeUsers;