import { ShareAccessRights, RoomsType } from "@docspace/common/constants";

export const getAccessOptions = (
  t,
  roomType = RoomsType.CustomRoom,
  withRemove = false,
  withSeparator = false
) => {
  let options = [];
  const accesses = {
    roomManager: {
      key: "roomManager",
      label: "Room manager",
      title: "Room manager",
      description: "Room manager",
      quota: "Paid",
      color: "#EDC409",
      access: ShareAccessRights.FullAccess,
    },
    editor: {
      key: "editor",
      label: "Editor",
      access: ShareAccessRights.CustomFilter,
    },
    formFiller: {
      key: "formFiller",
      label: "Form filler",
      access: ShareAccessRights.FormFilling,
    },
    reviewer: {
      key: "reviewer",
      label: "Reviewer",
      access: ShareAccessRights.Review,
    },
    commentator: {
      key: "commentator",
      label: "Commentator",
      access: ShareAccessRights.Comment,
    },
    viewer: {
      key: "viewer",
      label: "Viewer",
      access: ShareAccessRights.ReadOnly,
    },
  };

  switch (roomType) {
    case RoomsType.FillingFormsRoom:
      options = [
        accesses.roomManager,
        { key: "s1", isSeparator: withSeparator },
        accesses.formFiller,
        accesses.viewer,
      ];
      break;
    case RoomsType.EditingRoom:
      options = [
        accesses.roomManager,
        { key: "s1", isSeparator: withSeparator },
        accesses.editor,
        accesses.viewer,
      ];
      break;
    case RoomsType.ReviewRoom:
      options = [
        accesses.roomManager,
        { key: "s1", isSeparator: withSeparator },
        accesses.reviewer,
        accesses.commentator,
        accesses.viewer,
      ];
      break;
    case RoomsType.ReadOnlyRoom:
      options = [
        accesses.roomManager,
        { key: "s1", isSeparator: withSeparator },
        accesses.viewer,
      ];
      break;
    case RoomsType.CustomRoom:
      options = [
        accesses.roomManager,
        { key: "s1", isSeparator: withSeparator },
        accesses.editor,
        accesses.formFiller,
        accesses.reviewer,
        accesses.commentator,
        accesses.viewer,
      ];
      break;
  }

  const removeOption = [
    {
      key: "s2",
      isSeparator: true,
    },
    {
      key: "remove",
      label: "Remove",
    },
  ];

  return withRemove ? [...options, ...removeOption] : options;
};
