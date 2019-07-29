import _ from "lodash";

export function getSelectedUser(selection, userId) {
    return _.find(selection, function (obj) {
        return obj.id === userId;
    });
};

export function getUserByUserName(users, userName) {
    return _.find(users, function (obj) {
        return obj.userName === userName;
    });
};

export function isSelected(selection, userId) {
    return getSelectedUser(selection, userId) !== undefined;
};

export function skipUser(selection, userId) {
    return _.filter(selection, function (obj) {
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

const getUserDepartment = user => {
    return {
        title: user.department,
        action: () => console.log("Department action")
    };
};

const getUserPhone = user => {
    return {
        title: user.mobilePhone,
        action: () => console.log("Phone action")
    };
};

const getUserEmail = user => {
    return {
        title: user.email,
        action: () => console.log("Email action")
    };
};

const getUserRole = user => {
    if (user.isOwner) return "owner";
    else if (user.isAdmin) return "admin";
    else if (user.isVisitor) return "guest";
    else return "user";
};

const getUserStatus = user => {
    if (user.status === 1 && user.activationStatus === 1) return "normal";
    else if (user.status === 1 && user.activationStatus === 2) return "pending";
    else if (user.status === 2) return "disabled";
    else return "normal";
};

const getUserContextOptions = (user, isAdmin, history, settings) => {
    return [
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
            onClick: () => history.push(`${settings.homepage}/edit/${user.userName}`)
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
    ];
};

const getIsHead = user => {
    return false;
};

export function convertPeople(users, isAdmin, history, settings) {
    return users.map(user => {
        const status = getUserStatus(user);
        return {
            user: user,
            status: status,
            role: getUserRole(user),
            contextOptions: getUserContextOptions(
                user,
                isAdmin,
                history,
                settings
            ),
            department: getUserDepartment(user),
            phone: getUserPhone(user),
            email: getUserEmail(user),
            isHead: getIsHead(user)
        };
    });
};

const getChecked = (status, selected) => {
    let checked;
    switch (selected) {
        case "all":
            checked = true;
            break;
        case "active":
            checked = status === "normal";
            break;
        case "disabled":
            checked = status === "disabled";
            break;
        case "invited":
            checked = status === "pending";
            break;
        default:
            checked = false;
    }

    return checked;
};

export function getUsersBySelected(users, selected) {
    let newSelection = [];
    users.forEach(user => {
        const checked = getChecked(getUserStatus(user), selected);

        if (checked)
            newSelection.push(user);
    });

    return newSelection;
}