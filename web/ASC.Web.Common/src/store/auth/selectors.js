export function isAdmin(user) {
    let isPeopleAdmin = user.listAdminModules ? user.listAdminModules.includes('people') : false;
    return user.isAdmin || user.isOwner || isPeopleAdmin;
}

export function isMe(user, userName) {
    return userName === "@self" || user && userName === user.userName;
}

export function getCurrentModule(modules, currentModuleId) {
    return modules.find(module => module.id === currentModuleId);
}