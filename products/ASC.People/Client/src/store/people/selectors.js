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

export const getUserStatus = user => {
    if (user.status === 1 && user.activationStatus === 1) return "normal";
    else if (user.status === 1 && user.activationStatus === 2) return "pending";
    else if (user.status === 2) return "disabled";
    else return "normal";
  };

  export const getUserRole = user => {
    if (user.isOwner) return "owner";
    else if (user.isAdmin) return "admin";
    else if (user.isVisitor) return "guest";
    else return "user";
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