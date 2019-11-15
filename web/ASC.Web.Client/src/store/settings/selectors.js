import { differenceBy } from "lodash";

export function getSelectorOptions(users) {
  return users.map(user => {
    return {
      key: user.id,
      label: user.displayName,
      selected: false
    };
  });
}

export function getUserOptions(users, admins) {
  const sorted = differenceBy(users, admins, "id");
  return sorted.filter(user => !user.isVisitor);
}

export const getUserRole = user => {
  if (user.isOwner) return "owner";
  else if (user.isAdmin) return "admin";
  else if (
    user.listAdminModules !== undefined &&
    user.listAdminModules.includes("people")
  )
    return "admin";
  else if (user.isVisitor) return "guest";
  else return "user";
};
