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
