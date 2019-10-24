import * as api from "../services/api";

export function getAdminUsers() {
  return () => {
    return api.getUsers();
  };
}
