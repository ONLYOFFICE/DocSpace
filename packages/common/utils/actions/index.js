import {
  OwnerAccountsActions,
  DocSpaceAdminAccountsActions,
  RoomAdminAccountsActions,
} from "./Accounts";

export const getAccountsTypeActions = (isAdmin, isOwner) => {
  if (isOwner) return OwnerAccountsActions;

  if (isAdmin) return DocSpaceAdminAccountsActions;

  return RoomAdminAccountsActions;
};
