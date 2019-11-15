import { find, filter, cloneDeep } from "lodash";
import { EmployeeActivationStatus, EmployeeStatus } from "../../helpers/constants";
import { store } from 'asc-web-common';
const { isAdmin } = store.auth.selectors;

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

export function isUserSelected(selection, userId) {
    return getSelectedUser(selection, userId) !== undefined;
};

export function skipUser(selection, userId) {
    return filter(selection, function (obj) {
        return obj.id !== userId;
    });
};

export const getUserStatus = user => {
    if (user.status === EmployeeStatus.Active && user.activationStatus === EmployeeActivationStatus.Activated) {
        return "normal";
    }
    else if (user.status === EmployeeStatus.Active && user.activationStatus === EmployeeActivationStatus.Pending) {
        return "pending";
    }
    else if (user.status === EmployeeStatus.Disabled) {
        return "disabled";
    }
    else { 
        return "unknown";
    }
};

export const getUserRole = user => {
    if (user.isOwner) return "owner";
    else if (isAdmin(user)) return "admin";
    else if (user.isVisitor) return "guest";
    else return "user";
};

export const getUserContactsPattern = () => {
    return {
        contact: [
            { type: "mail", icon: "MailIcon" },
            { type: "phone", icon: "PhoneIcon" },
            { type: "mobphone", icon: "MobileIcon" },
            { type: "gmail", icon: "GmailIcon" },
            { type: "skype", icon: "SkypeIcon" },
            { type: "msn", icon: "WindowsMsnIcon" },
            { type: "icq", icon: "IcqIcon" },
            { type: "jabber", icon: "JabberIcon" },
            { type: "aim", icon: "AimIcon" }
        ],
        social: [
            { type: "facebook", icon: "ShareFacebookIcon" },
            { type: "livejournal", icon: "LivejournalIcon" },
            { type: "myspace", icon: "MyspaceIcon" },
            { type: "twitter", icon: "ShareTwitterIcon" },
            { type: "blogger", icon: "BloggerIcon" },
            { type: "yahoo", icon: "YahooIcon" }
        ]
    };
};

export const getUserContacts = (contacts) => {
    const mapContacts = (a, b) => {
        return a.map(a => ({ ...a, ...b.find(({ type }) => type === a.type) }))
                .filter(c => c.icon);
    }

    const info = {};
    const pattern = getUserContactsPattern();

    info.contact = mapContacts(contacts, pattern.contact);
    info.social = mapContacts(contacts, pattern.social);

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
};

export function isUserDisabled(user) {
    return getUserStatus(user) === "disabled";
};

export function getSelectedGroup(groups, selectedGroupId) {
    return find(groups, (group) => group.id === selectedGroupId);
}

export function getSelectionIds(selections) {
    return selections.map((user) => { return user.id });
}

export function toEmployeeWrapper(profile) {
    const emptyData = {
        id: "",
        firstName: "",
        lastName: "",
        email: "",
        password: "",
        birthday: "",
        sex: "male",
        workFrom: "",
        location: "",
        title: "",
        groups: [],
        notes: "",
        contacts: []
    };

    return cloneDeep({ ...emptyData, ...profile });
}

export function mapGroupsToGroupSelectorOptions(groups) {
    return groups.map(group => {
        return {
            key: group.id,
            label: group.name,
            manager: group.manager,
            total: 0
        }
    });
}

export function mapGroupSelectorOptionsToGroups(options) {
    return options.map(option => {
        return {
            id: option.key,
            name: option.label,
            manager: option.manager
        }
    });
}

export function filterGroupSelectorOptions(options, template) { 
    return options.filter(option => {
        return template ? option.label.indexOf(template) > -1 : true;
    })
}