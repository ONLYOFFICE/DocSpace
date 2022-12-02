import { RoomsType } from "@docspace/common/constants";

export const roomTypes = (t) => {
  return [
    {
      type: RoomsType.FillingFormsRoom,
      title: "FillingFormsRoomTitle",
      description: "FillingFormsRoomDescription",
      defaultTag: "Files:FillingFormRooms",
      withSecondaryInfo: true,
    },
    {
      type: RoomsType.EditingRoom,
      title: "CollaborationRoomTitle",
      description: "CollaborationRoomDescription",
      defaultTag: "Files:CollaborationRooms",
      withSecondaryInfo: true,
    },
    {
      type: RoomsType.ReviewRoom,
      title: "ReviewRoomTitle",
      description: "ReviewRoomDescription",
      defaultTag: "Common:Review",
      withSecondaryInfo: true,
    },
    {
      type: RoomsType.ReadOnlyRoom,
      title: "ViewOnlyRoomTitle",
      description: "ViewOnlyRoomDescription",
      defaultTag: "Files:ViewOnlyRooms",
      withSecondaryInfo: true,
    },
    {
      type: RoomsType.CustomRoom,
      title: "CustomRoomTitle",
      description: "CustomRoomDescription",
      defaultTag: "Files:CustomRooms",
      withSecondaryInfo: false,
    },
  ];
};

export const getRoomTypeTitleTranslation = (roomType = 1, t) => {
  switch (roomType) {
    case RoomsType.FillingFormsRoom:
      return t("CreateEditRoomDialog:FillingFormsRoomTitle");
    case RoomsType.EditingRoom:
      return t("CreateEditRoomDialog:CollaborationRoomTitle");
    case RoomsType.ReviewRoom:
      return t("CreateEditRoomDialog:ReviewRoomTitle");
    case RoomsType.ReadOnlyRoom:
      return t("CreateEditRoomDialog:ViewOnlyRoomTitle");
    case RoomsType.CustomRoom:
      return t("CreateEditRoomDialog:CustomRoomTitle");
  }
};

export const getRoomTypeDescriptionTranslation = (roomType = 1, t) => {
  switch (roomType) {
    case RoomsType.FillingFormsRoom:
      return t("CreateEditRoomDialog:FillingFormsRoomDescription");
    case RoomsType.EditingRoom:
      return t("CreateEditRoomDialog:CollaborationRoomDescription");
    case RoomsType.ReviewRoom:
      return t("CreateEditRoomDialog:ReviewRoomDescription");
    case RoomsType.ReadOnlyRoom:
      return t("CreateEditRoomDialog:ViewOnlyRoomDescription");
    case RoomsType.CustomRoom:
      return t("CreateEditRoomDialog:CustomRoomDescription");
  }
};

export const getRoomTypeDefaultTagTranslation = (roomType = 1, t) => {
  switch (roomType) {
    case RoomsType.FillingFormsRoom:
      return t("Files:FillingFormRooms");
    case RoomsType.EditingRoom:
      return t("Files:CollaborationRooms");
    case RoomsType.ReviewRoom:
      return t("Common:Review");
    case RoomsType.ReadOnlyRoom:
      return t("Files:ViewOnlyRooms");
    case RoomsType.CustomRoom:
      return t("Files:CustomRooms");
  }
};
