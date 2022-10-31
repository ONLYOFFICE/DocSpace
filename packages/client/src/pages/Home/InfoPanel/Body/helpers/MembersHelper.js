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
        key: "docSpaceAdmin",
        title: this.t("Common:DocSpaceAdmin"),
        label: this.t("Common:DocSpaceAdmin"),
        access: ShareAccessRights.FullAccess,
      },
      roomAdmin: {
        key: "roomAdmin",
        title: this.t("Common:RoomAdmin"),
        label: this.t("Common:RoomAdmin"),
        access: ShareAccessRights.RoomManager,
      },
      user: {
        key: "user",
        title: this.t("Common:User"),
        label: this.t("Common:User"),
        access: EmployeeType.Guest,
      },
      editor: {
        key: "editor",
        title: this.t("Translations:RoleEditor"),
        label: this.t("Translations:RoleEditor"),
        access: ShareAccessRights.Editing,
      },
      formFiller: {
        key: "formFiller",
        title: this.t("Translations:RoleFormFiller"),
        label: this.t("Translations:RoleFormFiller"),
        access: ShareAccessRights.FormFilling,
      },
      reviewer: {
        key: "reviewer",
        title: this.t("Translations:RoleReviewer"),
        label: this.t("Translations:RoleReviewer"),
        access: ShareAccessRights.Review,
      },
      commentator: {
        key: "commentator",
        title: this.t("Translations:RoleCommentator"),
        label: this.t("Translations:RoleCommentator"),
        access: ShareAccessRights.Comment,
      },
      viewer: {
        key: "viewer",
        title: this.t("Translations:RoleViewer"),
        label: this.t("Translations:RoleViewer"),
        access: ShareAccessRights.ReadOnly,
      },
    };
  };

  getOptionsByRoomType = (roomType) => {
    if (!roomType) return;

    const options = this.getOptions();

    switch (roomType) {
      case RoomsType.FillingFormsRoom:
        return [options.roomAdmin, options.formFiller, options.viewer];
      case RoomsType.EditingRoom:
        return [options.roomAdmin, options.editor, options.viewer];
      case RoomsType.ReviewRoom:
        return [
          options.roomAdmin,
          options.reviewer,
          options.commentator,
          options.viewer,
        ];
      case RoomsType.ReadOnlyRoom:
        return [options.roomAdmin, options.viewer];
      case RoomsType.CustomRoom:
        return [
          options.roomAdmin,
          options.editor,
          options.formFiller,
          options.reviewer,
          options.commentator,
          options.viewer,
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
