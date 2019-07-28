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
}