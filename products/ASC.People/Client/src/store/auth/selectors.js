export function isAdmin(user) {
    return user.isAdmin || user.isOwner;
};

export function isMe(user, userName) {
    return userName === "@self" || userName === user.userName;
};