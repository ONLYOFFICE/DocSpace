import {
  ShareAccessRights,
  EmployeeType,
  RoomsType,
} from "@docspace/common/constants";

class MembersHelper {
  constructor(props) {
    this.t = props.t;
  }

  getOptions = () => {
    return {
      docSpaceAdmin: {
        key: "owner",
        label: this.t("Common:Owner"),
        access: ShareAccessRights.FullAccess,
      },
      roomAdmin: {
        key: "roomAdmin",
        label: this.t("Common:RoomAdmin"),
        access: ShareAccessRights.RoomManager,
      },
      viewer: {
        key: "viewer",
        label: this.t("Translations:RoleViewer"),
        access: ShareAccessRights.ReadOnly,
      },
      editor: {
        key: "editor",
        label: this.t("Translations:RoleEditor"),
        access: ShareAccessRights.Editing,
      },
      formFiller: {
        key: "formFiller",
        label: this.t("Translations:RoleFormFiller"),
        access: ShareAccessRights.FormFilling,
      },
      reviewer: {
        key: "reviewer",
        label: this.t("Translations:RoleReviewer"),
        access: ShareAccessRights.Review,
      },
      commentator: {
        key: "commentator",
        label: this.t("Translations:RoleCommentator"),
        access: ShareAccessRights.Comment,
      },
    };
  };

  getOptionsByRoomType = (roomType, canChangeUserRole = false) => {
    if (!roomType) return;

    const options = this.getOptions();

    const deleteOption = canChangeUserRole
      ? [
          { key: "s2", isSeparator: true },
          {
            key: "remove",
            label: this.t("Translations:Remove"),
            access: ShareAccessRights.None,
          },
        ]
      : [];

    switch (roomType) {
      case RoomsType.FillingFormsRoom:
        return [
          options.roomAdmin,
          options.formFiller,
          options.viewer,
          ...deleteOption,
        ];
      case RoomsType.EditingRoom:
        return [
          options.roomAdmin,
          options.editor,
          options.viewer,
          ...deleteOption,
        ];
      case RoomsType.ReviewRoom:
        return [
          options.roomAdmin,
          options.reviewer,
          options.commentator,
          options.viewer,
          ...deleteOption,
        ];
      case RoomsType.ReadOnlyRoom:
        return [options.roomAdmin, options.viewer, ...deleteOption];
      case RoomsType.CustomRoom:
        return [
          options.roomAdmin,
          options.editor,
          options.formFiller,
          options.reviewer,
          options.commentator,
          options.viewer,
          ...deleteOption,
        ];
    }
  };

  getOptionByUserAccess = (access) => {
    if (!access) return;

    const options = this.getOptions();
    const [userOption] = Object.values(options).filter(
      (opt) => opt.access === access
    );

    return userOption;
  };
}

export default MembersHelper;
