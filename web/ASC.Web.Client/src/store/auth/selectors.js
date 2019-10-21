export function isAdmin(user) {
    return user.isAdmin || user.isOwner;
};
