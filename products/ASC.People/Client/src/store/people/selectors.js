import { find, filter } from "lodash";

export function getSelectedUser(selection, userId) {
    return find(selection, function (obj) {
        return obj.id === userId;
    });
};

export function getUserByUserName(users, userName) {
    return find(users, function (obj) {
        return obj.userName === userName;
    });
};

export function isSelected(selection, userId) {
    return getSelectedUser(selection, userId) !== undefined;
};

export function skipUser(selection, userId) {
    return filter(selection, function (obj) {
        return obj.id !== userId;
    });
};

export function getTreeGroups(groups) {
    const treeData = [
        {
            key: "0-0",
            title: "Departments",
            root: true,
            children: groups.map(g => {
                return {
                    key: g.id, title: g.name, root: false
                };
            }) || []
        }
    ];

    return treeData;
};

export const getUserStatus = user => {
    if (user.status === 1 && user.activationStatus === 1) {
        return "normal";
    }
    else if (user.status === 1 && (user.activationStatus === 0 || user.activationStatus === 2)) {
        return "pending";
    }
    else if (user.status === 2) {
        return "disabled";
    }
    else { 
        return "unknown";
    }
};

export const getUserRole = user => {
    if (user.isOwner) return "owner";
    else if (user.isAdmin) return "admin";
    else if (user.isVisitor) return "guest";
    else return "user";
};

export const getContacts = (contacts) => {
    const pattern = {
        contact: [
            { type: "mail", value: "", icon: "MailIcon" },
            { type: "phone", value: "", icon: "PhoneIcon" },
            { type: "mobphone", value: "", icon: "MobileIcon" },
            { type: "gmail", value: "", icon: "GmailIcon" },
            { type: "skype", value: "", icon: "SkypeIcon" },
            { type: "msn", value: "", icon: "WindowsMsnIcon" },
            { type: "icq", value: "", icon: "IcqIcon" },
            { type: "jabber", value: "", icon: "JabberIcon" },
            { type: "aim", value: "", icon: "AimIcon" }
        ],
        social: [
            { type: "facebook", value: "", icon: "ShareFacebookIcon" },
            { type: "livejournal", value: "", icon: "LivejournalIcon" },
            { type: "myspace", value: "", icon: "MyspaceIcon" },
            { type: "twitter", value: "", icon: "ShareTwitterIcon" },
            { type: "blogger", value: "", icon: "BloggerIcon" },
            { type: "yahoo", value: "", icon: "YahooIcon" }
        ]
    };

    const mapContacts = (a, b) => {
        return a.map(a => ({ ...a, ...b.find(({ type }) => type === a.type) })).filter(c => c.value !== "");
    }

    let info = {};

    info.contact = mapContacts(pattern.contact, contacts);
    info.social = mapContacts(pattern.social, contacts);

    return info;
};

const getUserChecked = (user, selected) => {
    const status = getUserStatus(user);
    switch (selected) {
        case "all":
            return true;
        case "active":
            return status === "normal";
        case "disabled":
            return status === "disabled";
        case "invited":
            return status === "pending";
        default:
            return false;
    }
};

export function getUsersBySelected(users, selected) {
    let newSelection = [];
    users.forEach(user => {
        const checked = getUserChecked(user, selected);

        if (checked)
            newSelection.push(user);
    });

    return newSelection;
}