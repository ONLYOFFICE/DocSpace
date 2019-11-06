import { filter } from "lodash";

export function getUsers(users, ownerId) {
  return filter(users, function(f) {
    return f.id !== ownerId;
  });
}

export function getAdmins(users) {
  const newArray = [];
  users.map(user => {
    if (user.listAdminModules !== undefined) {
      if (!user.listAdminModules.includes("people")) {
        newArray.push(user);
      }
    } else {
      newArray.push(user);
    }
  });
  return newArray.filter(user => !user.isVisitor);
}

export function getSelectorOptions(users) {
  return users.map(user => {
    return {
      key: user.id,
      label: user.displayName,
      selected: false
    };
  });
}
